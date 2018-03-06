namespace EzTrack.Data
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class EzTrackContext : DbContext
    {
        public EzTrackContext()
            : base("name=EzTrackContext")
        {
        }

        public virtual DbSet<Asset> Assets { get; set; }
        public virtual DbSet<AssetAllocation> AssetAllocations { get; set; }
        public virtual DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Asset>()
                .Property(e => e.BarcodeAlias)
                .IsUnicode(false);

            modelBuilder.Entity<Asset>()
                .HasMany(e => e.AssetAllocations)
                .WithRequired(e => e.Asset)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Order>()
                .Property(e => e.OrderName)
                .IsUnicode(false);

            modelBuilder.Entity<Order>()
                .Property(e => e.Status)
                .IsUnicode(false);

            modelBuilder.Entity<Order>()
                .HasMany(e => e.AssetAllocations)
                .WithRequired(e => e.Order)
                .WillCascadeOnDelete(false);
        }
    }
}
