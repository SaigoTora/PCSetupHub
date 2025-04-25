using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using PCSetupHub.Core.Extensions;
using PCSetupHub.Core.Interfaces;
using PCSetupHub.Core.Services;
using PCSetupHub.Data;
using PCSetupHub.Data.Repositories.Implementations;
using PCSetupHub.Data.Repositories.Interfaces;


var builder = WebApplication.CreateBuilder(args);
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();
CreateDbIfNotExists(app);
ConfigureMiddleware(app);
app.Run();

static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{// Add services to the container.
	services.AddControllersWithViews(options =>
	{
		options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
	});

	services.AddDbContext<PcSetupContext>(options =>
		options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

	services.AddDatabaseDeveloperPageExceptionFilter();
	services.AddAuth(configuration)
			.AddExternalAuthProviders(configuration);
	services.ConfigureRateLimiter();
	services.AddScoped<IUserRepository, UserRepository>();
	services.AddScoped<IUserService, UserService>();
}
static void CreateDbIfNotExists(IHost host)
{
	using var scope = host.Services.CreateScope();
	var services = scope.ServiceProvider;
	try
	{
		var context = services.GetRequiredService<PcSetupContext>();
		var userService = services.GetRequiredService<IUserService>();
		DbInitializer.Initialize(context, userService.RegisterAsync);
	}
	catch (Exception ex)
	{
		var logger = services.GetRequiredService<ILogger<Program>>();
		logger.LogError(ex, "An error occurred creating the DB.");
	}
}
static void ConfigureMiddleware(WebApplication app)
{// Configure the HTTP request pipeline.
	if (!app.Environment.IsDevelopment())
	{
		app.UseExceptionHandler("/Error/Index");
		app.UseStatusCodePagesWithReExecute("/Error/{0}");
		app.UseHsts();
	}

	app.UseHttpsRedirection();
	app.UseRouting();

	app.UseCookiePolicy(new CookiePolicyOptions
	{
		MinimumSameSitePolicy = SameSiteMode.Strict,
		HttpOnly = HttpOnlyPolicy.Always,
		Secure = CookieSecurePolicy.Always
	});

	app.UseRateLimiter();
	app.UseAuthentication();
	app.UseAuthorization();

	app.MapStaticAssets();

	app.MapControllerRoute(
		name: "default",
		pattern: "{controller=Home}/{action=Index}/{id?}")
		.WithStaticAssets();
}