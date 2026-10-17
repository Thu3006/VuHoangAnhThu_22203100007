using Microsoft.EntityFrameworkCore;
using QuanLyBaiDoXe_UNETI.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Thêm dịch vụ MVC
builder.Services.AddControllersWithViews();

// 2. Đăng ký DbContext với chuỗi kết nối DefaultConnection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Cấu hình Session quản lý phiên đăng nhập và phân quyền
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 4. Cho phép truy xuất HttpContext trong Razor Views
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// 5. Tự động nạp dữ liệu mẫu (Seed Data) khi ứng dụng khởi chạy
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        SeedData.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Lỗi xảy ra trong quá trình nạp dữ liệu mẫu (Seed Data).");
    }
}

// Cấu hình Middleware HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Bắt buộc đặt app.UseSession() trước UseAuthorization()
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=TaiKhoan}/{action=DangNhap}/{id?}");

app.Run();