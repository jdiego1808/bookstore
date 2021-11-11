using System;
using System.Data;
using System.Data.SqlClient;
using Utility.Entities;

namespace Utility.Repositories
{
    class SessionManager
    {
        private string _connectionString;
        private Session _session;
        public SessionManager(DbConnection connection)
        {
            _connectionString = connection.GetConnectionString();
            _session = null;
        }

        public SessionManager()
        {
            DbConnection connection = new DbConnection("Bookstore", "sa", "qwertASDF");
            _connectionString = connection.GetConnectionString();
            _session = null;
        }

        // Get session by session id
        public Session GetCurrentSession() => _session;

        public int UpsertSession(string seller_id) {
            _session = new() {
                SellerId = seller_id,
                StartingTime = DateTime.Now
            };
            
            SqlParameter paramPhone = new("@phone", SqlDbType.Char) { Value = seller_id };
            SqlParameter paramTime = new("@time", SqlDbType.DateTime) { Value = DateTime.Now };

            if (CheckSessionExist(seller_id)) {
                var updateCmd = "UPDATE session SET starting_time=@time WHERE seller_id=@phone";
                return SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, updateCmd, paramPhone, paramTime);
            }
            
            var insertCmd = "INSERT INTO session(seller_id) VALUES(@phone)";            
            return SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, insertCmd, paramPhone);
        }

        public int DeleteSession(string seller_id) {
            _session = null;
            
            var cmd = "DELETE FROM session WHERE seller_id=@phone";
            SqlParameter paramPhone = new("@phone", SqlDbType.Char) { Value = seller_id };
            return SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, cmd, paramPhone);
        }

        private bool CheckSessionExist(string seller_id) {
            var cmd = "SELLECT * FROM session WHERE seller_id=@phone";
            SqlParameter paramPhone = new("@phone", SqlDbType.Char) { Value = seller_id };
            using var res = SqlHelper.ExecuteReader(_connectionString, CommandType.Text, cmd, paramPhone);
            return res.HasRows;
        }



    }
}