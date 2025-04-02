using System;
using Application.Features.Contacts.Commands.CreateContact;
using Application.Features.Contacts.Commands.DeleteContact;
using Application.Features.Contacts.Commands.AddContactInformation;
using Application.Features.Contacts.Commands.DeleteContactInformation;
using Application.Features.Contacts.Queries.GetContactDetails;
using Domain.Entities;
using Xunit;

namespace Phonebook.Tests.Application.Validators
{
    public class ContactValidatorsTests
    {
        #region CreateContactValidator Tests
        
        [Fact]
        public void CreateContactValidator_ShouldPass_WhenValidData()
        {
            // Arrange
            var validator = new CreateContactValidator();
            var command = new CreateContactCommand
            {
                Firstname = "Umut",
                Lastname = "Eskitoğlu",
                Company = "Test Company"
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void CreateContactValidator_ShouldFail_WhenFirstnameIsEmpty()
        {
            // Arrange
            var validator = new CreateContactValidator();
            var command = new CreateContactCommand
            {
                Firstname = "",
                Lastname = "Eskitoğlu",
                Company = "Test Company"
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Firstname");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("required"));
        }

        [Fact]
        public void CreateContactValidator_ShouldFail_WhenFirstnameIsTooLong()
        {
            // Arrange
            var validator = new CreateContactValidator();
            var command = new CreateContactCommand
            {
                Firstname = new string('A', 101), // 101 characters
                Lastname = "Eskitoğlu",
                Company = "Test Company"
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Firstname");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("exceed 100"));
        }

        [Fact]
        public void CreateContactValidator_ShouldFail_WhenLastnameIsEmpty()
        {
            // Arrange
            var validator = new CreateContactValidator();
            var command = new CreateContactCommand
            {
                Firstname = "Umut",
                Lastname = "",
                Company = "Test Company"
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Lastname");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("required"));
        }

        [Fact]
        public void CreateContactValidator_ShouldFail_WhenLastnameIsTooLong()
        {
            // Arrange
            var validator = new CreateContactValidator();
            var command = new CreateContactCommand
            {
                Firstname = "Umut",
                Lastname = new string('A', 101), // 101 characters
                Company = "Test Company"
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Lastname");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("exceed 100"));
        }

        [Fact]
        public void CreateContactValidator_ShouldFail_WhenCompanyIsTooLong()
        {
            // Arrange
            var validator = new CreateContactValidator();
            var command = new CreateContactCommand
            {
                Firstname = "Umut",
                Lastname = "Eskitoğlu",
                Company = new string('A', 201) // 201 characters
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Company");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("exceed 200"));
        }
        
        #endregion

        #region DeleteContactValidator Tests
        
        [Fact]
        public void DeleteContactValidator_ShouldPass_WhenValidId()
        {
            // Arrange
            var validator = new DeleteContactValidator();
            var command = new DeleteContactCommand
            {
                ContactId = Guid.NewGuid()
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void DeleteContactValidator_ShouldFail_WhenIdIsEmpty()
        {
            // Arrange
            var validator = new DeleteContactValidator();
            var command = new DeleteContactCommand
            {
                ContactId = Guid.Empty
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "ContactId");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("required"));
        }
        
        #endregion

        #region DeleteContactInformationValidator Tests
        
        [Fact]
        public void DeleteContactInformationValidator_ShouldPass_WhenValidId()
        {
            // Arrange
            var validator = new DeleteContactInformationValidator();
            var command = new DeleteContactInformationCommand
            {
                ContactInformationId = Guid.NewGuid()
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void DeleteContactInformationValidator_ShouldFail_WhenIdIsEmpty()
        {
            // Arrange
            var validator = new DeleteContactInformationValidator();
            var command = new DeleteContactInformationCommand
            {
                ContactInformationId = Guid.Empty
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "ContactInformationId");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("required"));
        }
        
        #endregion

        #region GetContactDetailsValidator Tests
        
        [Fact]
        public void GetContactDetailsValidator_ShouldPass_WhenValidId()
        {
            // Arrange
            var validator = new GetContactDetailsValidator();
            var query = new GetContactDetailsQuery
            {
                ContactId = Guid.NewGuid()
            };

            // Act
            var result = validator.Validate(query);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void GetContactDetailsValidator_ShouldFail_WhenIdIsEmpty()
        {
            // Arrange
            var validator = new GetContactDetailsValidator();
            var query = new GetContactDetailsQuery
            {
                ContactId = Guid.Empty
            };

            // Act
            var result = validator.Validate(query);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "ContactId");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("required"));
        }
        
        #endregion
    }
} 