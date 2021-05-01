using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Models
{
    public class Borrowing
    {
        public string CNP { get; set; }
        public string ISBN { get; set; }
        public string BorrowingStartDate { get; set; }
        public string BorrowingEndDate { get; set; }
        public bool HasBeenReturned { get; set; } = false;
        public double BorrowingPrice { get; set; }
    }
}
