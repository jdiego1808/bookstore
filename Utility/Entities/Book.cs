using System;
using System.Collections.Generic;

namespace Utility.Entities
{
    public class Book
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Isbn { get; set; }
        public int PageCount { get; set; }
        public DateTime PublishedDate { get; set; }
        public string ImagelUrl { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public List<string> Authors { get; set; }        
        public List<string> Categories { get; set; }

        public override string ToString()
        {            
            return ($"{Id} - {Title} - {PageCount} - {PublishedDate} - {Price} - {Quantity} - {Status}");
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