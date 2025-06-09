using Microsoft.AspNetCore.Identity;
using MiniAccountManagementSystem.IdentityRelatedStores;
using MiniAccountManagementSystem.Interfaces;
using MiniAccountManagementSystem.Models.Identity;
using MiniAccountManagementSystem.Repository;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IVoucherRepository, VoucherRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();

builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);



builder.Services.AddAuthentication("Identity.Application")
    .AddCookie("Identity.Application", options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
    });


builder.Services.AddIdentityCore<User>(options =>
{
    options.Password.RequiredLength = 8;
})
.AddRoles<Role>()
.AddSignInManager()
.AddUserStore<UserStore>()
.AddRoleStore<RoleStore>()
.AddDefaultTokenProviders();


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("AccountantOnly", policy => policy.RequireRole("Accountant"));
    options.AddPolicy("AdminOrAccountant", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole("Admin") || context.User.IsInRole("Accountant")));
});

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
