using System;
using System.Collections.Generic;

namespace Utility.Entities
{
    public class Invoice
    {
        public string Id { get; set; }
        public string Seller { get; set; }
        public DateTime Date { get; set; }
        public List<Item> Items { get; set; }
        public decimal Total { get; set; }

    }

    public class Item
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Unit {get; set; }

        public override string ToString()
        {
            return $"{Id} - {Quantity} - {Price} - {Unit}";
        }
    }

    public class BookSell : Item
    {
        
    }

    public class StationerySell : Item
    {
        
    }
}