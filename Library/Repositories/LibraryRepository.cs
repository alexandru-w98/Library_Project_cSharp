using Library.Constants;
using Library.Models;
using Library.Services;
using Library.Services.BookServices;
using Library.Services.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.Repositories
{
    public class LibraryRepository
    {
        private readonly IDependencyService _dependencyService;
        public List<BookProductInfo> AvailableBooks { get; private set; }
        public List<Borrowing> BorrowedBooks { get; private set; }

        public LibraryRepository(List<BookProductInfo> availableBooks, IDependencyService dependencyService)
        {
            _dependencyService = dependencyService;
            AvailableBooks = availableBooks;
            BorrowedBooks = new List<Borrowing>();
        }
        public LibraryRepository(List<Borrowing> lendBooks, IDependencyService dependencyService)
        {
            _dependencyService = dependencyService;
            BorrowedBooks = lendBooks;

            GetApiBooks();
        }
        public LibraryRepository(List<BookProductInfo> books, List<Borrowing> lendBooks, IDependencyService dependencyService)
        {
            _dependencyService = dependencyService;
            AvailableBooks = books;
            BorrowedBooks = lendBooks;
        }
        public LibraryRepository(IDependencyService dependencyService)
        {
            _dependencyService = dependencyService;
            BorrowedBooks = new List<Borrowing>();

            GetApiBooks();
        }

        private void GetApiBooks()
        {
            try
            {
                AvailableBooks = new List<BookProductInfo>(_dependencyService.Get<IBookApiService>().GetAllBooks());
            } catch(Exception e)
            {
                AvailableBooks = new List<BookProductInfo>();
            }
        }

        public bool AddNewBook(Book book, double price, int quantity = 1)
        {
            var validationService = _dependencyService.Get<IValidationService>();

            if (validationService.isValidBook(book) &&
                validationService.IsValidPrice(price) &&
                validationService.IsValidQuantity(quantity))
            {
                var searchedIndex = AvailableBooks.FindIndex(item => item.Book.ISBN == book.ISBN);
                if (searchedIndex == -1)
                {
                    AvailableBooks.Add(new BookProductInfo { Book = book, BorrowPrice = price, Quantity = quantity });

                    return true;
                }
            }

            return false;
        }

        public bool AddExistingBook(string isbn, int quantity = 1)
        {
            var bookIndex = AvailableBooks.FindIndex(item => item.Book.ISBN == isbn);

            if ((bookIndex != -1) &&
                _dependencyService.Get<IValidationService>().IsValidQuantity(quantity))
            {
                AvailableBooks[bookIndex].Quantity += quantity;
                return true;
            }

            return false;
        }

        public bool Borrow(string isbn, string cnp)
        {
            var searchedBookIndex = AvailableBooks.FindIndex(item => item.Book.ISBN == isbn);

            if (searchedBookIndex != -1 &&
                AvailableBooks[searchedBookIndex].Quantity > 0 &&
                _dependencyService.Get<IValidationService>().IsValidCNP(cnp))
            {
                AvailableBooks[searchedBookIndex].Quantity--;

                var currentDate = DateTimeOffset.UtcNow;
                BorrowedBooks.Add(new Borrowing
                {
                    CNP = cnp,
                    ISBN = isbn,
                    BorrowingPrice = AvailableBooks[searchedBookIndex].BorrowPrice,
                    BorrowingStartDate = currentDate.ToUnixTimeMilliseconds().ToString(),
                    BorrowingEndDate = currentDate.AddDays(AppConstants.MAX_LOAN_DAYS).ToUnixTimeMilliseconds().ToString()
                });
                return true;
            } else
            {
                return false;
            }
        }

        public bool ReturnBook(string isbn, string cnp)
        {
            var searchedBookIndex = BorrowedBooks.FindIndex(item => item.CNP == cnp && item.ISBN == isbn);

            if (searchedBookIndex != -1)
            {
                if (AddExistingBook(isbn))
                {
                    BorrowedBooks[searchedBookIndex].HasBeenReturned = true;

                    return true;
                } else
                {
                    return false;
                }
            } else
            {
                return false;
            }
        }

        public void DisplayAvailableBooks()
        {
            foreach(var bookInfo in AvailableBooks)
            {
                Console.WriteLine("ISBN: " + bookInfo.Book.ISBN +
                    " Nume: " + bookInfo.Book.Name + 
                    " Quantity: " + bookInfo.Quantity +
                    " Borrowing Price: " + bookInfo.BorrowPrice);
            }
        }
        
        public void DisplayBorrowedBooks()
        {
            string result = "";
            foreach (var borrowing in BorrowedBooks)
            {
                result += "CNP: " + borrowing.CNP +
                    " Borrowing Start: " + borrowing.BorrowingStartDate +
                    " Borrowing End: " + borrowing.BorrowingEndDate +
                    " Borrowing Price: " + borrowing.BorrowingPrice;

                result += borrowing.HasBeenReturned ? " Status: Returned" : " Status: Not Returned";
            }

            Console.WriteLine(result);
        }

        public int GetAvailableQuantity(string isbn)
        {
            var searchedBook = AvailableBooks.FirstOrDefault(bookInfo => bookInfo.Book.ISBN == isbn);

            if (searchedBook != null)
            {
                return searchedBook.Quantity;
            } else
            {
                return -1;
            }
        }

        public void CalculateBorrowPenalty()
        {
            var currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

            foreach (var borrowing in BorrowedBooks)
            {
                try
                {
                    if (long.Parse(borrowing.BorrowingEndDate) <= long.Parse(currentTimestamp) && !borrowing.HasBeenReturned)
                    {
                        borrowing.BorrowingPrice += AppConstants.LOAN_PENALTY * borrowing.BorrowingPrice;
                    }
                } catch (Exception e)
                {
                    //
                }
            }
        }
    }
}
