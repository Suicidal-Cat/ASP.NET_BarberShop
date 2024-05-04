using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BarberShop.Infrastructure;
using Microsoft.AspNetCore.Identity.UI.Services;
using BarberShop.Utils;
using DataAccessLayer.UnitOfWork;
using BarberShop.Services.Interfaces;
using BarberShop.Services.ImplementationDatabase;
using BarberShop.Domain;
using BarberShop.Services.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BarberShopDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddIdentity<IdentityUser,IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<BarberShopDbContext>().AddDefaultTokenProviders();
builder.Services.AddScoped<IEmailSender, EmailSender> ();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IServiceService,ServiceService>();
builder.Services.AddScoped<IServiceCategoryService, ServiceCategoryService>();
builder.Services.AddScoped<IBarberService, BarberService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuerSigningKey=true,
			IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
			ValidateIssuer=true,
			ValidateAudience=false,
		};
	});

builder.Services.AddScoped<JWTService>();
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();	

app.UseAuthorization();
app.MapRazorPages();
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
	name: "MobileControllers",
	pattern: "mobile/{controller}/{action}/{id?}");

app.Run();
