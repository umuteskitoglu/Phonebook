using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Contacts.Queries.GetContacts;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Phonebook.Tests.TestHelpers;
using Xunit;

namespace Phonebook.Tests.Application.Queries
{
    public class GetContactsQueryTests
    {
        [Fact]
        public async Task Handle_ShouldReturnAllContacts()
        {
            // Arrange - Setup in-memory database
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: $"PhonebookGetContacts_DB_{Guid.NewGuid()}")
                .Options;

            // Add test data
            using (var context = new TestDbContext(options))
            {
                await context.Contacts.AddRangeAsync(
                    new Contact
                    {
                        Id = Guid.NewGuid(),
                        Firstname = "Umut",
                        Lastname = "Eskitoğlu",
                        Company = "Test Company"
                    },
                    new Contact
                    {
                        Id = Guid.NewGuid(),
                        Firstname = "Hamza",
                        Lastname = "Eskitoğlu",
                        Company = "Test Company"
                    }
                );
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new TestDbContext(options))
            {
                var handler = new GetContactsQueryHandler(context);
                var query = new GetContactsQuery();

                var result = await handler.Handle(query, CancellationToken.None);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count);
                
                var contacts = await context.Contacts.ToListAsync();
                
                // Find matching contacts by name - order may not be guaranteed
                var umutContact = contacts.First(c => c.Firstname == "Umut");
                var hamzaContact = contacts.First(c => c.Firstname == "Hamza");
                
                var umutResult = result.First(r => r.Firstname == "Umut");
                var hamzaResult = result.First(r => r.Firstname == "Hamza");
                
                // Verify John's contact info
                Assert.Equal(umutContact.Firstname, umutResult.Firstname);
                Assert.Equal(umutContact.Lastname, umutResult.Lastname);
                Assert.Equal(umutContact.Company, umutResult.Company);
                
                // Verify Jane's contact info
                Assert.Equal(hamzaContact.Firstname, hamzaResult.Firstname);
                Assert.Equal(hamzaContact.Lastname, hamzaResult.Lastname);
                Assert.Equal(hamzaContact.Company, hamzaResult.Company);
            }
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoContacts()
        {
            // Arrange - Setup empty in-memory database
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: $"PhonebookEmptyContacts_DB_{Guid.NewGuid()}")
                .Options;

            // Act
            using (var context = new TestDbContext(options))
            {
                var handler = new GetContactsQueryHandler(context);
                var query = new GetContactsQuery();

                var result = await handler.Handle(query, CancellationToken.None);

                // Assert
                Assert.NotNull(result);
                Assert.Empty(result);
            }
        }
    }
} 