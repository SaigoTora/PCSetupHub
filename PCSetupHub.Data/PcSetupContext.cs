using Microsoft.EntityFrameworkCore;

using PCSetupHub.Data.Models.Attributes;
using PCSetupHub.Data.Models.Hardware;
using PCSetupHub.Data.Models.Relationships;
using PCSetupHub.Data.Models.Users;

namespace PCSetupHub.Data
{
	public class PcSetupContext : DbContext
	{
		public DbSet<Color> Colors { get; set; }
		public DbSet<Comment> Comments { get; set; }
		public DbSet<Friendship> Friendships { get; set; }
		public DbSet<FriendshipStatus> FriendshipStatuses { get; set; }
		public DbSet<Message> Messages { get; set; }
		public DbSet<PrivacyLevel> PrivacyLevels { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<Hdd> Hdds { get; set; }
		public DbSet<Motherboard> Motherboards { get; set; }
		public DbSet<PcType> PcTypes { get; set; }
		public DbSet<PcConfiguration> PcConfigurations { get; set; }
		public DbSet<PowerSupply> PowerSupplies { get; set; }
		public DbSet<Processor> Processors { get; set; }
		public DbSet<Ram> Rams { get; set; }
		public DbSet<Ssd> Ssds { get; set; }
		public DbSet<VideoCard> VideoCards { get; set; }
		public DbSet<ColorHdd> ColorHdds { get; set; }
		public DbSet<ColorMotherboard> ColorMotherboards { get; set; }
		public DbSet<ColorPowerSupply> ColorPowerSupplies { get; set; }
		public DbSet<ColorRam> ColorRams { get; set; }
		public DbSet<ColorVideoCard> ColorVideoCards { get; set; }
		public DbSet<PcConfigurationHdd> PcConfigurationHdds { get; set; }
		public DbSet<PcConfigurationSsd> PcConfigurationSsds { get; set; }
		public DbSet<PcConfigurationRam> PcConfigurationRams { get; set; }

		private ModelBuilder _modelBuilder = new();

		public PcSetupContext()
		{ }
		public PcSetupContext(DbContextOptions<PcSetupContext> options)
			: base(options)
		{ }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			_modelBuilder = modelBuilder;
			base.OnModelCreating(_modelBuilder);

			SetTableNames();
			SetUniqueness();
			SetTableRelationships();
		}
		private void SetTableNames()
		{
			_modelBuilder.Entity<Color>().ToTable("Color");
			_modelBuilder.Entity<Comment>().ToTable("Comment");
			_modelBuilder.Entity<Friendship>().ToTable("Friendship");
			_modelBuilder.Entity<FriendshipStatus>().ToTable("FriendshipStatus");
			_modelBuilder.Entity<Message>().ToTable("Message");
			_modelBuilder.Entity<PrivacyLevel>().ToTable("PrivacyLevel");
			_modelBuilder.Entity<User>().ToTable("User");
			_modelBuilder.Entity<Hdd>().ToTable("Hdd");
			_modelBuilder.Entity<Motherboard>().ToTable("Motherboard");
			_modelBuilder.Entity<PcType>().ToTable("PcType");
			_modelBuilder.Entity<PcConfiguration>().ToTable("PcConfiguration");
			_modelBuilder.Entity<PowerSupply>().ToTable("PowerSupply");
			_modelBuilder.Entity<Processor>().ToTable("Processor");
			_modelBuilder.Entity<Ram>().ToTable("Ram");
			_modelBuilder.Entity<Ssd>().ToTable("Ssd");
			_modelBuilder.Entity<VideoCard>().ToTable("VideoCard");
			_modelBuilder.Entity<ColorHdd>().ToTable("ColorHdd");
			_modelBuilder.Entity<ColorMotherboard>().ToTable("ColorMotherboard");
			_modelBuilder.Entity<ColorPowerSupply>().ToTable("ColorPowerSupply");
			_modelBuilder.Entity<ColorRam>().ToTable("ColorRam");
			_modelBuilder.Entity<ColorVideoCard>().ToTable("ColorVideoCard");
			_modelBuilder.Entity<PcConfigurationHdd>().ToTable("PcConfigurationHdd");
			_modelBuilder.Entity<PcConfigurationSsd>().ToTable("PcConfigurationSsd");
			_modelBuilder.Entity<PcConfigurationRam>().ToTable("PcConfigurationRam");
		}
		private void SetUniqueness()
		{
			_modelBuilder.Entity<FriendshipStatus>()
				.HasIndex(fs => fs.Status)
				.IsUnique();

			_modelBuilder.Entity<Color>()
				.HasIndex(c => c.Name)
				.IsUnique();

			_modelBuilder.Entity<PrivacyLevel>()
				.HasIndex(pl => pl.Name)
				.IsUnique();

			_modelBuilder.Entity<User>()
				.HasIndex(u => u.Login)
				.IsUnique();
			_modelBuilder.Entity<User>()
				.HasIndex(u => u.Email)
				.IsUnique();
			_modelBuilder.Entity<User>()
				.HasIndex(u => u.PcConfigurationId)
				.IsUnique();

			_modelBuilder.Entity<PcType>()
				.HasIndex(pcType => pcType.Name)
				.IsUnique();
		}

		#region Relationships
		private void SetTableRelationships()
		{
			SetCommentRelationships();
			SetFriendshipRelationships();
			SetMessageRelationships();
			SetUserRelationships();

			SetPcConfigurationRelationships();
			SetPcConfigurationHddRelationships();
			SetPcConfigurationRamRelationships();
			SetPcConfigurationSsdRelationships();

			SetColorHddRelationships();
			SetColorMotherboardRelationships();
			SetColorPowerSupplyRelationships();
			SetColorRamRelationships();
			SetColorVideoCardRelationships();
		}

		private void SetCommentRelationships()
		{
			_modelBuilder.Entity<Comment>()
				.HasOne(c => c.User)
				.WithMany(u => u.ReceivedComments)
				.HasForeignKey(c => c.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			_modelBuilder.Entity<Comment>()
				.HasOne(c => c.Commentator)
				.WithMany(u => u.WrittenComments)
				.HasForeignKey(c => c.CommentatorId)
				.OnDelete(DeleteBehavior.Restrict);
		}
		private void SetFriendshipRelationships()
		{
			_modelBuilder.Entity<Friendship>()
				.HasOne(f => f.Initiator)
				.WithMany(u => u.SentFriendRequests)
				.HasForeignKey(f => f.InitiatorId)
				.OnDelete(DeleteBehavior.Restrict);

			_modelBuilder.Entity<Friendship>()
				.HasOne(f => f.Friend)
				.WithMany(u => u.ReceivedFriendRequests)
				.HasForeignKey(f => f.FriendId)
				.OnDelete(DeleteBehavior.Restrict);

			_modelBuilder.Entity<Friendship>()
				.HasOne(f => f.FriendshipStatus)
				.WithMany(fs => fs.Friendships)
				.HasForeignKey(f => f.FriendshipStatusId)
				.OnDelete(DeleteBehavior.Restrict);
		}
		private void SetMessageRelationships()
		{
			_modelBuilder.Entity<Message>()
				.HasOne(m => m.Sender)
				.WithMany(u => u.SentMessages)
				.HasForeignKey(m => m.SenderId)
				.OnDelete(DeleteBehavior.Restrict);

			_modelBuilder.Entity<Message>()
				.HasOne(m => m.Receiver)
				.WithMany(u => u.ReceivedMessages)
				.HasForeignKey(m => m.ReceiverId)
				.OnDelete(DeleteBehavior.Restrict);
		}
		private void SetUserRelationships()
		{
			_modelBuilder.Entity<User>()
				.HasOne(u => u.FollowersAccess)
				.WithMany(pl => pl.FollowersAccessUsers)
				.HasForeignKey(u => u.FollowersAccessId)
				.OnDelete(DeleteBehavior.Restrict);

			_modelBuilder.Entity<User>()
				.HasOne(u => u.FollowingsAccess)
				.WithMany(pl => pl.FollowingsAccessUsers)
				.HasForeignKey(u => u.FollowingsAccessId)
				.OnDelete(DeleteBehavior.Restrict);

			_modelBuilder.Entity<User>()
				.HasOne(u => u.MessagesAccess)
				.WithMany(pl => pl.MessagesAccessUsers)
				.HasForeignKey(u => u.MessagesAccessId)
				.OnDelete(DeleteBehavior.Restrict);

			_modelBuilder.Entity<User>()
				.HasOne(u => u.PcConfigAccess)
				.WithMany(pl => pl.PcConfigAccessUsers)
				.HasForeignKey(u => u.PcConfigAccessId)
				.OnDelete(DeleteBehavior.Restrict);

			_modelBuilder.Entity<User>()
				.HasOne(u => u.PcConfiguration)
				.WithOne(pc => pc.User)
				.HasForeignKey<User>(u => u.PcConfigurationId)
				.OnDelete(DeleteBehavior.Cascade);
		}

		private void SetPcConfigurationRelationships()
		{
			_modelBuilder.Entity<PcConfiguration>()
				.HasOne(pc => pc.Type)
				.WithMany(t => t.PcConfigurations)
				.HasForeignKey(pc => pc.TypeId)
				.OnDelete(DeleteBehavior.Restrict);

			_modelBuilder.Entity<PcConfiguration>()
				.HasOne(pc => pc.Motherboard)
				.WithMany(m => m.PcConfigurations)
				.HasForeignKey(pc => pc.MotherboardId)
				.OnDelete(DeleteBehavior.Restrict);

			_modelBuilder.Entity<PcConfiguration>()
				.HasOne(pc => pc.PowerSupply)
				.WithMany(ps => ps.PcConfigurations)
				.HasForeignKey(pc => pc.PowerSupplyId)
				.OnDelete(DeleteBehavior.Restrict);

			_modelBuilder.Entity<PcConfiguration>()
				.HasOne(pc => pc.Processor)
				.WithMany(p => p.PcConfigurations)
				.HasForeignKey(pc => pc.ProcessorId)
				.OnDelete(DeleteBehavior.Restrict);

			_modelBuilder.Entity<PcConfiguration>()
				.HasOne(pc => pc.VideoCard)
				.WithMany(v => v.PcConfigurations)
				.HasForeignKey(pc => pc.VideoCardId)
				.OnDelete(DeleteBehavior.Restrict);
		}
		private void SetPcConfigurationHddRelationships()
		{
			_modelBuilder.Entity<PcConfigurationHdd>()
				.HasOne(pcHdd => pcHdd.PcConfiguration)
				.WithMany(pc => pc.PcConfigurationHdds)
				.HasForeignKey(pcHdd => pcHdd.PcConfigurationId)
				.OnDelete(DeleteBehavior.Cascade);

			_modelBuilder.Entity<PcConfigurationHdd>()
				.HasOne(pcHdd => pcHdd.Hdd)
				.WithMany(hdd => hdd.PcConfigurationHdds)
				.HasForeignKey(pcHdd => pcHdd.HddId)
				.OnDelete(DeleteBehavior.Cascade);
		}
		private void SetPcConfigurationRamRelationships()
		{
			_modelBuilder.Entity<PcConfigurationRam>()
				.HasOne(pcRam => pcRam.PcConfiguration)
				.WithMany(pc => pc.PcConfigurationRams)
				.HasForeignKey(pcRam => pcRam.PcConfigurationId)
				.OnDelete(DeleteBehavior.Cascade);

			_modelBuilder.Entity<PcConfigurationRam>()
				.HasOne(pcRam => pcRam.Ram)
				.WithMany(ram => ram.PcConfigurationRams)
				.HasForeignKey(pcRam => pcRam.RamId)
				.OnDelete(DeleteBehavior.Cascade);
		}
		private void SetPcConfigurationSsdRelationships()
		{
			_modelBuilder.Entity<PcConfigurationSsd>()
				.HasOne(pcSsd => pcSsd.PcConfiguration)
				.WithMany(pc => pc.PcConfigurationSsds)
				.HasForeignKey(pcSsd => pcSsd.PcConfigurationId)
				.OnDelete(DeleteBehavior.Cascade);

			_modelBuilder.Entity<PcConfigurationSsd>()
				.HasOne(pcSsd => pcSsd.Ssd)
				.WithMany(ssd => ssd.PcConfigurationSsds)
				.HasForeignKey(pcSsd => pcSsd.SsdId)
				.OnDelete(DeleteBehavior.Cascade);
		}

		private void SetColorHddRelationships()
		{
			_modelBuilder.Entity<ColorHdd>()
				.HasOne(colorHdd => colorHdd.Color)
				.WithMany(c => c.ColorHdds)
				.HasForeignKey(colorHdd => colorHdd.ColorId)
				.OnDelete(DeleteBehavior.Cascade);

			_modelBuilder.Entity<ColorHdd>()
				.HasOne(colorHdd => colorHdd.Hdd)
				.WithMany(hdd => hdd.ColorHdds)
				.HasForeignKey(colorHdd => colorHdd.HddId)
				.OnDelete(DeleteBehavior.Cascade);
		}
		private void SetColorMotherboardRelationships()
		{
			_modelBuilder.Entity<ColorMotherboard>()
				.HasOne(colorM => colorM.Color)
				.WithMany(c => c.ColorMotherboards)
				.HasForeignKey(colorM => colorM.ColorId)
				.OnDelete(DeleteBehavior.Cascade);

			_modelBuilder.Entity<ColorMotherboard>()
				.HasOne(colorM => colorM.Motherboard)
				.WithMany(m => m.ColorMotherboards)
				.HasForeignKey(colorM => colorM.MotherboardId)
				.OnDelete(DeleteBehavior.Cascade);
		}
		private void SetColorPowerSupplyRelationships()
		{
			_modelBuilder.Entity<ColorPowerSupply>()
				.HasOne(colorPS => colorPS.Color)
				.WithMany(c => c.ColorPowerSupplies)
				.HasForeignKey(colorPS => colorPS.ColorId)
				.OnDelete(DeleteBehavior.Cascade);

			_modelBuilder.Entity<ColorPowerSupply>()
				.HasOne(colorPS => colorPS.PowerSupply)
				.WithMany(ps => ps.ColorPowerSupplies)
				.HasForeignKey(colorPS => colorPS.PowerSupplyId)
				.OnDelete(DeleteBehavior.Cascade);
		}
		private void SetColorRamRelationships()
		{
			_modelBuilder.Entity<ColorRam>()
				.HasOne(colorRam => colorRam.Color)
				.WithMany(c => c.ColorRams)
				.HasForeignKey(colorRam => colorRam.ColorId)
				.OnDelete(DeleteBehavior.Cascade);

			_modelBuilder.Entity<ColorRam>()
				.HasOne(colorRam => colorRam.Ram)
				.WithMany(ram => ram.ColorRams)
				.HasForeignKey(colorRam => colorRam.RamId)
				.OnDelete(DeleteBehavior.Cascade);
		}
		private void SetColorVideoCardRelationships()
		{
			_modelBuilder.Entity<ColorVideoCard>()
				.HasOne(colorVC => colorVC.Color)
				.WithMany(c => c.ColorVideoCards)
				.HasForeignKey(colorVC => colorVC.ColorId)
				.OnDelete(DeleteBehavior.Cascade);

			_modelBuilder.Entity<ColorVideoCard>()
				.HasOne(colorVC => colorVC.VideoCard)
				.WithMany(vc => vc.ColorVideoCards)
				.HasForeignKey(colorVC => colorVC.VideoCardId)
				.OnDelete(DeleteBehavior.Cascade);
		}
		#endregion
	}
}