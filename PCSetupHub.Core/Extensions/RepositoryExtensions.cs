using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using PCSetupHub.Data.Models.Hardware;
using PCSetupHub.Data.Repositories.Base;
using PCSetupHub.Data.Repositories.Implementations.PcConfigurations;
using PCSetupHub.Data.Repositories.Implementations.Users;
using PCSetupHub.Data.Repositories.Interfaces.PcConfigurations;
using PCSetupHub.Data.Repositories.Interfaces.Users;

namespace PCSetupHub.Core.Extensions
{
	public static class RepositoryExtensions
	{
		/// <summary>
		/// Registers application repositories in the dependency injection container.
		/// </summary>
		/// <param name="services">The service collection to configure.</param>
		/// <param name="configuration">The application configuration.</param>
		/// <returns>The updated service collection.</returns>
		public static IServiceCollection AddRepositories(this IServiceCollection services,
			IConfiguration configuration)
		{
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<IPcConfigurationRepository, PcConfigurationRepository>();
			services.AddScoped(typeof(IRepository<>), typeof(BaseRepo<>));
			services.AddScoped<IFriendshipRepository, FriendshipRepository>();
			services.AddScoped<IChatRepository, ChatRepository>();
			services.AddScoped<IMessageRepository, MessageRepository>();
			services.AddScoped<IHardwareComponentRepository<VideoCard>, VideoCardRepository>();
			services.AddScoped<IHardwareComponentRepository<Motherboard>, MotherboardRepository>();
			services.AddScoped<IHardwareComponentRepository<PowerSupply>, PowerSupplyRepository>();
			services.AddScoped<IHardwareComponentRepository<Hdd>, HddRepository>();
			services.AddScoped<IHardwareComponentRepository<Ram>, RamRepository>();

			return services;
		}
	}
}