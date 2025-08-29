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

	await ConfigureDatabaseAndCacheAsync(services, configuration);

	services.AddDatabaseDeveloperPageExceptionFilter();
	services.ConfigureRateLimiter();
	await services.AddAuthAsync(configuration);
	await services.AddExternalAuthProvidersAsync(configuration);

	services.AddAWSService<IAmazonS3>()
		.AddEndpointsApiExplorer()
		.AddSwaggerGen()
		.AddRepositories(configuration);

	services.AddScoped<IUserService, UserService>();
	services.AddScoped<IUserAccessService, UserAccessService>();
	services.AddScoped<IImageStorageService, ImageStorageService>();
	services.AddSingleton<ICacheService, CacheService>();
	services.AddHttpClient<INewsApiService, NewsApiService>(client =>
	{
		client.DefaultRequestHeaders.Add("User-Agent", "PCSetupHubApp/1.0");
	});

	services.Configure<AwsSettings>(configuration.GetSection("AwsSettings"));
	services.AddSignalR();
}
static async Task ConfigureDatabaseAndCacheAsync(IServiceCollection services,
	IConfiguration configuration)
{
	KeyVaultSecret secretDb = await AzureSecretService.GetSecretAsync(configuration,
		"ConnectionStrings--DefaultConnection");
	services.AddDbContext<PcSetupContext>(options => options.UseSqlServer(secretDb.Value));

	KeyVaultSecret secretRedis = await AzureSecretService.GetSecretAsync(configuration,
		"ConnectionStrings--RedisConnection");
	services.AddStackExchangeRedisCache(options =>
	{
		options.Configuration = secretRedis.Value;
	});
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