using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBanHangOnline.Data;
using WebBanHangOnline.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using System.Linq;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/AdminProduct
        public async Task<IActionResult> Index()
        {
            // Lấy sản phẩm kèm theo thông tin danh mục
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return View(products);
        }

        // GET: Admin/AdminProduct/Create
        public IActionResult Create()
        {
            // Sử dụng ViewBag để truyền danh sách danh mục cho View
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Admin/AdminProduct/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Name,Description,Price,Stock,CategoryId")] Product product,
            IFormFile? imageFile)
        {
            // SỬA LỖI: Thêm 2 dòng này để loại bỏ các lỗi validation không cần thiết
            ModelState.Remove("ImageUrl");
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    // Lưu ảnh và gán đường dẫn cho sản phẩm
                    product.ImageUrl = await SaveImage(imageFile);
                }
                else
                {
                    // Cung cấp một ảnh mặc định nếu không có ảnh nào được tải lên
                    product.ImageUrl = "/images/default-product.png";
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã thêm sản phẩm mới thành công!"; // Thêm thông báo thành công
                return RedirectToAction(nameof(Index));
            }

            // Nếu model không hợp lệ, gửi lại danh sách danh mục và hiển thị lại form
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }


        // GET: Admin/AdminProduct/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Admin/AdminProduct/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
    [Bind("Id,Name,Description,Price,Stock,CategoryId,ImageUrl")] Product productFromForm,
    IFormFile? imageFile)
        {
            if (id != productFromForm.Id)
            {
                return NotFound();
            }

            // SỬA LỖI: Tạm thời xóa lỗi của trường Category để ModelState.IsValid hoạt động đúng
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                // **BẮT ĐẦU PHẦN SỬA LỖI QUAN TRỌNG**

                // 1. Lấy sản phẩm gốc từ cơ sở dữ liệu
                var productToUpdate = await _context.Products.FindAsync(id);
                if (productToUpdate == null)
                {
                    return NotFound();
                }

                // 2. Xử lý ảnh (nếu có ảnh mới được tải lên)
                if (imageFile != null)
                {
                    // Xóa ảnh cũ trước
                    if (!string.IsNullOrEmpty(productToUpdate.ImageUrl))
                    {
                        DeleteImage(productToUpdate.ImageUrl);
                    }
                    // Lưu ảnh mới và cập nhật đường dẫn
                    productToUpdate.ImageUrl = await SaveImage(imageFile);
                }
                // Nếu không có ảnh mới, productToUpdate.ImageUrl sẽ giữ nguyên giá trị cũ.

                // 3. Cập nhật các thuộc tính của sản phẩm gốc với dữ liệu từ form
                productToUpdate.Name = productFromForm.Name;
                productToUpdate.Description = productFromForm.Description;
                productToUpdate.Price = productFromForm.Price;
                productToUpdate.Stock = productFromForm.Stock;
                productToUpdate.CategoryId = productFromForm.CategoryId;

                try
                {
                    // 4. Lưu lại các thay đổi vào cơ sở dữ liệu
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Đã cập nhật sản phẩm thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Products.Any(e => e.Id == productToUpdate.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // Nếu model không hợp lệ, gửi lại danh sách danh mục và hiển thị lại form
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", productFromForm.CategoryId);
            return View(productFromForm);
        }


        // GET: Admin/AdminProduct/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(m => m.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }

        // POST: Admin/AdminProduct/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    DeleteImage(product.ImageUrl);
                }
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> SaveImage(IFormFile imageFile)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/products");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
            return "/images/products/" + uniqueFileName;
        }

        private void DeleteImage(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl) || imageUrl == "/images/default-product.png")
            {
                return; // Không xóa ảnh mặc định
            }
            var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl.TrimStart('/'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
        }
    }
}