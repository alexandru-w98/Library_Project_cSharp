using System;
using System.Collections.Generic;
using System.Linq;
using Library.Constants;
using Library.Models;
using Library.Repositories;
using Library.Services;
using Library.Services.Validation;
using Moq;
using Xunit;

namespace Library_UnitTests
{
    public class LibraryRepositoryTests
    {

        [Theory]
        [InlineData("ISBN", "Book1", 1)]
        public void AddNewBookSuccess(string isbn, string name, double price)
        {
            var newBook = new Book { ISBN = isbn, Name = name};

            // Mock the validators
            var mockValidator = new Mock<IValidationService>();
            mockValidator.Setup(x => x.isValidBook(newBook)).Returns(true);
            mockValidator.Setup(x => x.IsValidQuantity(1)).Returns(true);
            mockValidator.Setup(x => x.IsValidPrice(price)).Returns(true);
            var dependencyService = new DependencyService();
            dependencyService.Register<IValidationService>(mockValidator.Object);
            var availableBooks = new List<BookProductInfo>();
            var library = new LibraryRepository(availableBooks, dependencyService);

            var addResult = library.AddNewBook(newBook, price);
            Assert.True(addResult);
            var addedBook = Assert.Single(library.AvailableBooks);
            Assert.NotNull(addedBook);

            Assert.Equal(isbn, addedBook.Book.ISBN);
            Assert.Equal(name, addedBook.Book.Name);
            Assert.Equal(price, addedBook.BorrowPrice);
        }

        [Theory]
        [InlineData("ISBN", "Book1", 1)] 
        public void AddNewBookInfoValidationFailedNotAlteringTheList(string isbn, string name, double price)
        {
            var newBook = new Book { ISBN = isbn, Name = name };

            var mockValidator = new Mock<IValidationService>();
            mockValidator.Setup(x => x.isValidBook(newBook)).Returns(false);
            mockValidator.Setup(x => x.IsValidQuantity(1)).Returns(true);
            mockValidator.Setup(x => x.IsValidPrice(price)).Returns(true);
            var dependencyService = new DependencyService();
            dependencyService.Register<IValidationService>(mockValidator.Object);
            var availableBooks = new List<BookProductInfo>();
            var library = new LibraryRepository(availableBooks, dependencyService);

            var addResult = library.AddNewBook(newBook, price);

            Assert.False(addResult);
            Assert.Empty(library.AvailableBooks);
        }

        [Theory]
        [InlineData("ISBN", "Book1", 1)]
        public void AddNewBookPriceValidationFailedNotAlteringTheList(string isbn, string name, double price)
        {
            var newBook = new Book { ISBN = isbn, Name = name };

            var mockValidator = new Mock<IValidationService>();
            mockValidator.Setup(x => x.isValidBook(newBook)).Returns(true);
            mockValidator.Setup(x => x.IsValidQuantity(1)).Returns(true);
            mockValidator.Setup(x => x.IsValidPrice(price)).Returns(false);
            var dependencyService = new DependencyService();
            dependencyService.Register<IValidationService>(mockValidator.Object);
            var availableBooks = new List<BookProductInfo>();
            var library = new LibraryRepository(availableBooks, dependencyService);

            var addResult = library.AddNewBook(newBook, price);

            Assert.False(addResult);
            Assert.Empty(library.AvailableBooks);
        }

        [Theory]
        [InlineData("ISBN", "Book1", 1)]
        public void AddNewBookAllValidationsFailedNotAlteringTheList(string isbn, string name, double price)
        {
            var newBook = new Book { ISBN = isbn, Name = name };

            var mockValidator = new Mock<IValidationService>();
            mockValidator.Setup(x => x.isValidBook(newBook)).Returns(false);
            mockValidator.Setup(x => x.IsValidQuantity(1)).Returns(false);
            mockValidator.Setup(x => x.IsValidPrice(price)).Returns(false);
            var dependencyService = new DependencyService();
            dependencyService.Register<IValidationService>(mockValidator.Object);
            var availableBooks = new List<BookProductInfo>();
            var library = new LibraryRepository(availableBooks, dependencyService);

            var addResult = library.AddNewBook(newBook, price);

            Assert.False(addResult);
            Assert.Empty(library.AvailableBooks);
        }

        [Theory]
        [InlineData("ISBN", "Book1", 1)]
        public void AddNewBookQuantityValidationFailedNotAlteringTheList(string isbn, string name, double price)
        {
            var newBook = new Book { ISBN = isbn, Name = name };

            var mockValidator = new Mock<IValidationService>();
            mockValidator.Setup(x => x.isValidBook(newBook)).Returns(true);
            mockValidator.Setup(x => x.IsValidQuantity(1)).Returns(false);
            mockValidator.Setup(x => x.IsValidPrice(price)).Returns(true);
            var dependencyService = new DependencyService();
            dependencyService.Register<IValidationService>(mockValidator.Object);
            var availableBooks = new List<BookProductInfo>();
            var library = new LibraryRepository(availableBooks, dependencyService);

            var addResult = library.AddNewBook(newBook, price);

            Assert.False(addResult);
            Assert.Empty(library.AvailableBooks);
        }

        [Theory]
        [InlineData("ISBN", "Book1", 1)]
        public void AddNewBookAlreadyExisting(string isbn, string name, double price)
        {
            var newBook = new Book { ISBN = isbn, Name = name };

            // Mock the validators
            var mockValidator = new Mock<IValidationService>();
            mockValidator.Setup(x => x.isValidBook(newBook)).Returns(true);
            mockValidator.Setup(x => x.IsValidQuantity(1)).Returns(true);
            mockValidator.Setup(x => x.IsValidPrice(price)).Returns(true);
            var dependencyService = new DependencyService();
            dependencyService.Register<IValidationService>(mockValidator.Object);
            var availableBooks = new List<BookProductInfo> { new BookProductInfo { Book = newBook} };
            var library = new LibraryRepository(availableBooks, dependencyService);

            var addResult = library.AddNewBook(newBook, price);
            Assert.False(addResult);

            var addedBook = library.AvailableBooks.FirstOrDefault(item => item.Book.ISBN == isbn);
            Assert.Equal(1, addedBook.Quantity);
        }

        [Theory]
        [InlineData("ISBN")]
        public void AddQuantityBookButBookNotFound(string isbn)
        {
            var mockValidator = new Mock<IValidationService>();
            mockValidator.Setup(x => x.IsValidQuantity(1)).Returns(true);
            var dependencyService = new DependencyService();
            dependencyService.Register<IValidationService>(mockValidator.Object);
            var availableBooks = new List<BookProductInfo>();
            var library = new LibraryRepository(availableBooks, dependencyService);

            var addResult = library.AddExistingBook(isbn);
            Assert.False(addResult);
            Assert.Empty(library.AvailableBooks);
        }

        [Theory]
        [InlineData("ISBN", "Name", 5)]
        public void AddExistingBookModifiesQuantity(string isbn, string name, int quantity)
        {
            var mockValidator = new Mock<IValidationService>();
            mockValidator.Setup(x => x.IsValidQuantity(quantity)).Returns(true);
            var dependencyService = new DependencyService();
            dependencyService.Register<IValidationService>(mockValidator.Object);
            var availableBooks = new List<BookProductInfo> { new BookProductInfo { Book = new Book { ISBN = isbn, Name = name }, Quantity = 1 } };
            var library = new LibraryRepository(availableBooks, dependencyService);

            var addResult = library.AddExistingBook(isbn, quantity);
            Assert.True(addResult);
            var addedBook = Assert.Single(library.AvailableBooks);
            Assert.NotNull(addedBook);
            Assert.Equal(1 + quantity, addedBook.Quantity);
        }

        [Theory]
        [InlineData("ISBN", "Name", 5)]
        public void AddExistingBookWithQuantityInvalidNotModifyingQuantity(string isbn, string name, int quantity)
        {
            var mockValidator = new Mock<IValidationService>();
            mockValidator.Setup(x => x.IsValidQuantity(quantity)).Returns(false);
            var dependencyService = new DependencyService();
            dependencyService.Register<IValidationService>(mockValidator.Object);
            var availableBooks = new List<BookProductInfo> { new BookProductInfo { Book = new Book { ISBN = isbn, Name = name }, Quantity = 1 } };
            var library = new LibraryRepository(availableBooks, dependencyService);

            var addResult = library.AddExistingBook(isbn, quantity);
            Assert.False(addResult);
            var addedBook = Assert.Single(library.AvailableBooks);
            Assert.NotNull(addedBook);
            Assert.Equal(1, addedBook.Quantity);
        }

        [Theory]
        [InlineData("ISBN", "23234234")]
        public void BorrowInexistingBook(string isbn, string cnp)
        {
            var mockValidator = new Mock<IValidationService>();
            mockValidator.Setup(x => x.IsValidCNP(cnp)).Returns(true);
            var dependencyService = new DependencyService();
            dependencyService.Register<IValidationService>(mockValidator.Object);
            var availableBooks = new List<BookProductInfo>();
            var library = new LibraryRepository(availableBooks, dependencyService);

            var borrowResult = library.Borrow(isbn, cnp);
            Assert.False(borrowResult);
            Assert.Empty(library.BorrowedBooks);
        }

        [Theory]
        [InlineData("ISBN", "23234234")]
        public void BorrowBookButAvailableQuantityZero(string isbn, string cnp)
        {
            var mockValidator = new Mock<IValidationService>();
            mockValidator.Setup(x => x.IsValidCNP(cnp)).Returns(true);
            var dependencyService = new DependencyService();
            dependencyService.Register<IValidationService>(mockValidator.Object);
            var availableBooks = new List<BookProductInfo> { new BookProductInfo { Book = new Book { ISBN = isbn }, Quantity = 0 } };
            var library = new LibraryRepository(availableBooks, dependencyService);

            var borrowResult = library.Borrow(isbn, cnp);
            Assert.False(borrowResult);
            Assert.Empty(library.BorrowedBooks);
        }

        [Theory]
        [InlineData("ISBN", "23234234")]
        public void BorrowBookCnpInvalid(string isbn, string cnp)
        {
            var mockValidator = new Mock<IValidationService>();
            mockValidator.Setup(x => x.IsValidCNP(cnp)).Returns(false);
            var dependencyService = new DependencyService();
            dependencyService.Register<IValidationService>(mockValidator.Object);
            var availableBooks = new List<BookProductInfo> { new BookProductInfo { Book = new Book { ISBN = isbn }, Quantity = 1 } };
            var library = new LibraryRepository(availableBooks, dependencyService);

            var borrowResult = library.Borrow(isbn, cnp);
            Assert.False(borrowResult);
            Assert.Empty(library.BorrowedBooks);
        }

        [Theory]
        [InlineData("ISBN", "23234234")]
        public void BorrowBookModifiesLendList(string isbn, string cnp)
        {
            var mockValidator = new Mock<IValidationService>();
            mockValidator.Setup(x => x.IsValidCNP(cnp)).Returns(true);
            var dependencyService = new DependencyService();
            dependencyService.Register<IValidationService>(mockValidator.Object);
            var availableBooks = new List<BookProductInfo> { new BookProductInfo { Book = new Book { ISBN = isbn }, Quantity = 1 } };
            var library = new LibraryRepository(availableBooks, dependencyService);

            var borrowResult = library.Borrow(isbn, cnp);
            Assert.True(borrowResult);
            var borrowedBook = Assert.Single(library.BorrowedBooks);
            Assert.NotNull(borrowedBook);

            Assert.Equal(isbn, borrowedBook.ISBN);
            Assert.False(borrowedBook.HasBeenReturned);
            // timestamps
        }

        [Theory]
        [InlineData("ISBN", "23234234")]
        public void BorrowBookModifiesAvailableQuantity(string isbn, string cnp)
        {
            var mockValidator = new Mock<IValidationService>();
            mockValidator.Setup(x => x.IsValidCNP(cnp)).Returns(true);
            var dependencyService = new DependencyService();
            dependencyService.Register<IValidationService>(mockValidator.Object);
            var availableBooks = new List<BookProductInfo> { new BookProductInfo { Book = new Book { ISBN = isbn }, Quantity = 1 } };
            var library = new LibraryRepository(availableBooks, dependencyService);

            var borrowResult = library.Borrow(isbn, cnp);
            Assert.True(borrowResult);
            var availableQuantity = Assert.Single(library.AvailableBooks).Quantity;
            Assert.Equal(0, availableQuantity);
        }

        [Theory]
        [InlineData("ISBN", "23234234")]
        public void ReturnBookInvalidISBN(string isbn, string cnp)
        {
            var availableBooks = new List<BookProductInfo>();
            var lendBooks = new List<Borrowing> { new Borrowing { CNP = cnp, ISBN = "NotCorrect" } };
            var library = new LibraryRepository(availableBooks, lendBooks, new DependencyService());

            var returnResult = library.ReturnBook(isbn, cnp);
            Assert.False(returnResult);
            Assert.Empty(library.AvailableBooks);
        }

        [Theory]
        [InlineData("ISBN", "23234234")]
        public void ReturnBookInvalidCNP(string isbn, string cnp)
        {
            var availableBooks = new List<BookProductInfo>();
            var lendBooks = new List<Borrowing> { new Borrowing { CNP = "11", ISBN = isbn } };
            var library = new LibraryRepository(availableBooks, lendBooks, new DependencyService());

            var returnResult = library.ReturnBook(isbn, cnp);
            Assert.False(returnResult);
            Assert.Empty(library.AvailableBooks);
        }

        [Theory]
        [InlineData("ISBN", "23234234")]
        public void ReturnBookAndBookIsInAvailableListUpdatesQuantity(string isbn, string cnp)
        {
            var mockValidator = new Mock<IValidationService>();
            mockValidator.Setup(x => x.IsValidQuantity(1)).Returns(true);
            var dependencyService = new DependencyService();
            dependencyService.Register<IValidationService>(mockValidator.Object);
            var availableBooks = new List<BookProductInfo> { new BookProductInfo { Book = new Book { ISBN = isbn} } };
            var lendBooks = new List<Borrowing> { new Borrowing { CNP = cnp, ISBN = isbn } };
            var library = new LibraryRepository(availableBooks, lendBooks, dependencyService);

            var returnResult = library.ReturnBook(isbn, cnp);
            Assert.True(returnResult);
            Assert.Equal(2, Assert.Single(library.AvailableBooks).Quantity);
        }

        [Theory]
        [InlineData("ISBN", "23234234")]
        public void ReturnBookSuccessUpdatesReturnedStatus(string isbn, string cnp)
        {
            var mockValidator = new Mock<IValidationService>();
            mockValidator.Setup(x => x.IsValidQuantity(1)).Returns(true);
            var dependencyService = new DependencyService();
            dependencyService.Register<IValidationService>(mockValidator.Object);
            var availableBooks = new List<BookProductInfo> { new BookProductInfo { Book = new Book { ISBN = isbn } } };
            var lendBooks = new List<Borrowing> { new Borrowing { CNP = cnp, ISBN = isbn } };
            var library = new LibraryRepository(availableBooks, lendBooks, dependencyService);

            var returnResult = library.ReturnBook(isbn, cnp);
            Assert.True(returnResult);
            Assert.True(Assert.Single(library.BorrowedBooks).HasBeenReturned);
        }

        [Theory]
        [InlineData("ISBN", "23234234")]
        public void ReturnBookFailedIsNotUpdatingReturnedStatus(string isbn, string cnp)
        {
   
            var availableBooks = new List<BookProductInfo>();
            var lendBooks = new List<Borrowing> { new Borrowing { CNP = cnp, ISBN = "NotCorrent" } };
            var library = new LibraryRepository(availableBooks, lendBooks, new DependencyService());

            var returnResult = library.ReturnBook(isbn, cnp);
            Assert.False(returnResult);
            Assert.False(Assert.Single(library.BorrowedBooks).HasBeenReturned);
        }

        [Theory]
        [InlineData("ISBN", "23234234")]
        public void ReturnBookFailedIsNotUpdatingReturnedStatu2s(string isbn, string cnp)
        {

            var availableBooks = new List<BookProductInfo>();
            var lendBooks = new List<Borrowing> { new Borrowing { CNP = cnp, ISBN = "NotCorrent" } };
            var library = new LibraryRepository(availableBooks, lendBooks, new DependencyService());

            var returnResult = library.ReturnBook(isbn, cnp);
            Assert.False(returnResult);
            Assert.False(Assert.Single(library.BorrowedBooks).HasBeenReturned);
        }

        [Theory]
        [InlineData("ISBN", "23234234", 15)]
        public void PenaltyAppliedAfterBorrowTimeExpires(string isbn, string cnp, double price)
        {
            var availableBooks = new List<BookProductInfo>();
            var lendBooks = new List<Borrowing> {
                new Borrowing {
                    CNP = cnp,
                    ISBN = isbn,
                    BorrowingEndDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
                    BorrowingPrice = price
                } 
            };
            var library = new LibraryRepository(availableBooks, lendBooks, new DependencyService());

            library.CalculateBorrowPenalty();
            Assert.Equal(AppConstants.LOAN_PENALTY * price + price, Assert.Single(library.BorrowedBooks).BorrowingPrice);
        }

        [Theory]
        [InlineData("ISBN", "23234234", 15)]
        public void PenaltyNotAppliedIfBookWasReturned(string isbn, string cnp, double price)
        {
            var availableBooks = new List<BookProductInfo>();
            var lendBooks = new List<Borrowing> {
                new Borrowing {
                    CNP = cnp,
                    ISBN = isbn,
                    BorrowingEndDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
                    BorrowingPrice = price,
                    HasBeenReturned = true
                }
            };
            var library = new LibraryRepository(availableBooks, lendBooks, new DependencyService());

            library.CalculateBorrowPenalty();
            Assert.Equal(price, Assert.Single(library.BorrowedBooks).BorrowingPrice);
        }
    }
}
