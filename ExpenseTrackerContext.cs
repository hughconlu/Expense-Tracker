using Microsoft.EntityFrameworkCore;
namespace ExpenseTracker.Models
{
    public class ExpenseTrackerContext : DbContext
    {

        public ExpenseTrackerContext(DbContextOptions<ExpenseTrackerContext> options) :base(options) { }
    
        public DbSet<Transaction> Transactions { get; set; }
    }
}
