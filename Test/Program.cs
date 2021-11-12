using System;
using Utility;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Utility.Repositories;
using Utility.Entities;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
           BookRepository bookRespository = new();
            /* var res = bookRespository.GetAll();
             var books = res.Books;
           
            Console.WriteLine("ID\tTitle\tPublishedDate\tPrice\tQuantity\tSatus");
            if (res.Success)
                books.ForEach(book =>
                {
                   if (book.PublishedDate.Equals(DateTime.MinValue))
                        Console.WriteLine($"{book.Id} - {book.Title} - {book.PageCount} - null - {book.Price} - {book.Quantity} - {book.Status}");
                    Console.WriteLine($"{book.Id} - {book.Title} - {book.PageCount} - {book.PublishedDate} - {book.Price} - {book.Quantity} - {book.Status}");

                });

            Console.WriteLine(res.Message);*/
            var book = new Book()
            {
                Title = "Test book",
                PageCount = 264,
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                Isbn = "1234567890",
                ImagelUrl = null,
                PublishedDate = DateTime.Now,
                Price = 23.99M,
                Status = "PUBLISH",
                Quantity = 1,
                Authors = new List<string>() { "Diego", "Klaus" },
                Categories = new List<string>() { "JavaScript", "Design Pattern" }
            };
            var res = bookRespository.InsertBook(book);
            Console.WriteLine(res.Message);


        }
    }
}
