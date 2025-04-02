using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Phonebook.Tests.TestHelpers
{
    // Test implementation of IPhonebookDbContext
    public class TestDbContext : DbContext, IPhonebookDbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        { }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContactInformation> ContactInformation { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<ReportDetail> ReportDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contact>().HasKey(c => c.Id);
            modelBuilder.Entity<ContactInformation>().HasKey(ci => ci.Id);

            modelBuilder.Entity<ContactInformation>()
                .HasOne<Contact>()
                .WithMany()
                .HasForeignKey(ci => ci.ContactId);

            modelBuilder.Entity<Report>().HasKey(r => r.Id);
            modelBuilder.Entity<ReportDetail>().HasKey(rd => rd.Id);

            modelBuilder.Entity<Report>()
                .HasMany(r => r.Details)
                .WithOne()
                .HasForeignKey(rd => rd.ReportId);
        }
    }
}