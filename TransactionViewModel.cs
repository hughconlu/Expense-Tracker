using System;

namespace ExpenseTracker.Models
{
    public class TransactionViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }

        public string Type { get; set; } // Income or Expense
    }
}
