# Giai đoạn 1: Build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Sao chép các file project và restore package
COPY ["WebBanHangOnline.csproj", "."]
RUN dotnet restore "WebBanHangOnline.csproj"

# Sao chép toàn bộ mã nguồn
COPY . .
RUN dotnet build "WebBanHangOnline.csproj" -c Release -o /app/build

# Publish ứng dụng
FROM build AS publish
RUN dotnet publish "WebBanHangOnline.csproj" -c Release -o /app/publish

# Giai đoạn 2: Tạo image để chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebBanHangOnline.dll"]