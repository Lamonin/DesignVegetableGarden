using DVG_MITIPS.Types;
using Microsoft.EntityFrameworkCore;

namespace DVG_MITIPS
{
    class DvgContext : DbContext
    {
        public DbSet<Vegetable> Vegetables { get; set; } = null!;
        public DbSet<Requirement> Requirements { get; set; } = null!;
        public DbSet<VegetableRequirement> VegetableRequirements { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=dvg.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vegetable>()
                .HasMany(v => v.Requirements)
                .WithMany(r => r.Vegetables)
                .UsingEntity<VegetableRequirement>();
            modelBuilder.Entity<Vegetable>().Navigation(v => v.Requirements).AutoInclude();
            modelBuilder.Entity<Vegetable>().Navigation(v => v.VegetableRequirements).AutoInclude();
            modelBuilder.Entity<Vegetable>().HasMany(v => v.CompatibleVegetables).WithMany();
        }
    }
}
