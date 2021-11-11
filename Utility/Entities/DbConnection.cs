using System.Data;
using System.Data.SqlClient;

namespace Utility.Entities
{
    public class DbConnection
    {
        const string SERVERNAME = "localhost";
        private string _serverName;
        private string _databaseName;
        private string _userName;
        private string _password;

        public DbConnection(string serverName, string databaseName)
        {
            _serverName = serverName;
            _databaseName = databaseName;
        }

        public DbConnection(string serverName, string databaseName, string userName, string password)
        {
            _serverName = serverName;
            _databaseName = databaseName;
            _userName = userName;
            _password = password;
        }

        public DbConnection(string database, string userName, string password)
        {
            _userName = userName;
            _password = password;
        }

        public string GetConnectionString()
        {
            _serverName = string.IsNullOrEmpty(_serverName) ? SERVERNAME : _serverName;

            if (_userName == null)
            {
                return @"Data Source=" + _serverName + ";Initial Catalog=" + _databaseName + ";Integrated Security=true";
            }
            else
            {
                return @"Data Source=" + _serverName + ";Initial Catalog=" + _databaseName + ";User Id=" + _userName + ";Password=" + _password;
            }
        }
    }
    
}