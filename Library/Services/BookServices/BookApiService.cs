using Library.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Services.BookServices
{
    public class BookApiService : IBookApiService
    {
        public IEnumerable<BookProductInfo> GetAllBooks()
        {
            return new List<BookProductInfo>
            {
                new BookProductInfo
                {
                    Book = new Book
                    {
                        Name = "Book1",
                        ISBN = "1234567890123"
                    },
                    Quantity = 3,
                    BorrowPrice = 10
                },
                new BookProductInfo
                {
                    Book = new Book
                    {
                        Name = "Book2",
                        ISBN = "1234567390123"
                    },
                    Quantity = 2,
                    BorrowPrice = 20
                },
                new BookProductInfo
                {
                    Book = new Book
                    {
                        Name = "Book3",
                        ISBN = "1234565890123"
                    },
                    Quantity = 1,
                    BorrowPrice = 15
                }
            };
        }
    }
}
