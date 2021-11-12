using System;
using Utility;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Utility.Repositories;
using Utility.Entities;
using Utility.Entities.Responses;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            
            DbConnection db = new DbConnection("127.0.0.1", "Bookstore", "sa", "JDiego1808!");
            SellerRepository sellerRepository = new SellerRepository(db);
            var login = sellerRepository.Login("0964524062", "210801");
            Seller seller = login.Seller;
            Console.WriteLine(seller);
            TransactionRepository tr = new TransactionRepository(db, sellerRepository);
            Dictionary<string, int> books = new () {
                { "53c2ae8528d75d572c06ad9d", 1 },
                { "53c2ae8528d75d572c06ad9f", 1 }
            };
            Dictionary<string, int> stationeries = new () {
                { "00ac74ced6ad4425a69ed6d0", 3 },
                { "02caaf41bc3649dab0278175", 2 }
            };
            Transaction res = tr.Create(seller.Id, books, stationeries);

            var result = tr.GetInvoice(res);
            Invoice invoice = result.Bill;
            Console.WriteLine($"{invoice.Id} - {invoice.Date} - {invoice.Seller} - {invoice.Total}");
            invoice.Items.ForEach(b => Console.WriteLine(b));
            var res2 = tr.Add(res);
            if(res2.Success) Console.WriteLine(res2.SuccessMessage);
            else Console.WriteLine(res2.ErrorMessage);
            sellerRepository.Logout();

        }

    }
}
