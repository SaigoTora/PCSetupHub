using Amazon.S3;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

using PCSetupHub.Core.Extensions;
using PCSetupHub.Core.Interfaces;
using PCSetupHub.Core.Middlewares;
using PCSetupHub.Core.Services;
using PCSetupHub.Core.Settings;
using PCSetupHub.Data;
using PCSetupHub.Data.Models.Hardware;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Repositories.Implementations.PcConfigurations;
using PCSetupHub.Data.Repositories.Implementations.Users;
using PCSetupHub.Data.Repositories.Interfaces.PcConfigurations;
using PCSetupHub.Data.Repositories.Interfaces.Users;
using PCSetupHub.Web.Hubs;


var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, loggerConfig) =>
{
	loggerConfig.ReadFrom.Configuration(context.Configuration);
});

await ConfigureServicesAsync(builder.Services, builder.Configuration);


var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();

CreateDbIfNotExists(app, logger);
ConfigureMiddleware(app);

logger.LogInformation("Application started and is now listening for requests");
app.Run();


static async Task ConfigureServicesAsync(IServiceCollection services, IConfiguration configuration)
{// Add services to the container.
	services.AddControllersWithViews(options =>
	{
		options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
	});

	services.AddHttpClient();

	KeyVaultSecret secretConnection = await AzureSecretService.GetSecretAsync(configuration,
		"ConnectionStrings--DefaultConnection");
	string connectionString = secretConnection.Value;
	services.AddDbContext<PcSetupContext>(options => options.UseSqlServer(connectionString));

	services.AddDatabaseDeveloperPageExceptionFilter();
	services.ConfigureRateLimiter();
	await services.AddAuthAsync(configuration);
	await services.AddExternalAuthProvidersAsync(configuration);
	services.AddAWSService<IAmazonS3>();
	services.AddEndpointsApiExplorer();
	services.AddSwaggerGen();

	services.AddScoped<IUserRepository, UserRepository>();
	services.AddScoped<IPcConfigurationRepository, PcConfigurationRepository>();
	services.AddScoped(typeof(IRepository<>), typeof(BaseRepo<>));
	services.AddScoped<IFriendshipRepository, FriendshipRepository>();
	services.AddScoped<IMessageRepository, MessageRepository>();
	services.AddScoped<IHardwareComponentRepository<VideoCard>, VideoCardRepository>();
	services.AddScoped<IHardwareComponentRepository<Motherboard>, MotherboardRepository>();
	services.AddScoped<IHardwareComponentRepository<PowerSupply>, PowerSupplyRepository>();
	services.AddScoped<IHardwareComponentRepository<Hdd>, HddRepository>();
	services.AddScoped<IHardwareComponentRepository<Ram>, RamRepository>();

	services.AddScoped<IUserService, UserService>();
	services.AddScoped<IUserAccessService, UserAccessService>();
	services.AddScoped<IImageStorageService, ImageStorageService>();

	services.Configure<AwsSettings>(configuration.GetSection("AwsSettings"));
	services.AddSignalR();
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
	else
	{
		app.UseSwagger();
		app.UseSwaggerUI();
	}

	app.UseHttpsRedirection();
	app.UseStaticFiles();
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

	app.MapControllerRoute(
		name: "default",
		pattern: "{controller=Home}/{action=Index}/{id?}")
		.WithStaticAssets();

	app.MapHub<ChatHub>("/chat");
}