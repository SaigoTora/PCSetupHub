using Microsoft.EntityFrameworkCore;

using PCSetupHub.Models.Attributes;
using PCSetupHub.Models.Hardware;
using PCSetupHub.Models.Relationships;
using PCSetupHub.Models.Users;

namespace PCSetupHub.Data
{
	public class PcSetupContext : DbContext
	{
		public DbSet<Color> Colors { get; set; }
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
		public DbSet<ColorHDD> ColorHDDs { get; set; }
		public DbSet<ColorMotherboard> ColorMotherboards { get; set; }
		public DbSet<ColorPowerSupply> ColorPowerSupplies { get; set; }
		public DbSet<ColorRAM> ColorRams { get; set; }
		public DbSet<ColorVideoCard> ColorVideoCards { get; set; }
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
			_modelBuilder.Entity<User>().ToTable("User");
			_modelBuilder.Entity<HDD>().ToTable("HDD");
			_modelBuilder.Entity<Motherboard>().ToTable("Motherboard");
			_modelBuilder.Entity<PcConfiguration>().ToTable("PcConfiguration");
			_modelBuilder.Entity<PowerSupply>().ToTable("PowerSupply");
			_modelBuilder.Entity<Processor>().ToTable("Processor");
			_modelBuilder.Entity<RAM>().ToTable("RAM");
			_modelBuilder.Entity<SSD>().ToTable("SSD");
			_modelBuilder.Entity<VideoCard>().ToTable("VideoCard");
			_modelBuilder.Entity<ColorHDD>().ToTable("ColorHDD");
			_modelBuilder.Entity<ColorMotherboard>().ToTable("ColorMotherboard");
			_modelBuilder.Entity<ColorPowerSupply>().ToTable("ColorPowerSupply");
			_modelBuilder.Entity<ColorRAM>().ToTable("ColorRAM");
			_modelBuilder.Entity<ColorVideoCard>().ToTable("ColorVideoCard");
			_modelBuilder.Entity<PcConfigurationHDD>().ToTable("PcConfigurationHDD");
			_modelBuilder.Entity<PcConfigurationSSD>().ToTable("PcConfigurationSSD");
			_modelBuilder.Entity<PcConfigurationRAM>().ToTable("PcConfigurationRAM");
		}
		private void SetUniqueness()
		{
			_modelBuilder.Entity<FriendshipStatus>()
				.HasIndex(fs => fs.Status)
				.IsUnique();

			_modelBuilder.Entity<Color>()
				.HasIndex(c => c.Name)
				.IsUnique();

			_modelBuilder.Entity<User>()
				.HasIndex(u => u.Name)
				.IsUnique();
			_modelBuilder.Entity<User>()
				.HasIndex(u => u.Email)
				.IsUnique();
			_modelBuilder.Entity<User>()
				.HasIndex(u => u.PcConfigurationID)
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
			SetPcConfigurationHDDRelationships();
			SetPcConfigurationRAMRelationships();
			SetPcConfigurationSSDRelationships();

			SetColorHDDRelationships();
			SetColorMotherboardRelationships();
			SetColorPowerSupplyRelationships();
			SetColorRAMRelationships();
			SetColorVideoCardRelationships();
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
				.HasForeignKey(f => f.FriendID)
				.OnDelete(DeleteBehavior.Restrict);

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
				.OnDelete(DeleteBehavior.Cascade);
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
		private void SetPcConfigurationHDDRelationships()
		{
			_modelBuilder.Entity<PcConfigurationHDD>()
				.HasOne(pcHdd => pcHdd.PcConfiguration)
				.WithMany(pc => pc.PcConfigurationHDDs)
				.HasForeignKey(pcHdd => pcHdd.PcConfigurationID)
				.OnDelete(DeleteBehavior.Cascade);

			_modelBuilder.Entity<PcConfigurationHDD>()
				.HasOne(pcHdd => pcHdd.HDD)
				.WithMany(hdd => hdd.PcConfigurationHDDs)
				.HasForeignKey(pcHdd => pcHdd.HDDID)
				.OnDelete(DeleteBehavior.Cascade);
		}
		private void SetPcConfigurationRAMRelationships()
		{
			_modelBuilder.Entity<PcConfigurationRAM>()
				.HasOne(pcRam => pcRam.PcConfiguration)
				.WithMany(pc => pc.PcConfigurationRAMs)
				.HasForeignKey(pcRam => pcRam.PcConfigurationID)
				.OnDelete(DeleteBehavior.Cascade);

			_modelBuilder.Entity<PcConfigurationRAM>()
				.HasOne(pcRam => pcRam.RAM)
				.WithMany(ram => ram.PcConfigurationRAMs)
				.HasForeignKey(pcRam => pcRam.RAMID)
				.OnDelete(DeleteBehavior.Cascade);
		}
		private void SetPcConfigurationSSDRelationships()
		{
			_modelBuilder.Entity<PcConfigurationSSD>()
				.HasOne(pcSsd => pcSsd.PcConfiguration)
				.WithMany(pc => pc.PcConfigurationSSDs)
				.HasForeignKey(pcSsd => pcSsd.PcConfigurationID)
				.OnDelete(DeleteBehavior.Cascade);

			_modelBuilder.Entity<PcConfigurationSSD>()
				.HasOne(pcSsd => pcSsd.SSD)
				.WithMany(ssd => ssd.PcConfigurationSSDs)
				.HasForeignKey(pcSsd => pcSsd.SSDID)
				.OnDelete(DeleteBehavior.Cascade);
		}

		private void SetColorHDDRelationships()
		{
			_modelBuilder.Entity<ColorHDD>()
				.HasOne(colorHdd => colorHdd.Color)
				.WithMany(c => c.ColorHDDs)
				.HasForeignKey(colorHdd => colorHdd.ColorID)
				.OnDelete(DeleteBehavior.Cascade);

			_modelBuilder.Entity<ColorHDD>()
				.HasOne(colorHdd => colorHdd.HDD)
				.WithMany(hdd => hdd.ColorHDDs)
				.HasForeignKey(colorHdd => colorHdd.HDDID)
				.OnDelete(DeleteBehavior.Cascade);
		}
		private void SetColorMotherboardRelationships()
		{
			_modelBuilder.Entity<ColorMotherboard>()
				.HasOne(colorM => colorM.Color)
				.WithMany(c => c.ColorMotherboards)
				.HasForeignKey(colorM => colorM.ColorID)
				.OnDelete(DeleteBehavior.Cascade);

			_modelBuilder.Entity<ColorMotherboard>()
				.HasOne(colorM => colorM.Motherboard)
				.WithMany(m => m.ColorMotherboards)
				.HasForeignKey(colorM => colorM.MotherboardID)
				.OnDelete(DeleteBehavior.Cascade);
		}
		private void SetColorPowerSupplyRelationships()
		{
			_modelBuilder.Entity<ColorPowerSupply>()
				.HasOne(colorPS => colorPS.Color)
				.WithMany(c => c.ColorPowerSupplies)
				.HasForeignKey(colorPS => colorPS.ColorID)
				.OnDelete(DeleteBehavior.Cascade);

			_modelBuilder.Entity<ColorPowerSupply>()
				.HasOne(colorPS => colorPS.PowerSupply)
				.WithMany(ps => ps.ColorPowerSupplies)
				.HasForeignKey(colorPS => colorPS.PowerSupplyID)
				.OnDelete(DeleteBehavior.Cascade);
		}
		private void SetColorRAMRelationships()
		{
			_modelBuilder.Entity<ColorRAM>()
				.HasOne(colorRam => colorRam.Color)
				.WithMany(c => c.ColorRAMs)
				.HasForeignKey(colorRam => colorRam.ColorID)
				.OnDelete(DeleteBehavior.Cascade);

			_modelBuilder.Entity<ColorRAM>()
				.HasOne(colorRam => colorRam.RAM)
				.WithMany(ram => ram.ColorRAMs)
				.HasForeignKey(colorRam => colorRam.RAMID)
				.OnDelete(DeleteBehavior.Cascade);
		}
		private void SetColorVideoCardRelationships()
		{
			_modelBuilder.Entity<ColorVideoCard>()
				.HasOne(colorVC => colorVC.Color)
				.WithMany(c => c.ColorVideoCards)
				.HasForeignKey(colorVC => colorVC.ColorID)
				.OnDelete(DeleteBehavior.Cascade);

			_modelBuilder.Entity<ColorVideoCard>()
				.HasOne(colorVC => colorVC.VideoCard)
				.WithMany(vc => vc.ColorVideoCards)
				.HasForeignKey(colorVC => colorVC.VideoCardID)
				.OnDelete(DeleteBehavior.Cascade);
		}
		#endregion
	}
}