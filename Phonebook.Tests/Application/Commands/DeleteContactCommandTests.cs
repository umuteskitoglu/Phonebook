using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Contacts.Commands.DeleteContact;
using Application.Interfaces;
using Domain.Entities;
using Domain.Helpers;
using Microsoft.EntityFrameworkCore;
using Phonebook.Tests.TestHelpers;
using Xunit;

namespace Phonebook.Tests.Application.Commands
{
    public class DeleteContactCommandTests
    {
        [Fact]
        public async Task Handle_ShouldDeleteContact_WhenContactExists()
        {
            // Arrange - Setup in-memory database
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: $"PhonebookDelete_DB_{Guid.NewGuid()}")
                .Options;

            var contactId = Guid.NewGuid();
            var contact = new Contact
            {
                Id = contactId,
                Firstname = "Umut",
                Lastname = "Eskitoğlu",
                Company = "Test Company"
            };

            // Add test data
            using (var context = new TestDbContext(options))
            {
                await context.Contacts.AddAsync(contact);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new TestDbContext(options))
            {
                var handler = new DeleteContactCommandHandler(context);
                var command = new DeleteContactCommand
                {
                    ContactId = contactId
                };

                var result = await handler.Handle(command, CancellationToken.None);
                
                // Assert
                Assert.True(result.Success);
                Assert.Equal("Contact deleted successfully", result.Message);
            }

            // Verify that contact was deleted
            using (var context = new TestDbContext(options))
            {
                var contacts = await context.Contacts.ToListAsync();
                Assert.Empty(contacts);
            }
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenContactDoesNotExist()
        {
            // Arrange - Setup in-memory database
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: $"PhonebookDeleteNotFound_DB_{Guid.NewGuid()}")
                .Options;

            var nonExistentContactId = Guid.NewGuid();

            // Act
            using (var context = new TestDbContext(options))
            {
                var handler = new DeleteContactCommandHandler(context);
                var command = new DeleteContactCommand
                {
                    ContactId = nonExistentContactId
                };

                var result = await handler.Handle(command, CancellationToken.None);
                
                // Assert
                Assert.False(result.Success);
                Assert.Equal("Contact not found", result.Message);
            }
        }
    }
} 