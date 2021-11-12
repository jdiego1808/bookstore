using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Utility.Entities;
using Utility.Entities.Responses;

namespace Utility.Repositories
{
    public class StationeryRepository
    {
        private readonly string _connectionString;
        private const string DEFAULT_IMAGE = "https://icon-library.com/images/photo-placeholder-icon/photo-placeholder-icon-14.jpg";

        public StationeryRepository(DbConnection connection)
        {
            _connectionString = connection.GetConnectionString();
        }

        public StationeryRepository() {
            DbConnection connection = new DbConnection("Bookstore", "sa", "JDiego1808!");
            _connectionString = connection.GetConnectionString();
        }

        // Get all stationery
        public StationeryResponse GetAllStationeries() {
            try {
                var cmd = "SELECT * FROM stationeries";
                List<Stationery> stationeries = new List<Stationery>();
                using var reader = SqlHelper.ExecuteReader(_connectionString, CommandType.Text, cmd);
                if (reader.HasRows) {
                    while(reader.Read()) {
                        string imageUrl = reader["thumbnail_url"].ToString();
                        string unit = reader["unit"].ToString();
                        string price = reader["price"].ToString();
                        string quantity = reader["quantity"].ToString();

                        var stationery = new Stationery() {
                            Id = reader["id"].ToString(),
                            Name = reader["product_name"].ToString(),
                            ImageUrl = string.IsNullOrEmpty(imageUrl) ? DEFAULT_IMAGE : imageUrl,
                            Unit = string.IsNullOrEmpty(unit) ? "unspecificed" : unit,
                            Price = string.IsNullOrEmpty(price) ? 0 : Convert.ToDecimal(price),
                            Quantity = string.IsNullOrEmpty(quantity) ? 0 : Convert.ToInt32(quantity)
                        };
                        stationeries.Add(stationery);
                    }
                    return new StationeryResponse(true, "Fetch data successful", stationeries);
                }
                return new StationeryResponse(false, "No data found");
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return new StationeryResponse(false, "Fetch data failed");
            }
        }
        
        // Get stationery by id
        public StationeryResponse GetStationeryById(string id) {
            try {
                var cmd = "SELECT * FROM stationeries WHERE id = @id";
                using var reader = SqlHelper.ExecuteReader(_connectionString, CommandType.Text, cmd, new SqlParameter("@id", id));

                if(reader.Read()) {
                    string imageUrl = reader["thumbnail_url"].ToString();
                    string unit = reader["unit"].ToString();
                    string price = reader["price"].ToString();
                    string quantity = reader["quantity"].ToString();

                    var stationery = new Stationery() {
                        Id = reader["id"].ToString(),
                        Name = reader["product_name"].ToString(),
                        ImageUrl = string.IsNullOrEmpty(imageUrl) ? DEFAULT_IMAGE : imageUrl,
                        Unit = string.IsNullOrEmpty(unit) ? "unspecificed" : unit,
                        Price = string.IsNullOrEmpty(price) ? 0 : Convert.ToDecimal(price),
                        Quantity = string.IsNullOrEmpty(quantity) ? 0 : Convert.ToInt32(quantity)
                    };
                    return new StationeryResponse(true, "Fetch data successful", stationery);
                }
                return new StationeryResponse(false, "Stationery not found");
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return new StationeryResponse(false, "Fetch data failed");
            }
        }

        // Get stationery by text search
        public StationeryResponse GetStationeryByTextSearch(string text) {
            try {
                var cmd = "SELECT * FROM stationeries WHERE product_name LIKE @text";
                using var reader = SqlHelper.ExecuteReader(_connectionString, CommandType.Text, cmd, new SqlParameter("@text", "%" + text + "%"));
                List<Stationery> stationeries = new List<Stationery>();

                if (reader.HasRows) {
                    while(reader.Read()) {
                        string imageUrl = reader["thumbnail_url"].ToString();
                        string unit = reader["unit"].ToString();
                        string price = reader["price"].ToString();
                        string quantity = reader["quantity"].ToString();

                        var stationery = new Stationery() {
                            Id = reader["id"].ToString(),
                            Name = reader["product_name"].ToString(),
                            ImageUrl = string.IsNullOrEmpty(imageUrl) ? DEFAULT_IMAGE : imageUrl,
                            Unit = string.IsNullOrEmpty(unit) ? "unspecificed" : unit,
                            Price = string.IsNullOrEmpty(price) ? 0 : Convert.ToDecimal(price),
                            Quantity = string.IsNullOrEmpty(quantity) ? 0 : Convert.ToInt32(quantity)
                        };
                        stationeries.Add(stationery);
                    }
                    return new StationeryResponse(true, "Fetch data successful", stationeries);
                }
                return new StationeryResponse(false, "Stationery not found");
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return new StationeryResponse(false, "Fetch data failed");
            }
        }

        // Get price of stationery by id
        public decimal GetPriceOfStationeryById(string id) {
            try {
                var cmd = "SELECT price FROM stationeries WHERE id = @id";
                using var reader = SqlHelper.ExecuteReader(_connectionString, CommandType.Text, cmd, new SqlParameter("@id", id));

                if(reader.Read()) {
                    var price = Convert.ToDecimal(reader["price"]);
                    return price;
                }
                return -1;
            }
            catch {
                throw;
            }
        }

        // Insert a new stationery
        public StationeryResponse InsertStationery(Stationery stationery) {
            try {
                var cmd = "INSERT INTO stationeries (product_name, thumbnail_url, unit, price, quantity) " 
                    + "VALUES (@name, @imageUrl, @unit, @price, @quantity)";
                var parameters = new SqlParameter[] {
                    new SqlParameter("@name", stationery.Name),
                    new SqlParameter("@imageUrl", stationery.ImageUrl),
                    new SqlParameter("@unit", stationery.Unit),
                    new SqlParameter("@price", stationery.Price),
                    new SqlParameter("@quantity", stationery.Quantity)
                };
                SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, cmd, parameters);
                return new StationeryResponse(true, "Insert data successful");

            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return new StationeryResponse(false, "Insert failed");
            }
        }
        
        // Update a stationery
        public StationeryResponse UpdateStationeryBasicInfo(Stationery stationery) {
            try {
                var cmd = "UPDATE stationeries SET product_name = @name, thumbnail_url = @imageUrl, unit = @unit WHERE id = @id";
                var parameters = new SqlParameter[] {
                    new SqlParameter("@name", stationery.Name),
                    new SqlParameter("@imageUrl", stationery.ImageUrl),
                    new SqlParameter("@unit", stationery.Unit),
                    new SqlParameter("@id", stationery.Id)
                };
                SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, cmd, parameters);

                return new StationeryResponse(true, "Update data successful");

            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return new StationeryResponse(false, "Update failed");
            }
        }

        // Update price of a stationery
        public StationeryResponse UpdateStationeryPrice(string id, decimal price) {
            try {
                var cmd = "UPDATE stationeries SET price = @price WHERE id = @id";
                var parameters = new SqlParameter[] {
                    new SqlParameter("@price", price),
                    new SqlParameter("@id", id)
                };
                SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, cmd, parameters);
                return new StationeryResponse(true, "Update data successful");
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return new StationeryResponse(false, "Update failed");
            }
        }

        // Update quantity of a stationery
        public StationeryResponse UpdateStationeryQuantity(string id, int quantity) {
            try {
                var cmd = "UPDATE stationeries SET quantity = @quantity WHERE id = @id";
                var parameters = new SqlParameter[] {
                    new SqlParameter("@quantity", quantity),
                    new SqlParameter("@id", id)
                };
                SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, cmd, parameters);

                return new StationeryResponse(true, "Update data successful");

            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return new StationeryResponse(false, "Update failed");
            }
        }

        // Delete a stationery
        public StationeryResponse DeleteStationery(string id) {
            try {
                var cmd = "DELETE FROM stationeries WHERE id = @id";
                var parameters = new SqlParameter[] {
                    new SqlParameter("@id", id)
                };
                SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, cmd, parameters);
                return new StationeryResponse(true, "Delete data successful");

            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return new StationeryResponse(false, "Delete failed");
            }
        }
        
    }
}