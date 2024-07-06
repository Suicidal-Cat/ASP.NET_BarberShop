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
using BarberShopWeb.Hateoas;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BarberShopDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
	{
		options.Password.RequiredLength = 8;
		options.Password.RequireDigit = false;
		options.Password.RequireLowercase = false;
		options.Password.RequireUppercase = false;
		options.Password.RequireNonAlphanumeric = false;
		options.SignIn.RequireConfirmedEmail = true;
		options.SignIn.RequireConfirmedAccount= true;
	})
	.AddEntityFrameworkStores<BarberShopDbContext>()
	.AddDefaultTokenProviders();

builder.Services
.AddAuthentication() // Cookie by default
	.AddCookie(options =>
	{
		options.LoginPath = "/Account/Unauthorized/";
		options.AccessDeniedPath = "/Account/Forbidden/";
	})
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = false,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = builder.Configuration["JWT:Issuer"],
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
		};
	});

builder.Services.AddScoped<IEmailSender, EmailSender> ();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IServiceService,ServiceService>();
builder.Services.AddScoped<IServiceCategoryService, ServiceCategoryService>();
builder.Services.AddScoped<IBarberService, BarberService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<JWTService>();
builder.Services.AddCors();
// Add services to the container.
builder.Services.AddControllersWithViews(opt =>
{
    opt.OutputFormatters.Add(new HateoasJsonOutputFormmater());
});



var app = builder.Build();
// Configure the HTTP request pipeline.

app.UseCors(opt =>
{
	opt.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins(builder.Configuration["JWT:ClientUrl"]);
});

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

app.Run();
