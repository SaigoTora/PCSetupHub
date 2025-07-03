using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

using PCSetupHub.Core.Extensions;
using PCSetupHub.Core.Interfaces;
using PCSetupHub.Core.Middlewares;
using PCSetupHub.Core.Services;
using PCSetupHub.Data;
using PCSetupHub.Data.Models.Hardware;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Repositories.Implementations.PcConfigurations;
using PCSetupHub.Data.Repositories.Implementations.Users;
using PCSetupHub.Data.Repositories.Interfaces.PcConfigurations;
using PCSetupHub.Data.Repositories.Interfaces.Users;


var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, loggerConfig) =>
{
	loggerConfig.ReadFrom.Configuration(context.Configuration);
});

ConfigureServices(builder.Services, builder.Configuration);


var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();

CreateDbIfNotExists(app, logger);
ConfigureMiddleware(app);

logger.LogInformation("Application started and is now listening for requests");
app.Run();


static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{// Add services to the container.
	services.AddControllersWithViews(options =>
	{
		options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
	});

	services.AddHttpClient();
	services.AddDbContext<PcSetupContext>(options =>
		options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

	services.AddDatabaseDeveloperPageExceptionFilter();
	services.AddAuth(configuration)
			.AddExternalAuthProviders(configuration);
	services.ConfigureRateLimiter();
	services.AddScoped<IUserRepository, UserRepository>();
	services.AddScoped<IPcConfigurationRepository, PcConfigurationRepository>();
	services.AddScoped(typeof(IRepository<>), typeof(BaseRepo<>));
	services.AddScoped<IUserService, UserService>();

	services.AddScoped<IHardwareComponentRepository<VideoCard>, VideoCardRepository>();
	services.AddScoped<IHardwareComponentRepository<Motherboard>, MotherboardRepository>();
	services.AddScoped<IHardwareComponentRepository<PowerSupply>, PowerSupplyRepository>();
	services.AddScoped<IHardwareComponentRepository<Hdd>, HddRepository>();
	services.AddScoped<IHardwareComponentRepository<Ram>, RamRepository>();
}
static void CreateDbIfNotExists(IHost host, ILogger<Program> logger)
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
		logger.LogError(ex, "An error occurred creating the DB");
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

	app.UseMiddleware<TokenRefreshMiddleware>();

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