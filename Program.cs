using IdentityApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<IdentityContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SQLite_Connection")));

builder.Services.AddIdentity<AppUser, AppRole>()
    .AddEntityFrameworkStores<IdentityContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options => { // Şifre olulturma kurallarını yapılandırmak için IdentityOptions kullanıyoruz.
    options.Password.RequireDigit = true; // Şifre en az bir rakam içermelidir.
    options.Password.RequireLowercase = true; // Şifre en az bir küçük harf içermelidir.
    options.Password.RequireNonAlphanumeric = true; // Şifre en az bir özel karakter içermelidir.
    options.Password.RequireUppercase = true; // Şifre en az bir büyük harf içermelidir.
    options.Password.RequiredLength = 6; // Şifre en az 6 karakter uzunluğunda olmalıdır.
    options.Password.RequiredUniqueChars = 1; // Şifre en az 1 benzersiz karakter içermelidir.

    options.User.RequireUniqueEmail = true; // Kullanıcıların benzersiz e-posta adreslerine sahip olmalarını zorunlu kılar.
    options.User.AllowedUserNameCharacters = string.Empty; // Boş string, kullanıcı adı karakter kontrolünü kapatır; FullName'e ne yazılırsa (boşluk, Türkçe karakter, büyük harf vb.) geçerli olur.
});

builder.Services.Configure<IdentityOptions>(options => { // Oturum açma ayarlarını yapılandırmak için IdentityOptions kullanıyoruz.
    // Default Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Hesap kilitlenme süresi 5 dakika olarak ayarlanır.
    options.Lockout.MaxFailedAccessAttempts = 5; // Maksimum başarısız giriş denemesi sayısı 5 olarak ayarlanır.
    options.Lockout.AllowedForNewUsers = true; // Yeni kullanıcılar için kilitleme özelliği etkinleştirilir.
    
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

IdentitySeedData.IdentityTestUser(app);

app.Run();
