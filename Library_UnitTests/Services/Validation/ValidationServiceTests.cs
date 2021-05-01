using Library.Services.Validation;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Library_UnitTests.Services.Validation
{
    public class ValidationServiceTests
    {
        [Theory]
        [InlineData("Name1")]
        [InlineData("Name.")]
        [InlineData("Name 1")]
        public void NameValidationSuccess(string name)
        {
            var validationService = new ValidationService();

            Assert.True(validationService.IsValidName(name));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("       ")]
        public void NameValidationFailed(string name)
        {
            var validationService = new ValidationService();

            Assert.False(validationService.IsValidName(name));
        }

        [Theory]
        [InlineData("1234567890123")]
        public void ISBNValidationSuccess(string isbn)
        {
            var validationService = new ValidationService();

            Assert.True(validationService.IsValidISBN(isbn));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ISBN")]
        [InlineData("IS2BN")]
        [InlineData("IS2BN67890123")]
        public void ISBNValidationFailed(string isbn)
        {
            var validationService = new ValidationService();

            Assert.False(validationService.IsValidISBN(isbn));
        }

        [Theory]
        [InlineData(3)]
        [InlineData(3.5)]
        public void PriceValidationSuccess(double price)
        {
            var validationService = new ValidationService();

            Assert.True(validationService.IsValidPrice(price));
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(-2.5)]
        public void PriceValidationFailed(double price)
        {
            var validationService = new ValidationService();

            Assert.False(validationService.IsValidPrice(price));
        }

        [Theory]
        [InlineData("")]
        [InlineData("234234234")]
        [InlineData("23423 4234")]
        [InlineData("          23423 4234")]
        public void IsDigitsOnlySuccess(string input)
        {
            var validationService = new ValidationService();

            Assert.True(validationService.IsDigitsOnly(input));
        }

        [Theory]
        [InlineData("a")]
        [InlineData("323234ddasd")]
        [InlineData("                     323234ddasd            ")]
        public void IsDigitsOnlyFailed(string input)
        {
            var validationService = new ValidationService();

            Assert.False(validationService.IsDigitsOnly(input));
        }
    }
}
