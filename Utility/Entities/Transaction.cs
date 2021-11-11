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
            CreateDate = DateTime.Now;
        }
    }
}