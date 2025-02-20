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

		private ModelBuilder _modelBuilder = new();

		private PcSetupContext()
		{ }
		public PcSetupContext(DbContextOptions<PcSetupContext> options)
			: base(options)
		{ }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			_modelBuilder = modelBuilder;
			base.OnModelCreating(_modelBuilder);

			SetTableNames();
			SetTableRelationships();
		}
		private void SetTableNames()
		{
			_modelBuilder.Entity<Comment>().ToTable("Comment");
			_modelBuilder.Entity<Friendship>().ToTable("Friendship");
			_modelBuilder.Entity<FriendshipStatus>().ToTable("FriendshipStatus");
			_modelBuilder.Entity<Message>().ToTable("Message");
			_modelBuilder.Entity<User>().ToTable("User");
			_modelBuilder.Entity<HDD>().ToTable("HDD");
			_modelBuilder.Entity<Motherboard>().ToTable("Motherboard");
			_modelBuilder.Entity<PcConfiguration>().ToTable("PcConfiguration");
			_modelBuilder.Entity<PowerSupply>().ToTable("PowerSupply");
			_modelBuilder.Entity<Processor>().ToTable("Processor");
			_modelBuilder.Entity<RAM>().ToTable("RAM");
			_modelBuilder.Entity<SSD>().ToTable("SSD");
			_modelBuilder.Entity<VideoCard>().ToTable("VideoCard");
			_modelBuilder.Entity<PcConfigurationHDD>().ToTable("PcConfigurationHDD");
			_modelBuilder.Entity<PcConfigurationSSD>().ToTable("PcConfigurationSSD");
			_modelBuilder.Entity<PcConfigurationRAM>().ToTable("PcConfigurationRAM");
		}

		#region Relationships
		private void SetTableRelationships()
		{
			SetCommentRelationships();
			SetFriendshipRelationships();
			SetMessageRelationships();
			SetUserRelationships();
			SetPcConfigurationRelationships();
			SetPcConfigurationHDDsRelationships();
			SetPcConfigurationRAMsRelationships();
			SetPcConfigurationSSDsRelationships();
		}
		private void SetCommentRelationships()
		{
			_modelBuilder.Entity<Comment>()
				.HasOne(c => c.User)
				.WithMany(u => u.ReceivedComments)
				.HasForeignKey(c => c.UserID)
				.OnDelete(DeleteBehavior.Cascade);

			_modelBuilder.Entity<Comment>()
				.HasOne(c => c.Commentator)
				.WithMany(u => u.WrittenComments)
				.HasForeignKey(c => c.CommentatorID)
				.OnDelete(DeleteBehavior.ClientSetNull);
		}
		private void SetFriendshipRelationships()
		{
			_modelBuilder.Entity<Friendship>()
				.HasOne(f => f.User)
				.WithMany(u => u.Friendships)
				.HasForeignKey(f => f.UserID)
				.OnDelete(DeleteBehavior.Cascade);

			_modelBuilder.Entity<Friendship>()
				.HasOne(f => f.Friend)
				.WithMany()
				.HasForeignKey(f => f.FriendID)
				.OnDelete(DeleteBehavior.ClientSetNull);

			_modelBuilder.Entity<Friendship>()
				.HasOne(f => f.FriendshipStatus)
				.WithMany(fs => fs.Friendships)
				.HasForeignKey(f => f.FriendshipStatusID)
				.OnDelete(DeleteBehavior.Restrict);
		}
		private void SetMessageRelationships()
		{
			_modelBuilder.Entity<Message>()
				.HasOne(m => m.Sender)
				.WithMany(u => u.SentMessages)
				.HasForeignKey(m => m.SenderID)
				.OnDelete(DeleteBehavior.Restrict);

			_modelBuilder.Entity<Message>()
				.HasOne(m => m.Receiver)
				.WithMany(u => u.ReceivedMessages)
				.HasForeignKey(m => m.ReceiverID)
				.OnDelete(DeleteBehavior.Restrict);
		}
		private void SetUserRelationships()
		{
			_modelBuilder.Entity<User>()
				.HasOne(u => u.PcConfiguration)
				.WithOne(pc => pc.User)
				.HasForeignKey<User>(u => u.PcConfigurationID)
				.OnDelete(DeleteBehavior.Restrict);
		}
		private void SetPcConfigurationRelationships()
		{
			_modelBuilder.Entity<PcConfiguration>()
				.HasOne(pc => pc.Motherboard)
				.WithMany(m => m.PcConfigurations)
				.HasForeignKey(pc => pc.MotherboardID)
				.OnDelete(DeleteBehavior.Restrict);

			_modelBuilder.Entity<PcConfiguration>()
				.HasOne(pc => pc.PowerSupply)
				.WithMany(ps => ps.PcConfigurations)
				.HasForeignKey(pc => pc.PowerSupplyID)
				.OnDelete(DeleteBehavior.Restrict);

			_modelBuilder.Entity<PcConfiguration>()
				.HasOne(pc => pc.Processor)
				.WithMany(p => p.PcConfigurations)
				.HasForeignKey(pc => pc.ProcessorID)
				.OnDelete(DeleteBehavior.Restrict);

			_modelBuilder.Entity<PcConfiguration>()
				.HasOne(pc => pc.VideoCard)
				.WithMany(v => v.PcConfigurations)
				.HasForeignKey(pc => pc.VideoCardID)
				.OnDelete(DeleteBehavior.Restrict);
		}
		private void SetPcConfigurationHDDsRelationships()
		{
			_modelBuilder.Entity<PcConfigurationHDD>()
				.HasOne(pchdd => pchdd.PcConfiguration)
				.WithMany(pc => pc.PcConfigurationHDDs)
				.HasForeignKey(pchdd => pchdd.PcConfigurationID)
				.OnDelete(DeleteBehavior.Cascade);

			_modelBuilder.Entity<PcConfigurationHDD>()
				.HasOne(pchdd => pchdd.HDD)
				.WithMany(hdd => hdd.PcConfigurationHDDs)
				.HasForeignKey(pchdd => pchdd.HDDID)
				.OnDelete(DeleteBehavior.Cascade);
		}
		private void SetPcConfigurationRAMsRelationships()
		{
			_modelBuilder.Entity<PcConfigurationRAM>()
				.HasOne(pcram => pcram.PcConfiguration)
				.WithMany(pc => pc.PcConfigurationRAMs)
				.HasForeignKey(pcram => pcram.PcConfigurationID)
				.OnDelete(DeleteBehavior.Cascade);

			_modelBuilder.Entity<PcConfigurationRAM>()
				.HasOne(pcram => pcram.RAM)
				.WithMany(ram => ram.PcConfigurationRAMs)
				.HasForeignKey(pcram => pcram.RAMID)
				.OnDelete(DeleteBehavior.Cascade);
		}
		private void SetPcConfigurationSSDsRelationships()
		{
			_modelBuilder.Entity<PcConfigurationSSD>()
				.HasOne(pcssd => pcssd.PcConfiguration)
				.WithMany(pc => pc.PcConfigurationSSDs)
				.HasForeignKey(pcssd => pcssd.PcConfigurationID)
				.OnDelete(DeleteBehavior.Cascade);

			_modelBuilder.Entity<PcConfigurationSSD>()
				.HasOne(pcssd => pcssd.SSD)
				.WithMany(ssd => ssd.PcConfigurationSSDs)
				.HasForeignKey(pcssd => pcssd.SSDID)
				.OnDelete(DeleteBehavior.Cascade);
		}
		#endregion
	}
}