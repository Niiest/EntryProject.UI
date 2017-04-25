using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EntryProject.UI.DataModels
{
    public partial class TestDBContext : DbContext
    {
        public virtual DbSet<GroupType> GroupTypes { get; set; }
        public virtual DbSet<GroupUser> GroupUsers { get; set; }
        public virtual DbSet<Group> Groups { get; set; }

        public TestDBContext(DbContextOptions options)
            :base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GroupType>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<GroupUser>(entity =>
            {
                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.GroupUsers)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK__GroupUser__Group__286302EC");
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.GroupType)
                    .WithMany(p => p.Groups)
                    .HasForeignKey(d => d.GroupTypeId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK__Groups__GroupTyp__25869641");
            });

            modelBuilder.Entity<Group>().ToTable("Groups");
            modelBuilder.Entity<GroupType>().ToTable("GroupTypes");
            modelBuilder.Entity<GroupUser>().ToTable("GroupUsers");
        }
    }
}