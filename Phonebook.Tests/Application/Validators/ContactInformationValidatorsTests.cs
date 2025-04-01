using System;
using Application.Features.Contacts.Commands.AddContactInformation;
using Domain.Entities;
using Xunit;

namespace Phonebook.Tests.Application.Validators
{
    public class ContactInformationValidatorsTests
    {
        #region AddContactInformationValidator Tests
        
        [Fact]
        public void AddContactInformationValidator_ShouldPass_WhenValidData()
        {
            // Arrange
            var validator = new AddContactInformationValidator();
            var command = new AddContactInformationCommand
            {
                ContactId = Guid.NewGuid(),
                Type = InformationType.PhoneNumber,
                InformationContent = "+905551234567"
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void AddContactInformationValidator_ShouldFail_WhenContactIdIsEmpty()
        {
            // Arrange
            var validator = new AddContactInformationValidator();
            var command = new AddContactInformationCommand
            {
                ContactId = Guid.Empty,
                Type = InformationType.PhoneNumber,
                InformationContent = "+905551234567"
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "ContactId");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("required"));
        }

        [Fact]
        public void AddContactInformationValidator_ShouldFail_WhenTypeIsInvalid()
        {
            // Arrange
            var validator = new AddContactInformationValidator();
            var command = new AddContactInformationCommand
            {
                ContactId = Guid.NewGuid(),
                Type = (InformationType)999, // Invalid enum value
                InformationContent = "Some content"
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Type");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Invalid information type"));
        }

        [Fact]
        public void AddContactInformationValidator_ShouldFail_WhenInformationContentIsEmpty()
        {
            // Arrange
            var validator = new AddContactInformationValidator();
            var command = new AddContactInformationCommand
            {
                ContactId = Guid.NewGuid(),
                Type = InformationType.Email,
                InformationContent = ""
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "InformationContent");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("required"));
        }

        [Fact]
        public void AddContactInformationValidator_ShouldFail_WhenInformationContentIsTooLong()
        {
            // Arrange
            var validator = new AddContactInformationValidator();
            var command = new AddContactInformationCommand
            {
                ContactId = Guid.NewGuid(),
                Type = InformationType.Location,
                InformationContent = new string('A', 501) // 501 characters
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "InformationContent");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("exceed 500"));
        }

        [Fact]
        public void AddContactInformationValidator_ShouldFail_WhenPhoneNumberFormatIsInvalid()
        {
            // Arrange
            var validator = new AddContactInformationValidator();
            var command = new AddContactInformationCommand
            {
                ContactId = Guid.NewGuid(),
                Type = InformationType.PhoneNumber,
                InformationContent = "invalid-phone$"
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "InformationContent");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Invalid phone number format"));
        }

        [Fact]
        public void AddContactInformationValidator_ShouldPass_WhenPhoneNumberFormatIsValid()
        {
            // Test various valid phone number formats
            var validator = new AddContactInformationValidator();
            string[] validPhoneNumbers = {
                "+905551234567",
                "+90 555 123 45 67",
                "05551234567",
                "555-123-4567",
                "(555) 123-4567"
            };

            foreach (var phoneNumber in validPhoneNumbers)
            {
                // Arrange
                var command = new AddContactInformationCommand
                {
                    ContactId = Guid.NewGuid(),
                    Type = InformationType.PhoneNumber,
                    InformationContent = phoneNumber
                };

                // Act
                var result = validator.Validate(command);

                // Assert
                Assert.True(result.IsValid, $"Phone number '{phoneNumber}' should be valid");
            }
        }

        [Fact]
        public void AddContactInformationValidator_ShouldFail_WhenEmailFormatIsInvalid()
        {
            // Arrange
            var validator = new AddContactInformationValidator();
            var command = new AddContactInformationCommand
            {
                ContactId = Guid.NewGuid(),
                Type = InformationType.Email,
                InformationContent = "invalid-email"
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "InformationContent");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Invalid email address format"));
        }

        [Fact]
        public void AddContactInformationValidator_ShouldPass_WhenEmailFormatIsValid()
        {
            // Test various valid email formats
            var validator = new AddContactInformationValidator();
            string[] validEmails = {
                "test@example.com",
                "first.last@example.co.uk",
                "name+tag@example.org",
                "user.name@subdomain.example.com"
            };

            foreach (var email in validEmails)
            {
                // Arrange
                var command = new AddContactInformationCommand
                {
                    ContactId = Guid.NewGuid(),
                    Type = InformationType.Email,
                    InformationContent = email
                };

                // Act
                var result = validator.Validate(command);

                // Assert
                Assert.True(result.IsValid, $"Email '{email}' should be valid");
            }
        }

        #endregion
    }
} 