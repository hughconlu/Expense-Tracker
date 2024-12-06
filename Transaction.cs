using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ExpenseTracker.Models
{
    public class Transaction
    {

        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public bool IsIncome { get; set; }

        public IncomeCategory? IncomeCategory { get; set; }
        public ExpenseCategory? ExpenseCategory { get; set; }
    }

    public enum ExpenseCategory
    {
        Food,
        Shopping,
        Utilities,
        Transportation,
        Entertainment
    }

    public enum IncomeCategory
    {
        Salary,
        Business,
        Other
    }
}
