using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Models
{
    public class BookProductInfo
    {
        public Book Book { get; set; }
        public int Quantity { get; set; } = 1;
        public double BorrowPrice { get; set; }
    }
}
