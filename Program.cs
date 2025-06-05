using Microsoft.AspNetCore.Identity;
using MiniAccountManagementSystem.IdentityRelatedStore;
using MiniAccountManagementSystem.Interfaces;
using MiniAccountManagementSystem.Models.Identity;
using MiniAccountManagementSystem.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);


builder.Services.AddAuthentication("Identity.Application")
    .AddCookie("Identity.Application", options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });


builder.Services.AddIdentityCore<User>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
})
.AddSignInManager()
.AddUserStore<UserStore>()
.AddDefaultTokenProviders();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
