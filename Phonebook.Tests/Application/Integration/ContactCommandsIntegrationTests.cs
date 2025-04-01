using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Contacts.Commands.CreateContact;
using Application.Features.Contacts.Commands.DeleteContact;
using Application.Features.Contacts.Queries.GetContacts;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Phonebook.Tests.Application.Integration
{
    public class ContactCommandsIntegrationTests : IDisposable
    {
        private readonly TestDbContext _dbContext;

        public ContactCommandsIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: $"PhonebookTest_{Guid.NewGuid()}")
                .Options;

            _dbContext = new TestDbContext(options);
        }

        [Fact]
        public async Task CreateContact_ThenGetContacts_ShouldReturnCreatedContact()
        {
            // Arrange
            var createHandler = new CreateContactCommandHandler(_dbContext);
            var getContactsHandler = new GetContactsQueryHandler(_dbContext);

            var createCommand = new CreateContactCommand
            {
                Firstname = "Umut",
                Lastname = "Eskitoğlu",
                Company = "Test Company"
            };

            // Act - Create Contact
            var createResult = await createHandler.Handle(createCommand, CancellationToken.None);
            
            // Assert - Contact creation successful
            Assert.True(createResult.Success);
            Assert.NotEqual(Guid.Empty, createResult.Data);
            
            // Act - Get Contacts
            var getContactsQuery = new GetContactsQuery();
            var contacts = await getContactsHandler.Handle(getContactsQuery, CancellationToken.None);
            
            // Assert - Contact exists in the list
            Assert.Single(contacts);
            Assert.Equal("Umut", contacts[0].Firstname);
            Assert.Equal("Eskitoğlu", contacts[0].Lastname);
            Assert.Equal("Test Company", contacts[0].Company);
            Assert.Equal(createResult.Data, contacts[0].Id);
        }

        [Fact]
        public async Task CreateContact_ThenDeleteContact_ShouldRemoveContactFromDb()
        {
            // Arrange
            var createHandler = new CreateContactCommandHandler(_dbContext);
            var deleteHandler = new DeleteContactCommandHandler(_dbContext);
            var getContactsHandler = new GetContactsQueryHandler(_dbContext);

            var createCommand = new CreateContactCommand
            {
                Firstname = "Umut",
                Lastname = "Eskitoğlu",
                Company = "Another Company"
            };

            // Act - Create Contact
            var createResult = await createHandler.Handle(createCommand, CancellationToken.None);
            
            // Assert - Contact creation successful
            Assert.True(createResult.Success);
            
            // Act - Delete Contact
            var deleteCommand = new DeleteContactCommand
            {
                ContactId = createResult.Data
            };
            var deleteResult = await deleteHandler.Handle(deleteCommand, CancellationToken.None);
            
            // Assert - Delete successful
            Assert.True(deleteResult.Success);
            
            // Act - Get Contacts
            var getContactsQuery = new GetContactsQuery();
            var contacts = await getContactsHandler.Handle(getContactsQuery, CancellationToken.None);
            
            // Assert - Contact list is empty
            Assert.Empty(contacts);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }

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