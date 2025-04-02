using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Contacts.Commands.DeleteContactInformation;
using Application.Interfaces;
using Domain.Entities;
using Domain.Helpers;
using Microsoft.EntityFrameworkCore;
using Phonebook.Tests.TestHelpers;
using Xunit;

namespace Phonebook.Tests.Application.Commands
{
    public class DeleteContactInformationCommandTests
    {
        [Fact]
        public async Task Handle_ShouldDeleteContactInformation_WhenContactInformationExists()
        {
            // Arrange - Setup in-memory database
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: $"PhonebookDeleteInfo_DB_{Guid.NewGuid()}")
                .Options;

            var contactId = Guid.NewGuid();
            var contactInfoId = Guid.NewGuid();
            
            // Add test data
            using (var context = new TestDbContext(options))
            {
                // First add a contact
                var contact = new Contact
                {
                    Id = contactId,
                    Firstname = "Umut",
                    Lastname = "Eskitoğlu",
                    Company = "Test Company"
                };
                await context.Contacts.AddAsync(contact);
                
                // Then add contact information
                var contactInfo = new ContactInformation
                {
                    Id = contactInfoId,
                    ContactId = contactId,
                    Type = InformationType.PhoneNumber,
                    InformationContent = "+905351234545"
                };
                await context.ContactInformation.AddAsync(contactInfo);
                
                await context.SaveChangesAsync();
            }

            // Verify test data is in the database before deleting
            using (var context = new TestDbContext(options))
            {
                var contactInfo = await context.ContactInformation.FindAsync(contactInfoId);
                Assert.NotNull(contactInfo);
            }

            // Act - Delete contact information
            using (var context = new TestDbContext(options))
            {
                var handler = new DeleteContactInformationCommandHandler(context);
                var command = new DeleteContactInformationCommand
                {
                    ContactInformationId = contactInfoId
                };

                var result = await handler.Handle(command, CancellationToken.None);
                
                // Assert - Success result
                Assert.True(result.Success);
                Assert.Equal("Contact information removed successfully", result.Message);
            }

            // Verify that contact information was deleted
            using (var context = new TestDbContext(options))
            {
                var contactInfo = await context.ContactInformation.FindAsync(contactInfoId);
                Assert.Null(contactInfo);
                
                // But contact should still exist
                var contact = await context.Contacts.FindAsync(contactId);
                Assert.NotNull(contact);
            }
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenContactInformationDoesNotExist()
        {
            // Arrange - Setup in-memory database
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: $"PhonebookDeleteInfoNotFound_DB_{Guid.NewGuid()}")
                .Options;

            var nonExistentContactInfoId = Guid.NewGuid();

            // Act
            using (var context = new TestDbContext(options))
            {
                var handler = new DeleteContactInformationCommandHandler(context);
                var command = new DeleteContactInformationCommand
                {
                    ContactInformationId = nonExistentContactInfoId
                };

                var result = await handler.Handle(command, CancellationToken.None);
                
                // Assert
                Assert.False(result.Success);
                Assert.Equal("Contact information not found", result.Message);
            }
        }
    }
} 