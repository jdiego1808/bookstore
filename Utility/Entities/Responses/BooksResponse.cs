using System;
using System.Collections.Generic;

namespace Utility.Entities.Responses
{
    public class BooksResponse
    {
        public List<Book> Books { get; set; }
        public int Count { get; set; }
        public Book Book { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }

        public BooksResponse(bool success, string message)
        {
            Success = false;
            Message = string.Empty;
            Books = new List<Book>();
            Count = 0;
            Book = new Book();
        }

        public BooksResponse(bool success, string message, List<Book> books)
        {
            Success = success;
            Message = message;
            Books = books;
            Count = books.Count;
            Book = new Book();
        }

        public BooksResponse(bool success, string message, Book book)
        {
            Success = success;
            Message = message;
            Book = book;
            Books = new List<Book>();
            Count = 0;
        }
    }
}