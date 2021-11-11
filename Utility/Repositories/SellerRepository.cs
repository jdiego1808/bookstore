using System;
using Utility.Entities;
using Utility.Entities.Responses;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Utility.Repositories
{
    public class SellerRepository
    {
        private string _connectionString;
        private SessionManager _session;
        private const string ADMIN = "admin";
        private const string SELLER = "seller";
        private static List<string> ROLES = new() { ADMIN, SELLER };

        public SellerRepository(DbConnection connection) {
            _connectionString = connection.GetConnectionString();
            _session = new(connection);
        }

        public SellerRepository() {
            DbConnection connection = new DbConnection("Bookstore", "sa", "qwertASDF");
            _connectionString = connection.GetConnectionString();
            _session = new(connection);
        }

        public Seller GetSellerByPhone(string phone) {
            var cmd = "SELECT * FROM seller WHERE @phone=phone";
            SqlParameter phoneParam = new SqlParameter("@phone", SqlDbType.Char) { Value = phone };
            using var res = SqlHelper.ExecuteReader(_connectionString, cmd, phoneParam);
            if (res.HasRows)
            {
                res.Read();
                return new Seller
                {
                    Id = res["id"].ToString(),
                    Name = res["name"].ToString(),
                    Phone = res["phone"].ToString(),
                    Hash = res["hash_password"].ToString(),
                    Address = res["address"].ToString(),
                    Role = res["role"].ToString()
                };
            }

            return null;
        }

        public Seller GetSellerById(string id) {
            var cmd = "SELECT * FROM seller WHERE id=@id";
            SqlParameter phoneParam = new SqlParameter("@id", SqlDbType.VarChar) { Value = id };
            using var res = SqlHelper.ExecuteReader(_connectionString, cmd, phoneParam);
            if (res.HasRows)
            {
                res.Read();
                return new Seller
                {
                    Id = res["id"].ToString(),
                    Name = res["name"].ToString(),
                    Phone = res["phone"].ToString(),
                    Hash = res["hash_password"].ToString(),
                    Address = res["address"].ToString(),
                    Role = res["role"].ToString()
                };
            }
            
            return null;
        }

        // login method
        public SellerResponse Login(string phone, string password) {
            try {
                var storedSeller = GetSellerByPhone(phone);
                if (storedSeller == null)
                {
                    return new SellerResponse(false, "No seller found. Please check the phone number.");
                }
                if (!PasswordHashOMatic.Verify(password, storedSeller.Hash))
                {
                    return new SellerResponse(false, "The password provided is not valid");
                }
                
                int res = _session.UpsertSession(phone);
                if (res == 0) return new SellerResponse(false, "Session update error");

                return new SellerResponse(true, "Login successful", storedSeller);
            }
            catch(Exception ex)
            {
                return new SellerResponse(false, ex.Message);
            }
        }

        // register method
        public SellerResponse Register(Seller seller) {
            try {               
                int res = AddSeller(seller);
                if (res == -1) return new SellerResponse(false, "Please fill all the required fields");
                if (res == 0) return new SellerResponse(false, "Seller registration failed");

                int res1 = _session.UpsertSession(seller.Phone);
                if (res1 == 0) return new SellerResponse(false, "Session update error");

                return new SellerResponse(true, "Seller registration successful", seller);
            }
            catch(Exception ex) {
                return new SellerResponse(false, ex.Message);
            }
        }

        public SellerResponse Logout(string phone) {
            try {
                int res = _session.DeleteSession(phone);
                if (res == 0) return new SellerResponse(false, "Session update error");

                return new SellerResponse(true, "Logout successful");
            }
            catch(Exception ex) {
                return new SellerResponse(false, ex.Message);
            }
        }

        public SellerResponse GetCurrentSession() {
            try {
                var session = _session.GetCurrentSession();
                if (session == null) return new SellerResponse(false, "No session found");
                var seller = GetSellerByPhone(session.SellerId);

                return new SellerResponse(true, "Session found", seller);
            } 
            catch (Exception ex) {
                return new SellerResponse(false, ex.Message);
            }
        }

        public int AddSeller(Seller seller) {
            try {

                if (string.IsNullOrEmpty(seller.Phone) || string.IsNullOrEmpty(seller.Name) || string.IsNullOrEmpty(seller.Password)) {
                    return -1;
                }
                var cmd = "INSERT INTO sellers(name, address, phone, hash_password, role) VALUES(@name, @address, @phone, @hash, @role)";
                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@name", SqlDbType.VarChar) { Value = seller.Name },
                    new SqlParameter("@address", SqlDbType.VarChar) { Value = seller.Address },
                    new SqlParameter("@phone", SqlDbType.Char) { Value = seller.Phone },
                    new SqlParameter("@hash", SqlDbType.VarChar) { Value = PasswordHashOMatic.Hash(seller.Password) },
                    new SqlParameter("@role", SqlDbType.VarChar) { 
                        Value = string.IsNullOrEmpty(seller.Role) ? SELLER : seller.Role
                    }
                };
                
                return SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, cmd, sqlParams);
            }
            catch(Exception ex) {
                throw ex;
            }

        }

        // update seller password
        public SellerResponse UpdatePassword(string phone, string new_password, string old_password) {
            try
            {
                var storedSeller = GetSellerByPhone(phone);
                if (storedSeller == null) {
                    return new SellerResponse(false, "No seller found. Please check the phone number.");
                }
                if (!PasswordHashOMatic.Verify(old_password, storedSeller.Hash)) {
                    return new SellerResponse(false, "The password provided is not valid");
                }

                var cmd = "UPDATE sellers SET hash_password=@hash WHERE phone=@phone";
                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@hash", SqlDbType.VarChar) { Value = PasswordHashOMatic.Hash(new_password) },
                    new SqlParameter("@phone", SqlDbType.Char) { Value = phone }
                };

                int res = SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, cmd, sqlParams);
                if (res == 0) return new SellerResponse(false, "Password update failed");

                return new SellerResponse(true, "Password update successful");
            }
            catch(Exception ex) {
                return new SellerResponse(false, ex.Message);
            }
        }

        public SellerResponse UpdateSellerRole(string phone, string role) {
            try {
                var currentSeller = GetSellerByPhone(_session.GetCurrentSession().SellerId);
                if (currentSeller.Role != ADMIN) {
                    return new SellerResponse(false, "You are not authorized to perform this action");
                }

                var storedSeller = GetSellerByPhone(phone);
                if (storedSeller == null) {
                    return new SellerResponse(false, "No seller found. Please check the phone number.");
                }
                
                if (!ROLES.Contains(role.ToLower())) {
                    return new SellerResponse(false, "The role provided is not valid");
                }

                var cmd = "UPDATE sellers SET role=@role WHERE phone=@phone";
                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@role", SqlDbType.VarChar) { Value = role },
                    new SqlParameter("@phone", SqlDbType.Char) { Value = phone }
                };

                int res = SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, cmd, sqlParams);
                if (res == 0) return new SellerResponse(false, "Role update failed");

                return new SellerResponse(true, "Seller role update successful");
            }
            catch(Exception ex)
            {
                return new SellerResponse(false, ex.Message);
            }
        }

        public SellerResponse UpdateSellerAddress(string phone, string address)
        {
            try
            {
                if (_session.GetCurrentSession() == null)
                {
                    return new SellerResponse(false, "You are not authorized to perform this action");
                }
                var storedSeller = GetSellerByPhone(phone);
                if (storedSeller == null)
                {
                    return new SellerResponse(false, "No seller found. Please check the phone number.");
                }

                var cmd = "UPDATE sellers SET address=@address WHERE phone=@phone";
                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@address", SqlDbType.VarChar) { Value = address },
                    new SqlParameter("@phone", SqlDbType.Char) { Value = phone }
                };

                int res = SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, cmd, sqlParams);
                if (res == 0) return new SellerResponse(false, "Address update failed");

                return new SellerResponse(true, "Seller address update successful");
            }
            catch(Exception ex)
            {
                return new SellerResponse(false, ex.Message);
            }
        }

        public SellerResponse UpdateSellerName(string phone, string name)
        {
            try
            {
                if (_session.GetCurrentSession() == null)
                {
                    return new SellerResponse(false, "You are not authorized to perform this action");
                }
                var storedSeller = GetSellerByPhone(phone);
                if (storedSeller == null)
                {
                    return new SellerResponse(false, "No seller found. Please check the phone number.");
                }

                var cmd = "UPDATE sellers SET name=@name WHERE phone=@phone";
                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@name", SqlDbType.VarChar) { Value = name },
                    new SqlParameter("@phone", SqlDbType.Char) { Value = phone }
                };

                int res = SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, cmd, sqlParams);
                if (res == 0) return new SellerResponse(false, "Name update failed");

                return new SellerResponse(true, "Seller name update successful");
            }
            catch(Exception ex)
            {
                return new SellerResponse(false, ex.Message);
            }
        }
        
        public SellerResponse UpdateSellerPhone(string seller_id, string new_phone) {
            try {
                var currentSeller = GetSellerByPhone(_session.GetCurrentSession().SellerId);
                if (currentSeller.Role != ADMIN) {
                    return new SellerResponse(false, "You are not authorized to perform this action");
                }

                var storedSeller = GetSellerById(seller_id);
                if (storedSeller == null) {
                    return new SellerResponse(false, "No seller found. Please check seller id again.");
                }

                var cmd = "UPDATE sellers SET phone=@phone WHERE id=@id";
                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@phone", SqlDbType.Char) { Value = new_phone },
                    new SqlParameter("@id", SqlDbType.VarChar) { Value = seller_id }
                };

                int res = SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, cmd, sqlParams);
                if (res == 0) return new SellerResponse(false, "Phone update failed");

                return new SellerResponse(true, "Seller phone update successful");
            }
            catch(Exception ex) {
                return new SellerResponse(false, ex.Message);
            }
        }

        // Delete seller by id
        // only admin can delete a seller
        public SellerResponse DeleteSellerById(string seller_id) {
            try {
                var currentSeller = GetSellerByPhone(_session.GetCurrentSession().SellerId);
                if (currentSeller.Role != ADMIN) {
                    return new SellerResponse(false, "You are not authorized to perform this action");
                }

                var storedSeller = GetSellerById(seller_id);
                if (storedSeller == null) {
                    return new SellerResponse(false, "No seller found. Please check seller id again.");
                }

                var cmd = "DELETE FROM sellers WHERE id=@id";
                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@id", SqlDbType.VarChar) { Value = seller_id }
                };

                int res = SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, cmd, sqlParams);
                if (res == 0) return new SellerResponse(false, "Seller delete failed");

                return new SellerResponse(true, "Seller delete successful");
            }
            catch(Exception ex) {
                return new SellerResponse(false, ex.Message);
            }
        }

        // Delete seller by phone
        // only admin can delete a seller
        public SellerResponse DeleteSellerByPhone(string phone) {
            try {
                var currentSeller = GetSellerByPhone(_session.GetCurrentSession().SellerId);
                if (currentSeller.Role != ADMIN) {
                    return new SellerResponse(false, "You are not authorized to perform this action");
                }

                var storedSeller = GetSellerByPhone(phone);
                if (storedSeller == null) {
                    return new SellerResponse(false, "No seller found. Please check seller phone again.");
                }

                var cmd = "DELETE FROM sellers WHERE phone=@phone";
                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@phone", SqlDbType.Char) { Value = phone }
                };

                int res = SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, cmd, sqlParams);
                if (res == 0) return new SellerResponse(false, "Seller delete failed");

                return new SellerResponse(true, "Seller delete successful");
            }
            catch(Exception ex) {
                return new SellerResponse(false, ex.Message);
            }
        }
    }
}