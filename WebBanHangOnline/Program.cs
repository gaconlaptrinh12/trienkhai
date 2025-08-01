using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebBanHangOnline.Data;
using WebBanHangOnline.Models;
using Microsoft.AspNetCore.Http;
using Prometheus; // Thêm thư viện Prometheus để thu thập metrics

var builder = WebApplication.CreateBuilder(args);

// Thêm dòng này để chỉ định cổng lắng nghe của ứng dụng, cần thiết cho Docker
builder.WebHost.UseUrls("http://*:8083", "https://*:8084");

// 1. Cấu hình kết nối đến SQL Server
// Lấy chuỗi kết nối từ biến môi trường hoặc file appsettings.
// Trong Kubernetes, giá trị này sẽ được cung cấp từ app-deployment.yml
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlServerOptionsAction: sqlOptions =>
    {
        // Thêm chính sách thử lại khi kết nối thất bại, rất hữu ích trong K8s khi DB có thể mất một lúc để khởi động
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 10,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    }));

// 2. Cấu hình Identity
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// 3. Cấu hình đường dẫn cho các trang của Identity
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// 4. Thêm dịch vụ cho Controller và View
builder.Services.AddControllersWithViews();

// 5. Cấu hình Session để lưu giỏ hàng
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    // Cấu hình SecurePolicy và SameSiteMode để tương thích với LoadBalancer/Ingress Controller
    // trong Kubernetes, nơi HTTPS được xử lý ở tầng ngoài.
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None;
});


var app = builder.Build();

// Thêm cấu hình cho Prometheus
// Các dòng này sẽ kích hoạt endpoint /metrics để Prometheus có thể scrape dữ liệu
app.UseRouting(); // Cần có UseRouting() trước UseHttpMetrics()
app.UseHttpMetrics(); // Thu thập các metrics về HTTP request (thời gian, số lượng)

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();

// Đảm bảo UseRouting() nằm trước cả UseSession() và UseAuthentication()
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// Thực hiện di chuyển cơ sở dữ liệu và seed dữ liệu khi khởi động
// Điều này giúp đảm bảo schema DB luôn được cập nhật trong mọi môi trường
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        // Bước 1: Áp dụng các migration còn thiếu
        dbContext.Database.Migrate();
        Console.WriteLine("Database migration applied successfully.");
        // Bước 2: Seed dữ liệu ban đầu
        await SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB or migrating.");
    }
}

// Map các route
app.MapControllerRoute(
    name: "Admin",
    pattern: "{area:exists}/{controller=Admin}/{action=Dashboard}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Tạo endpoint /metrics cho Prometheus.
// Chú ý: Dòng này phải đứng sau các app.UseRouting() và app.MapControllerRoute()
app.MapMetrics();

app.Run();
