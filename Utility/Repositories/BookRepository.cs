using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Utility.Entities;
using Utility.Entities.Responses;
using System.Globalization;

namespace Utility.Repositories
{
    public class BookRepository 
    {
        private readonly string _connectionString;
        private const string DEFAULT_IMAGE = "https://icon-library.com/images/photo-placeholder-icon/photo-placeholder-icon-14.jpg";

        public BookRepository(DbConnection connection)
        {
            _connectionString = connection.GetConnectionString();            
        }

        public BookRepository() {
            DbConnection connection = new ("Bookstore", "sa", "JDiego1808!");
            _connectionString = connection.GetConnectionString();
        }

        public BooksResponse GetAll() {
            try {
                var books = new List<Book>();
                var cmdBooks = "SELECT * FROM books";
                
                using var reader = SqlHelper.ExecuteReader(_connectionString, CommandType.Text, cmdBooks);
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string id = reader["id"].ToString();
                            var book = new Book() { Id = id };
                            string d = reader["publishedDate"].ToString();
                            if (string.IsNullOrEmpty(d)) book.PublishedDate = DateTime.MinValue;
                            else book.PublishedDate = DateTime.Parse(d, CultureInfo.InvariantCulture);
                            book.Title = reader["title"].ToString();
                            book.Isbn = reader["isbn"].ToString();
                            book.PageCount = Convert.ToInt32(reader["pageCount"]);
                            book.Description = reader["description"] != null ? reader["description"].ToString() : "";
                            book.Status = reader["status"].ToString();
                            book.ImagelUrl = reader["thumbnailUrl"] != null ? reader["thumbnailUrl"].ToString() : DEFAULT_IMAGE;
                            book.Price = reader["price"].ToString() == null ? -1 : Convert.ToDecimal(reader["price"]);
                            book.Quantity = reader["quantity"] != null ? Convert.ToInt32(reader["quantity"]) : 0;
                            book.Authors = GetAuthorsByBookId(id);
                            book.Categories = GetCategoriesByBookId(id);
                            
                            books.Add(book);
                        }
                       
                    }
                    else return new BooksResponse(false, "Fetch data failed");
                }
                return new BooksResponse(true, "Fetch data successful", books);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return new BooksResponse(false, "Fetch data failed");
            }
        }

        public BooksResponse GetBookById(string id) {
            try {
                var cmd = "SELECT * FROM books WHERE id = @id";
                using var reader = SqlHelper.ExecuteReader(_connectionString, CommandType.Text, cmd, new SqlParameter("@id", id));
                if (reader.Read())
                {
                    var book = new Book() { Id = id };
                    string d = reader["publishedDate"].ToString();
                    if (string.IsNullOrEmpty(d)) book.PublishedDate = DateTime.MinValue;
                    else book.PublishedDate = DateTime.Parse(d, CultureInfo.InvariantCulture);
                    book.Title = reader["title"].ToString();
                    book.Isbn = reader["isbn"].ToString();
                    book.PageCount = Convert.ToInt32(reader["pageCount"]);
                    book.Description = reader["description"] != null ? reader["description"].ToString() : "";
                    book.Status = reader["status"].ToString();
                    book.ImagelUrl = reader["thumbnailUrl"] != null ? reader["thumbnailUrl"].ToString() : DEFAULT_IMAGE;
                    book.Price = reader["price"].ToString() == null ? -1 : Convert.ToDecimal(reader["price"]);
                    book.Quantity = reader["quantity"] != null ? Convert.ToInt32(reader["quantity"]) : 0;
                    book.Authors = GetAuthorsByBookId(id);
                    book.Categories = GetCategoriesByBookId(id);
                    return new BooksResponse(true, "Fetch data successful", book);
                }
                return new BooksResponse(false, "Book not found");
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return new BooksResponse(false, "Fetch data failed");
            }
        }

        // Get books by text search
        public BooksResponse GetBooksByTextSearch(string text) {
            try {
                var cmd = "SELECT * FROM books WHERE title LIKE @text OR description LIKE @text";
                using var reader = SqlHelper.ExecuteReader(_connectionString, CommandType.Text, cmd, new SqlParameter("@text", "%" + text + "%"));
                var books = new List<Book>();
                while (reader.Read())
                {
                    string id = reader["id"].ToString();
                    var book = new Book() { Id = id };
                    string d = reader["publishedDate"].ToString();
                    if (string.IsNullOrEmpty(d)) book.PublishedDate = DateTime.MinValue;
                    else book.PublishedDate = DateTime.Parse(d, CultureInfo.InvariantCulture);
                    
                    book.Title = reader["title"].ToString();
                    book.Isbn = reader["isbn"].ToString();
                    book.PageCount = Convert.ToInt32(reader["pageCount"]);
                    book.Description = reader["description"] != null ? reader["description"].ToString() : "";
                    book.Status = reader["status"].ToString();
                    book.ImagelUrl = reader["thumbnailUrl"] != null ? reader["thumbnailUrl"].ToString() : DEFAULT_IMAGE;
                    book.Price = reader["price"].ToString() == null ? -1 : Convert.ToDecimal(reader["price"]);
                    book.Quantity = reader["quantity"] != null ? Convert.ToInt32(reader["quantity"]) : 0;
                    book.Authors = GetAuthorsByBookId(id);
                    book.Categories = GetAuthorsByBookId(id);

                    books.Add(book);
                }
                return new BooksResponse(true, "Fetch data successful", books);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return new BooksResponse(false, "Fetch data failed");
            }
        }

        // Get authors of book by book id
        public List<string> GetAuthorsByBookId(string id) {
            var cmd = "SELECT * FROM authors WHERE bookId = @id";
            using var reader = SqlHelper.ExecuteReader(_connectionString, CommandType.Text, cmd, new SqlParameter("@id", id));

            var authors = new List<string>();
            if (reader.HasRows) {
                while (reader.Read())
                {
                    authors.Add(reader["author"].ToString());
                }
                return authors;
            }
            
            return null;
        }

        // Get categories of book by book id
        public List<string> GetCategoriesByBookId(string id) {
            var cmd = "SELECT * FROM categories WHERE bookId = @id";
            using var reader = SqlHelper.ExecuteReader(_connectionString, CommandType.Text, cmd, new SqlParameter("@id", id));

            var categories = new List<string>();

            if (reader.HasRows) {
                while (reader.Read())
                {
                    categories.Add(reader["category"].ToString());
                }
                return categories;
            }
            
            return null;
        }

        // Get price of a book by book id
        public decimal GetPriceOfBookById(string id) {
            var cmd = "SELECT price FROM books WHERE id = @id";
            using var reader = SqlHelper.ExecuteReader(_connectionString, CommandType.Text, cmd, new SqlParameter("@id", id));
            if (reader.Read())
            {
                return Convert.ToDecimal(reader["price"]);
            }
            return 0;
        }

        // Update basic info of a book by book id
        public BooksResponse UpdateBookBasicInfo(Book book) {
            if (book == null) return new BooksResponse(false, "Book is null");
            if (book.Id == null) return new BooksResponse(false, "Book id is null");

            try {
                var cmd = "UPDATE books SET title = @title, isbn = @isbn, pageCount = @pageCount,"
                    + " description = @description, publishedDate = @publishedDate,"
                    + " status = @status, thumbnailurl = @thumbnailurl"
                    + " WHERE id = @id";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@title", SqlDbType.VarChar) { Value = book.Title ?? (object)DBNull.Value },
                    new SqlParameter("@isbn", SqlDbType.VarChar) { Value = book.Isbn ?? (object)DBNull.Value },
                    new SqlParameter("@pageCount", SqlDbType.Int) { 
                        Value = book.PageCount.ToString() != null ? book.PageCount : DBNull.Value 
                    },
                    new SqlParameter("@description", SqlDbType.VarChar) { Value = book.Description ?? (object)DBNull.Value },
                    new SqlParameter("@publishedDate", SqlDbType.DateTime) { 
                        Value = book.PublishedDate.ToString() != null ? book.PublishedDate : DBNull.Value 
                    },
                    new SqlParameter("@status", SqlDbType.VarChar) { Value = book.Status ?? (object)DBNull.Value },
                    new SqlParameter("@thumbnailurl", SqlDbType.VarChar) { Value = book.ImagelUrl ?? (object)DBNull.Value },
                    new SqlParameter("@id", SqlDbType.VarChar) { Value = book.Id }
                };
                SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, cmd, parameters.ToArray());
                return new BooksResponse(true, "Update book successful", book);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return new BooksResponse(false, "Update failed");
            }
        }

        // Update authors of a book by book id
        public BooksResponse UpdateBookAuthors(string id, List<string> authors) {
            try {
                var cmd = "DELETE FROM authors WHERE bookId = @id";
                SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, cmd, new SqlParameter("@id", id));

                InsertBookAuthors(id, authors);
                return new BooksResponse(success: true, message: "Update authors successful");
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return new BooksResponse(false, "Update failed");
            }
        }

        // Update categories of a book by book id
        public BooksResponse UpdateBookCategories(string id, List<string> categories) {
            try {
                var cmd = "DELETE FROM categories WHERE bookId = @id";
                SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, cmd, new SqlParameter("@id", id));

                InsertBookCategories(id, categories);
                return new BooksResponse(success: true, message: "Update categories successful");
            
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return new BooksResponse(false, "Update failed");
            }
        }

        // Update quantity of a book by book id
        public BooksResponse UpdateBookQuantity(string id, int quantity) {
            try {
                var cmd = "UPDATE books SET quantity = @quantity WHERE id = @id";
                SqlHelper.ExecuteNonQuery(
                    _connectionString, 
                    CommandType.Text, 
                    cmd, 
                    new SqlParameter("@quantity", quantity), 
                    new SqlParameter("@id", id)
                );
                return new BooksResponse(true, "Update quantity successful");
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return new BooksResponse(false, "Update failed");
            }
        }

        // Update price of a book by book id
        public BooksResponse UpdateBookPrice(string id, decimal price) {
            try {
                var cmd = "UPDATE books SET price = @price WHERE id = @id";
                SqlHelper.ExecuteNonQuery(
                    _connectionString, 
                    CommandType.Text, 
                    cmd, 
                    new SqlParameter("@price", price), 
                    new SqlParameter("@id", id)
                );
                return new BooksResponse(true, "Update price successful");
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return new BooksResponse(false, "Update failed");
            }
        }

        // Insert book authors
        public int InsertBookAuthors(string id, List<string> authors) {            
            try {
                int rowAffected = 0;
                SqlParameter paramId = new SqlParameter("@bookId", id);
                foreach (var author in authors)
                {
                    var cmd = "INSERT INTO authors (bookId, author) VALUES (@bookId, @author)";
                    int res = SqlHelper.ExecuteNonQuery(
                        _connectionString, 
                        CommandType.Text, 
                        cmd, 
                        new SqlParameter[] { paramId, new SqlParameter("@author", author) }
                    );
                    
                    rowAffected += res;
                }
                return rowAffected;
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                throw;
            }
            
        }

        // Insert book categories
        public int InsertBookCategories(string id, List<string> categories) {
            
            try {
                SqlParameter paramId = new SqlParameter("@bookId", id);
                int rowAffected = 0;
                foreach (var category in categories)
                {
                    var cmd = "INSERT INTO categories (bookId, category) VALUES (@bookId, @category)";
                    int res  = SqlHelper.ExecuteNonQuery(
                        _connectionString, 
                        CommandType.Text, 
                        cmd, 
                        new SqlParameter[] { paramId,  new SqlParameter("@category", category) }
                    );
                    rowAffected += res;
                }
                return rowAffected;
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                throw;
            }
        
        }

        // Insert a new book
        public BooksResponse InsertBook(Book book) {
            book.Id = book.GenerateId();
            Console.WriteLine(book);
            try {
                var cmd = "INSERT INTO books (id, title, isbn, pageCount, description, publishedDate, status, thumbnailurl, price, quantity) "
                    + "VALUES (@id, @title, @isbn, @pageCount, @description, @publishedDate, @status, @thumbnailurl, @price, @quantity)";

                SqlParameter[] parameters = new SqlParameter[] {
                    new SqlParameter("@id", SqlDbType.VarChar) { Value = book.Id },
                    new SqlParameter("@title", SqlDbType.VarChar) { Value = book.Title ?? (object)DBNull.Value },
                    new SqlParameter("@isbn", SqlDbType.VarChar) { Value = book.Isbn ?? (object)DBNull.Value },
                    new SqlParameter("@pageCount", SqlDbType.Int) { 
                        Value = book.PageCount.ToString() != null ? book.PageCount : DBNull.Value 
                    },
                    new SqlParameter("@description", SqlDbType.VarChar) { Value = book.Description ?? (object)DBNull.Value },
                    new SqlParameter("@publishedDate", SqlDbType.DateTime) { 
                        Value = book.PublishedDate.ToString() != null ? book.PublishedDate : DBNull.Value 
                    },
                    new SqlParameter("@status", SqlDbType.VarChar) { Value = book.Status ?? (object)DBNull.Value },
                    new SqlParameter("@thumbnailurl", SqlDbType.VarChar) { Value = book.ImagelUrl ?? (object)DBNull.Value },
                    new SqlParameter("@price", SqlDbType.Decimal) { 
                        Value = book.Price.ToString() != null ? book.Price : (object)DBNull.Value 
                    },
                    new SqlParameter("@quantity", SqlDbType.Int) { 
                        Value = book.Quantity.ToString() != null ? book.Quantity : 1
                    }
                };
                int rowAffected = SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, cmd, parameters);

                if (rowAffected == 0) {
                    return new BooksResponse(false, "Insert book failed");
                }                               

                InsertBookAuthors(book.Id, book.Authors);
                InsertBookCategories(book.Id, book.Categories);                
                
                return new BooksResponse(true, "Insert book successful", book);
            }
            catch (Exception ex) {           
                Console.WriteLine(ex.Message);    
                return new BooksResponse(false, "Insert failed");                
            }
            
        }

        // Delete a book by book id and delete all related data
        public BooksResponse DeleteBook(string id) {
            try {
                var cmd = "DELETE FROM authors WHERE bookId = @id";
                SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, cmd, new SqlParameter("@id", id));

                cmd = "DELETE FROM categories WHERE bookId = @id";
                SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, cmd, new SqlParameter("@id", id));

                cmd = "DELETE FROM books WHERE id = @id";
                int rowAffected = SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, cmd, new SqlParameter("@id", id));
                if (rowAffected == 0) {
                    return new BooksResponse(true, "Delete book failed");
                }
                
                return new BooksResponse(true, "Delete book successful");
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);    
                return new BooksResponse(false, "Delete failed");
            }
        }

    }
}