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

        public BookRepository(DbConnection connection)
        {
            _connectionString = connection.GetConnectionString();            
        }

        public BookRepository() {
            DbConnection connection = new ("BooksDB", "sa", "qwertASDF");
            _connectionString = connection.GetConnectionString();
        }

        public BooksResponse GetAll() {
            try {
                var books = new List<Book>();
                var cmdBooks = "SELECT * FROM books";
               /* var cmdAuthors = "SELECT * FROM authors";
                var cmdCategories = "SELECT * FROM categories";

                Dictionary<string, List<string>> categories = new();
                Dictionary<string, List<string>> authors = new();

                using var dsAuthors = SqlHelper.ExecuteDataset(_connectionString, CommandType.Text, cmdAuthors);
                {
                    authors = dsAuthors.Tables[0].AsEnumerable()
                        .GroupBy(r => r.Field<string>("bookId"))
                        .ToDictionary(r => r.Key, r => r.Select(x => x.Field<string>("author")).ToList());
                }
                using var dsCategories = SqlHelper.ExecuteDataset(_connectionString, CommandType.Text, cmdCategories);
                {
                    categories = dsCategories.Tables[0].AsEnumerable()
                        .GroupBy(r => r.Field<string>("bookId"))
                        .ToDictionary(r => r.Key, r => r.Select(x => x.Field<string>("category")).ToList());
                }*/

                
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
                            book.ImagelUrl = reader["thumbnailUrl"] != null ? reader["thumbnailUrl"].ToString() : "";
                            book.Price = Convert.ToDecimal(reader["price"]);
                            book.Quantity = Convert.ToInt32(reader["quantity"]);
                            /* book.Authors = authors.TryGetValue(id, out List<string> authorList) ? authorList : new List<string>();
                             book.Categories = categories.TryGetValue(id, out List<string> categoryList) ? categoryList : new List<string>();*/
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
                return new BooksResponse(false, ex.Message);
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
                    book.ImagelUrl = reader["thumbnailUrl"] != null ? reader["thumbnailUrl"].ToString() : "";
                    book.Price = Convert.ToDecimal(reader["price"]);
                    book.Quantity = Convert.ToInt32(reader["quantity"]);
                    book.Authors = GetAuthorsByBookId(id);
                    book.Categories = GetCategoriesByBookId(id);
                    return new BooksResponse(true, "Fetch data successful", book);
                }
                return new BooksResponse(false, "Book not found");
            }
            catch (Exception ex) {
                return new BooksResponse(false, ex.Message);
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
                    book.ImagelUrl = reader["thumbnailUrl"] != null ? reader["thumbnailUrl"].ToString() : "";
                    book.Price = Convert.ToDecimal(reader["price"]);
                    book.Quantity = Convert.ToInt32(reader["quantity"]);
                    book.Authors = GetAuthorsByBookId(id);
                    book.Categories = GetAuthorsByBookId(id);

                    books.Add(book);
                }
                return new BooksResponse(true, "Fetch data successful", books);
            }
            catch (Exception ex) {
                return new BooksResponse(false, ex.Message);
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
            try {
                var cmd = "UPDATE books SET title = @title, isbn = @isbn, pageCount = @pageCount,"
                    + " description = @description, publishedDate = @publishedDate,"
                    + " status = @status, thumbnailurl = @thumbnailurl"
                    + " WHERE id = @id";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@title", book.Title),
                    new SqlParameter("@isbn", book.Isbn),
                    new SqlParameter("@pageCount", book.PageCount),
                    new SqlParameter("@description", book.Description),
                    new SqlParameter("@publishedDate", book.PublishedDate),
                    new SqlParameter("@status", book.Status),
                    new SqlParameter("@thumbnailurl", book.ImagelUrl),
                    new SqlParameter("@id", book.Id)
                };
                SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, cmd, parameters.ToArray());
                return new BooksResponse(true, "Update book successful", book);
            }
            catch (Exception ex) {
                return new BooksResponse(false, ex.Message);
            }
        }

        // Update authors of a book by book id
        public BooksResponse UpdateBookAuthors(string id, List<string> authors) {
            try {
                var cmd = "DELETE FROM authors WHERE bookId = @id";
                SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, cmd, new SqlParameter("@id", id));

                int res = InsertBookAuthors(id, authors);
                if (res != 0) {
                    return new BooksResponse(true, "Update authors successful");
                }
                return new BooksResponse(false, "Update authors failed");
            }
            catch (Exception ex) {
                return new BooksResponse(false, ex.Message);
            }
        }

        // Update categories of a book by book id
        public BooksResponse UpdateBookCategories(string id, List<string> categories) {
            try {
                var cmd = "DELETE FROM categories WHERE bookId = @id";
                SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, cmd, new SqlParameter("@id", id));

                int res = InsertBookCategories(id, categories);
                if (res != 0) {
                    return new BooksResponse(true, "Update categories successful");
                }
                return new BooksResponse(false, "Update categories failed");
            }
            catch (Exception ex) {
                return new BooksResponse(false, ex.Message);
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
                return new BooksResponse(false, ex.Message);
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
                return new BooksResponse(false, ex.Message);
            }
        }

        // Insert book authors
        public int InsertBookAuthors(string id, List<string> authors) {
            try {
                int rowAffected = 0;
                foreach (var author in authors)
                {
                    var cmd = "INSERT INTO authors (bookId, author) VALUES (@bookId, @author)";
                    rowAffected  = SqlHelper.ExecuteNonQuery(
                        _connectionString, 
                        CommandType.Text, 
                        cmd, 
                        new SqlParameter("@bookId", id), 
                        new SqlParameter("@author", author)
                    );
                }
                return rowAffected;
            }
            catch { throw; }
        }

        // Insert book categories
        public int InsertBookCategories(string id, List<string> categories) {
            try {
                int rowAffected = 0;
                foreach (var category in categories)
                {
                    var cmd = "INSERT INTO categories (bookId, category) VALUES (@bookId, @category)";
                    rowAffected  = SqlHelper.ExecuteNonQuery(
                        _connectionString, 
                        CommandType.Text, 
                        cmd, 
                        new SqlParameter("@bookId", id), 
                        new SqlParameter("@category", category)
                    );
                }
                return rowAffected;
            }
            catch { throw; }
        }

        // Insert a new book
        public BooksResponse InsertBook(Book book) {
            try {
                var cmd = "INSERT INTO books (title, isbn, pageCount, description, publishedDate, status, thumbnailurl, price, quantity) "
                    + "VALUES (@title, @isbn, @pageCount, @description, @publishedDate, @status, @thumbnailurl, @price, @quantity)";

                SqlParameter[] parameters = new SqlParameter[] {
                    new SqlParameter("@title", book.Title),
                    new SqlParameter("@isbn", book.Isbn),
                    new SqlParameter("@pageCount", book.PageCount),
                    new SqlParameter("@description", book.Description),
                    new SqlParameter("@publishedDate", book.PublishedDate),
                    new SqlParameter("@status", book.Status),
                    new SqlParameter("@thumbnailurl", book.ImagelUrl),
                    new SqlParameter("@price", book.Price),
                    new SqlParameter("@quantity", book.Quantity)
                };
                int rowAffected = SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, cmd, parameters);
                if (rowAffected == 0) {
                    return new BooksResponse(true, "Insert book failed");
                }
                InsertBookAuthors(book.Id, book.Authors);
                InsertBookCategories(book.Id, book.Categories);

                return new BooksResponse(true, "Insert book successful", book);
            }
            catch (Exception ex) {
                return new BooksResponse(false, ex.Message);
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
                return new BooksResponse(false, ex.Message);
            }
        }

    }
}