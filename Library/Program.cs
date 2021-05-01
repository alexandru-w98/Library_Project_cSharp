using Library.Constants;
using Library.Models;
using Library.Repositories;
using Library.Services;
using Library.Services.BookServices;
using Library.Services.Validation;
using System;
using System.Timers;

namespace Library
{
    class Program
    {
        static void Main(string[] args)
        {
            // Register all services
            IDependencyService dependencyService = new DependencyService();
            dependencyService.Register<IBookApiService>(new BookApiService());
            dependencyService.Register<IValidationService>(new ValidationService());

            var library = new LibraryRepository(dependencyService);

            char option;
            string isbn, cnp, name;
            bool hasBeenBorrowed, hasBeenAdded, running = true;
            double price;
            int quantity;

            //Daily call to check for penalties
            long interval = AppConstants.ROUTINE_INTERVAL;

            Timer timeChecker = new Timer(interval);
            timeChecker.Elapsed += new ElapsedEventHandler((s, e) =>
            {
                library.CalculateBorrowPenalty();
            });
            timeChecker.Enabled = true;

            while (running)
            {
                Console.WriteLine("-------------------------------------------------");
                Console.WriteLine("******************** LIBRARY ********************");
                Console.WriteLine("-------------------------------------------------\n");
                Console.WriteLine("                Customer OPTIONS                 ");
                Console.WriteLine("1. Show all books");
                Console.WriteLine("2. Find the available quantity for a certain book");
                Console.WriteLine("3. Borrow a book");
                Console.WriteLine("4. Return a book");

                Console.WriteLine("\n                  Admin OPTIONS                 ");
                Console.WriteLine("5. Add a new book to the library");
                Console.WriteLine("6. Update the quantity for an existing book");
                Console.WriteLine("7. Show all borrowed books\n");
                Console.WriteLine("8. Exit\n");
                Console.Write("Choose your option: ");
                option = Console.ReadKey().KeyChar;

                Console.WriteLine();
                switch (option)
                {
                    case '1':
                        library.DisplayAvailableBooks();
                        break;
                    case '2':
                        Console.Write("Enter the ISBN: ");
                        isbn = Console.ReadLine();
                        quantity = library.GetAvailableQuantity(isbn);
                        if (quantity != -1)
                        {
                            Console.WriteLine("There are " + quantity + " copies available");
                        }
                        else
                        {
                            Console.WriteLine("You entered an invalid ISBN !");
                        }
                        break;
                    case '3':
                        Console.Write("Enter the ISBN: ");
                        isbn = Console.ReadLine();
                        Console.Write("Enter the CNP: ");
                        cnp = Console.ReadLine();

                        hasBeenBorrowed = library.Borrow(isbn, cnp);
                        if (hasBeenBorrowed)
                        {
                            Console.WriteLine("You borrowed the book!");
                        }
                        else
                        {
                            Console.WriteLine("You entered invalid informations!");
                        }
                        Console.WriteLine();
                        break;
                    case '4':
                        Console.Write("Enter the ISBN: ");
                        isbn = Console.ReadLine();
                        Console.Write("Enter the CNP: ");
                        cnp = Console.ReadLine();

                        hasBeenBorrowed = library.ReturnBook(isbn, cnp);
                        if (hasBeenBorrowed)
                        {
                            Console.WriteLine("You returned the book!");
                        }
                        else
                        {
                            Console.WriteLine("You entered invalid informations!");
                        }
                        Console.WriteLine();
                        break;
                    case '5':
                        Console.Write("Enter the ISBN: ");
                        isbn = Console.ReadLine();
                        Console.Write("Enter the Book Name: ");
                        name = Console.ReadLine();
                        Console.Write("Enter the borrowing Price: ");
                        try
                        {
                            price = double.Parse(Console.ReadLine());
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("You entered invalid informations!");
                            break;
                        }

                        hasBeenAdded = library.AddNewBook(new Book { ISBN = isbn, Name = name }, price);
                        if (hasBeenAdded)
                        {
                            Console.WriteLine("The book was added!");
                        }
                        else
                        {
                            Console.WriteLine("You entered invalid informations!");
                        }

                        break;
                    case '6':
                        Console.Write("Enter the ISBN: ");
                        isbn = Console.ReadLine();
                        Console.Write("Enter the Quantity: ");
                        try
                        {
                            quantity = int.Parse(Console.ReadLine());
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("You entered invalid informations!");
                            break;
                        }

                        hasBeenAdded = library.AddExistingBook(isbn, quantity);
                        if (hasBeenAdded)
                        {
                            Console.WriteLine("The quantity was added!");
                        }
                        else
                        {
                            Console.WriteLine("You entered invalid informations!");
                        }

                        break;
                    case '7':
                        library.DisplayBorrowedBooks();
                        Console.WriteLine();
                        break;
                    case '8':
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid Option");
                        break;
                }
            }
        }
    }
}
