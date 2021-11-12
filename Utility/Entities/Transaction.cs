using System;
using System.Collections.Generic;

namespace Utility.Entities
{
    public class Transaction
    {
        public string Id { get; set; }
        public string SellerId { get; set; }
        public DateTime CreateDate { get; set; }
        public decimal TotalPrice { get; set; }
        public List<BookSell> Books { get; set; }
        public List<StationerySell> Stationeries { get; set; }

        public Transaction()
        {
            Id = GenerateId();
            CreateDate = DateTime.Now;
            Books = new List<BookSell>();
            Stationeries = new List<StationerySell>();
        }

        public string GenerateId()
        {
            string id = Guid.NewGuid().ToString();
            var tmp = id.Split('-');
            tmp[^1] = tmp[^1].Substring(0, 4);
            id = string.Concat(tmp);
            return id;
        }
    }
}