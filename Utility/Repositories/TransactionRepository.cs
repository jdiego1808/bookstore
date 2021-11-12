using System;
using System.Collections.Generic;
using Utility.Entities;
using Utility.Entities.Responses;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Utility.Repositories
{
    public class TransactionRepository
    {
        private const string ITEM_BOOK = "BookSell";
        private const string ITEM_STATIONERY = "StationerySell";
        private string _connectionString;
        private StationeryRepository _stationeryRepository;
        private BookRepository _bookRepository;
        private SellerRepository _sellerRepository;

        public TransactionRepository(DbConnection connection, BookRepository bookRepository, StationeryRepository stationeryRepository, SellerRepository sellerRepository)
        {
            _connectionString = connection.GetConnectionString();
            _bookRepository = bookRepository;
            _stationeryRepository = stationeryRepository;
            _sellerRepository = sellerRepository;
        }

        public TransactionRepository(DbConnection connection, SellerRepository sellerRepository)
        {
            _connectionString = connection.GetConnectionString();
            _bookRepository = new(connection);
            _stationeryRepository = new(connection);
            _sellerRepository = sellerRepository;
        }

        // Create transaction
        public Transaction Create(string seller_id, Dictionary<string, int> books, Dictionary<string, int> stationeries)
        {
            Transaction transaction = new Transaction();
            decimal totalPrice = 0;
            transaction.SellerId = seller_id;

            // loop through books
            foreach (KeyValuePair<string, int> book in books) {
                decimal price = _bookRepository.GetPriceOfBookById(book.Key);
                totalPrice += price * book.Value;
                transaction.Books.Add(new BookSell() { Id = book.Key, Quantity = book.Value, Price = price });
            }
            // loop through stationeries
            foreach (KeyValuePair<string, int> stationery in stationeries)
            {
                var price = _stationeryRepository.GetPriceOfStationeryById(stationery.Key);
                totalPrice += price * stationery.Value;

                transaction.Stationeries.Add(new StationerySell() { 
                    Id = stationery.Key, 
                    Quantity = stationery.Value, 
                    Price = price 
                });
            }
            transaction.TotalPrice = totalPrice;
            return transaction;
        }

        // Get invoice of a transaction
        public TransactionResponse GetInvoice(Transaction transaction)
        {
            try {
                Invoice invoice = new Invoice() {
                    Id = transaction.Id,
                    Seller = _sellerRepository.GetCurrentSession().Seller.Name,
                    Date = DateTime.Now,
                    Total = transaction.TotalPrice,
                    Items = new List<Item>()
                };

                transaction.Books.ForEach(book => {
                    invoice.Items.Add(new Item() {
                        Name = _bookRepository.GetBookById(book.Id).Book.Title,
                        Id = book.Id,
                        Quantity = book.Quantity,
                        Price = book.Price,
                        Unit = "book"
                    });                    
                });
                transaction.Stationeries.ForEach(stationery => {
                    var s = _stationeryRepository.GetStationeryById(stationery.Id);
                    invoice.Items.Add(new Item() {                        
                        Name = s.Stationery.Name,
                        Id = stationery.Id,
                        Quantity = stationery.Quantity,
                        Price = stationery.Price,
                        Unit = s.Stationery.Unit
                    });
                });
                
                return new TransactionResponse(true, "Get invoice data succesful", invoice);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return new TransactionResponse(false, "An error occurred");
            }
        }

        public TransactionResponse Add(Transaction transaction)
        {
            try {
                var cmdInsertTran = "INSERT INTO [transactions] (id, seller_id, create_date, total_price) VALUES (@id, @SellerId, @Date, @TotalPrice)";

                SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, cmdInsertTran, new SqlParameter[] {
                    new SqlParameter("@id", transaction.Id),
                    new SqlParameter("@SellerId", transaction.SellerId),
                    new SqlParameter("@Date", transaction.CreateDate),
                    new SqlParameter("@TotalPrice", transaction.TotalPrice)
                });

                transaction.Books.ForEach(book => {
                    AddItem(transaction.Id, book);
                });
                transaction.Stationeries.ForEach(stationery => {
                    AddItem(transaction.Id, stationery);
                });
                
                return new TransactionResponse(true, "Insert transaction succesful");
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return new TransactionResponse(false, "An error occurred");
            }
        }

        // Add item to a transaction
        private int AddItem(string transactionId, Item item)
        {
            try {
                var cmd = "";
                switch (item.GetType().Name) {
                    case ITEM_BOOK:
                        cmd = "INSERT INTO [book_sells] (trans_id, book_id, quantity) VALUES(@trans_id, @item_id, @quantity)";
                        break;
                    case ITEM_STATIONERY:
                        cmd = "INSERT INTO [stationery_sells] (trans_id, stationery_id, quantity) VALUES(@trans_id, @item_id, @quantity)";
                        break;
                    default: 
                        return 0;
                }

                return SqlHelper.ExecuteNonQuery(_connectionString, CommandType.Text, cmd, new SqlParameter[] {
                    new SqlParameter("@trans_id", transactionId),
                    new SqlParameter("@item_id", item.Id),
                    new SqlParameter("@quantity", item.Quantity)
                });

            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        // Get all transactions
        public TransactionResponse GetAll()
        {
            try {
                var cmd = "SELECT * FROM [transactions]";
                var res = SqlHelper.ExecuteReader(_connectionString, CommandType.Text, cmd);
                var transactions = new List<Transaction>();
                while (res.Read()) {
                    var transaction = new Transaction();
                    transaction.Id = res["id"].ToString();
                    transaction.SellerId = res["seller_id"].ToString();
                    transaction.CreateDate = DateTime.Parse((res["create_date"].ToString()));
                    transaction.TotalPrice = decimal.Parse(res["total_price"].ToString());
                    transactions.Add(transaction);
                }
                return new TransactionResponse(true, "Get all transactions succesful", transactions);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return new TransactionResponse(false, "An error occurred");
            }
        }

    }
}