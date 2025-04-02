using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Data
{
    public class PhonebookDbContext : DbContext, IPhonebookDbContext
    {
        public PhonebookDbContext(DbContextOptions<PhonebookDbContext> options) : base(options)
        {
        }

        public DbSet<Contact> Contacts { get; set; } = null!;
        public DbSet<ContactInformation> ContactInformation { get; set; } = null!;
        public DbSet<Report> Reports { get; set; } = null!;
        public DbSet<ReportDetail> ReportDetails { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.ToTable("Contacts");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Firstname).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Lastname).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Company).IsRequired().HasMaxLength(200);
            });

            modelBuilder.Entity<ContactInformation>(entity =>
            {
                entity.ToTable("ContactInformation");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.InformationContent).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Type).IsRequired();

                entity.HasOne(e => e.Contact)
                      .WithMany(c => c.ContactInformation)
                      .HasForeignKey(e => e.ContactId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            
            modelBuilder.Entity<Report>(entity =>
            {
                entity.ToTable("Reports");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.RequestedAt).IsRequired();
                entity.Property(e => e.Status).IsRequired();
            });

            modelBuilder.Entity<ReportDetail>(entity =>
            {
                entity.ToTable("ReportDetails");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.ReportId).IsRequired();
                entity.Property(e => e.Location).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ContactCount).IsRequired();
                entity.Property(e => e.PhoneNumberCount).IsRequired();
            });
        }
    }
}
