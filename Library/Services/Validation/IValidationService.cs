using Library.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Services.Validation
{
    public interface IValidationService
    {
        bool isValidBook(Book book);
        bool IsValidName(string name);
        bool IsValidISBN(string isbn);
        bool IsValidPrice(double price);
        bool IsValidQuantity(int quantity);
        bool IsDigitsOnly(string input);
        bool IsValidCNP(string cnp);
    }
}
