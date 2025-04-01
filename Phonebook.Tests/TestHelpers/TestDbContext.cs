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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contact>().HasKey(c => c.Id);
            modelBuilder.Entity<ContactInformation>().HasKey(ci => ci.Id);

            modelBuilder.Entity<ContactInformation>()
                .HasOne<Contact>()
                .WithMany()
                .HasForeignKey(ci => ci.ContactId);
        }
    }
} 