using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Contacts.Commands.CreateContact;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Phonebook.Tests.TestHelpers;
using Xunit;

namespace Phonebook.Tests.Application.Commands
{
    public class CreateContactCommandTests
    {
        [Fact]
        public async Task Handle_ShouldCreateContact_WhenValidRequest()
        {
            // Arrange - Setup in-memory database
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: $"PhonebookCreate_DB_{Guid.NewGuid()}")
                .Options;

            using (var context = new TestDbContext(options))
            {
                var handler = new CreateContactCommandHandler(context);
                var command = new CreateContactCommand
                {
                    Firstname = "Umut",
                    Lastname = "Eskitoğlu",
                    Company = "Test Company"
                };

                // Act
                var result = await handler.Handle(command, CancellationToken.None);

                // Assert
                Assert.True(result.Success);
                Assert.Equal("Contact created successfully", result.Message);
                Assert.NotEqual(Guid.Empty, result.Data);
            }

            // Verify that contact was created
            using (var context = new TestDbContext(options))
            {
                var contact = await context.Contacts.FirstOrDefaultAsync();
                Assert.NotNull(contact);
                Assert.Equal("Umut", contact.Firstname);
                Assert.Equal("Eskitoğlu", contact.Lastname);
                Assert.Equal("Test Company", contact.Company);
            }
        }

        [Fact]
        public async Task Handle_ShouldCreateContact_WithCorrectData()
        {
            // Arrange - Setup in-memory database
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: $"PhonebookCreateData_DB_{Guid.NewGuid()}")
                .Options;

            Guid contactId;

            // Act
            using (var context = new TestDbContext(options))
            {
                var handler = new CreateContactCommandHandler(context);
                var command = new CreateContactCommand
                {
                    Firstname = "Umut",
                    Lastname = "Eskitoğlu",
                    Company = "Test Company"
                };

                var result = await handler.Handle(command, CancellationToken.None);
                contactId = result.Data;
            }

            // Assert
            using (var context = new TestDbContext(options))
            {
                var contact = await context.Contacts.FindAsync(contactId);
                Assert.NotNull(contact);
                Assert.Equal("Umut", contact.Firstname);
                Assert.Equal("Eskitoğlu", contact.Lastname);
                Assert.Equal("Test Company", contact.Company);
            }
        }
    }
} 