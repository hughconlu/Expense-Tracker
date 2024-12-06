using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Models;
using Newtonsoft.Json;

namespace ExpenseTracker.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ExpenseTrackerContext _context;

        public TransactionsController(ExpenseTrackerContext context)
        {
            _context = context;
        }

        //GET : Transactions/CreateIncome
        public IActionResult CreateIncome()
        {
            return View();
        }

        //POST : Transactions/CreateIncome
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateIncome(Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                transaction.IsIncome = true;
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(transaction);
        }

        //GET : Transactions/CreateExpense
        public IActionResult CreateExpense()
        {
            return View();
        }

        //POST : Transactions/CreateExpense
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateExpense(Transaction transaction)
        {
            if(ModelState.IsValid)
            {
                transaction.IsIncome = false; 
                _context.Add(transaction); ;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(transaction);
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            var allTransactions = await _context.Transactions.ToListAsync();

            //Total Income, Total Expenses, and Balance Calculations
            ViewBag.TotalIncome = allTransactions.Where(t => t.IsIncome).Sum(t => t.Amount);
            ViewBag.TotalExpenses = allTransactions.Where(t => !t.IsIncome).Sum(t => t.Amount);
            ViewBag.Balance = ViewBag.TotalIncome - ViewBag.TotalExpenses;

            var recentTransactions = await _context.Transactions.OrderByDescending(t => t.Date).Take(5).ToListAsync();

            //Pie Chart
            var expenses = _context.Transactions
              .Where(t => !t.IsIncome)
              .GroupBy(t => t.Category)
              .Select(g => new
            {
                Category = g.Key,
                Total = g.Sum(t => t.Amount)
            })
             .ToList();
            ViewBag.ExpenseData = JsonConvert.SerializeObject(expenses);

            //Bar Graph for Monthly Income and Expenses
            var allMonths = Enumerable.Range(1, 12).ToList();
            var incomeData = await _context.Transactions
                .Where (t => t.IsIncome)
                .GroupBy(t => t.Date.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    TotalIncome = g.Sum(t=> t.Amount)
                }).ToListAsync();

            var expenseData = await _context.Transactions
               .Where(t => !t.IsIncome)
               .GroupBy(t => t.Date.Month)
               .Select(g => new
               {
                   Month = g.Key,
                   TotalExpense = g.Sum(t => t.Amount)
               }).ToListAsync();

            var incomeVsExpense = allMonths.Select(month => new
                                  {
                                      Month = month,
                                      Income = incomeData.FirstOrDefault(i => i.Month == month)?.TotalIncome ?? 0,
                                      Expense = expenseData.FirstOrDefault(i => i.Month == month)?.TotalExpense ?? 0
                                  }).ToList();
            ViewBag.IncomeVsExpenseData = JsonConvert.SerializeObject(incomeVsExpense);

            return View(recentTransactions);

        }

        //Overall Transaction History
        public IActionResult OverallTransactionHistory()
        {
            var transactions = _context.Transactions.Select(t => new TransactionViewModel
            {
                Description = t.Description,
                Id = t.Id,
                Date = t.Date,
                Amount = t.Amount,
                Category = t.Category,
                Type = t.IsIncome ? "Income" : "Expense"
            }).OrderBy(t => t.Date).ToList();

            return View(transactions);
        }

        //Income History
        public async Task<IActionResult> IncomeHistory()
        {
            var incomeTransactions = await _context.Transactions.Where(t => t.IsIncome).OrderBy(t => t.Date).ToListAsync();
            return View(incomeTransactions);
        }

        //Expense History
        public async Task<IActionResult> ExpenseHistory()
        {

            var expenseTransactions = await _context.Transactions.Where(t => !t.IsIncome).OrderBy(t => t.Date).ToListAsync();
            return View(expenseTransactions);

        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        
        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Amount,Category,Date,IsIncome")] Transaction transaction)
        {
            if (id != transaction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.Id == id);
        }

    }

 }   