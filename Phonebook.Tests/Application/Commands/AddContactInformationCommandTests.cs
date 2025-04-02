using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Contacts.Commands.AddContactInformation;
using Application.Interfaces;
using Domain.Entities;
using Domain.Helpers;
using Microsoft.EntityFrameworkCore;
using Phonebook.Tests.TestHelpers;
using Xunit;

namespace Phonebook.Tests.Application.Commands
{
    public class AddContactInformationCommandTests
    {
        [Fact]
        public async Task Handle_ShouldAddContactInformation_WhenContactExists()
        {
            // Arrange - Setup in-memory database
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: $"PhonebookAddInfo_DB_{Guid.NewGuid()}")
                .Options;

            var contactId = Guid.NewGuid();
            var contact = new Contact
            {
                Id = contactId,
                Firstname = "Umut",
                Lastname = "Eskitoğlu",
                Company = "Test Company"
            };

            // Add contact to database
            using (var context = new TestDbContext(options))
            {
                await context.Contacts.AddAsync(contact);
                await context.SaveChangesAsync();
            }

            // Act - Add contact information
            Guid contactInfoId;
            using (var context = new TestDbContext(options))
            {
                var handler = new AddContactInformationCommandHandler(context);
                var command = new AddContactInformationCommand
                {
                    ContactId = contactId,
                    Type = InformationType.PhoneNumber,
                    InformationContent = "+905351234545"
                };

                var result = await handler.Handle(command, CancellationToken.None);
                
                // Assert - Success result
                Assert.True(result.Success);
                Assert.Equal("Contact information added successfully", result.Message);
                Assert.NotEqual(Guid.Empty, result.Data);
                
                contactInfoId = result.Data;
            }

            // Assert - Contact info was added to database
            using (var context = new TestDbContext(options))
            {
                var contactInfo = await context.ContactInformation.FindAsync(contactInfoId);
                Assert.NotNull(contactInfo);
                Assert.Equal(contactId, contactInfo.ContactId);
                Assert.Equal(InformationType.PhoneNumber, contactInfo.Type);
                Assert.Equal("+905351234545", contactInfo.InformationContent);
            }
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenContactDoesNotExist()
        {
            // Arrange - Setup in-memory database
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: $"PhonebookAddInfoNotFound_DB_{Guid.NewGuid()}")
                .Options;

            var nonExistentContactId = Guid.NewGuid();

            // Act
            using (var context = new TestDbContext(options))
            {
                var handler = new AddContactInformationCommandHandler(context);
                var command = new AddContactInformationCommand
                {
                    ContactId = nonExistentContactId,
                    Type = InformationType.Email,
                    InformationContent = "test@gmail.com"
                };

                var result = await handler.Handle(command, CancellationToken.None);
                
                // Assert
                Assert.False(result.Success);
                Assert.Equal("Contact not found", result.Message);
                Assert.Equal(Guid.Empty, result.Data);
                
                // Verify no contact info was added
                var contactInfos = await context.ContactInformation.ToListAsync();
                Assert.Empty(contactInfos);
            }
        }
    }
} 