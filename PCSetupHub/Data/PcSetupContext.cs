using Microsoft.EntityFrameworkCore;

using PCSetupHub.Models.Hardware;
using PCSetupHub.Models.Relationships;
using PCSetupHub.Models.Users;

namespace PCSetupHub.Data
{
	public class PcSetupContext : DbContext
	{
		public DbSet<Comment> Comments { get; set; }
		public DbSet<Friendship> Friendships { get; set; }
		public DbSet<FriendshipStatus> FriendshipStatuses { get; set; }
		public DbSet<Message> Messages { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<HDD> HDDs { get; set; }
		public DbSet<Motherboard> Motherboards { get; set; }
		public DbSet<PcConfiguration> PcConfigurations { get; set; }
		public DbSet<PowerSupply> PowerSupplies { get; set; }
		public DbSet<Processor> Processors { get; set; }
		public DbSet<RAM> RAMs { get; set; }
		public DbSet<SSD> SSDs { get; set; }
		public DbSet<VideoCard> VideoCards { get; set; }
		public DbSet<PcConfigurationHDD> PcConfigurationHDDs { get; set; }
		public DbSet<PcConfigurationSSD> PcConfigurationSSDs { get; set; }
		public DbSet<PcConfigurationRAM> PcConfigurationRAMs { get; set; }

		public PcSetupContext(DbContextOptions<PcSetupContext> options) : base(options)
		{ }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Comment>().ToTable("Comment");
			modelBuilder.Entity<Friendship>().ToTable("Friendship");
			modelBuilder.Entity<FriendshipStatus>().ToTable("FriendshipStatus");
			modelBuilder.Entity<Message>().ToTable("Message");
			modelBuilder.Entity<User>().ToTable("User");
			modelBuilder.Entity<HDD>().ToTable("HDD");
			modelBuilder.Entity<Motherboard>().ToTable("Motherboard");
			modelBuilder.Entity<PcConfiguration>().ToTable("PcConfiguration");
			modelBuilder.Entity<PowerSupply>().ToTable("PowerSupply");
			modelBuilder.Entity<Processor>().ToTable("Processor");
			modelBuilder.Entity<RAM>().ToTable("RAM");
			modelBuilder.Entity<SSD>().ToTable("SSD");
			modelBuilder.Entity<VideoCard>().ToTable("VideoCard");
			modelBuilder.Entity<PcConfigurationHDD>().ToTable("PcConfigurationHDD");
			modelBuilder.Entity<PcConfigurationSSD>().ToTable("PcConfigurationSSD");
			modelBuilder.Entity<PcConfigurationRAM>().ToTable("PcConfigurationRAM");

			modelBuilder.Entity<Comment>()
				.HasOne(c => c.User)
				.WithMany(u => u.Comments)
				.HasForeignKey(c => c.UserID);

			modelBuilder.Entity<Comment>()
				.HasOne(c => c.Commentator)
				.WithMany()
				.HasForeignKey(c => c.CommentatorID)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Friendship>()
				.HasOne(f => f.User)
				.WithMany()
				.HasForeignKey(f => f.UserID)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Friendship>()
				.HasOne(f => f.Friend)
				.WithMany()
				.HasForeignKey(f => f.FriendID)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Message>()
				.HasOne(m => m.Sender)
				.WithMany()
				.HasForeignKey(m => m.SenderID)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Message>()
				.HasOne(m => m.Receiver)
				.WithMany()
				.HasForeignKey(m => m.ReceiverID)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}