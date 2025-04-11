using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab5;
using Lab5.Models;
using Lab5.Models.ViewModels;

namespace Lab5.Controllers
{
    public class CustomersController : Controller
    {
        private readonly DealsFinderDbContext _context;

        public CustomersController(DealsFinderDbContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index(int? id)
        {
            var allCustomers = await _context.Customers.ToListAsync();
            ViewBag.AllCustomers = allCustomers;

            if (id == null)
            {
                return View(new CustomerSubscriptionViewModel());
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                return NotFound();
            }

            var subscriptions = await _context.Subscriptions
                .Where(s => s.CustomerId == id)
                .Include(s => s.FoodDeliveryService)
                .Select(s => new FoodDeliveryServiceSubscriptionViewModel
                {
                    FoodDeliveryServiceId = s.ServiceId,
                    Title = s.FoodDeliveryService.Title,
                    IsSubscribed = true
                })
                .ToListAsync();

            var viewModel = new CustomerSubscriptionViewModel
            {
                Customer = customer,
                Subscriptions = subscriptions
            };

            return View(viewModel);
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LastName,FirstName,BirthDate")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LastName,FirstName,BirthDate")] Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
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
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }

        public async Task<IActionResult> EditSubscriptions(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            // Get all food delivery services
            var allServices = await _context.FoodDeliveryServices.ToListAsync();

            // Get customer's current subscriptions
            var currentSubscriptions = await _context.Subscriptions
                .Where(s => s.CustomerId == id)
                .Select(s => s.ServiceId)
                .ToListAsync();

            var viewModel = new CustomerSubscriptionViewModel
            {
                Customer = customer,
                Subscriptions = allServices.Select(s => new FoodDeliveryServiceSubscriptionViewModel
                {
                    FoodDeliveryServiceId = s.Id,
                    Title = s.Title,
                    IsSubscribed = currentSubscriptions.Contains(s.Id)
                })
                .OrderBy(s => s.IsSubscribed) // Not subscribed first
                .ThenBy(s => s.Title)         // Then alphabetically
                .ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddSubscription(int customerId, string foodDeliveryServiceId)
        {
            var subscription = new Subscription
            {
                CustomerId = customerId,
                ServiceId = foodDeliveryServiceId
            };

            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(EditSubscriptions), new { id = customerId });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveSubscription(int customerId, string foodDeliveryServiceId)
        {
            var subscription = await _context.Subscriptions.FindAsync(customerId, foodDeliveryServiceId);
            if (subscription != null)
            {
                _context.Subscriptions.Remove(subscription);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(EditSubscriptions), new { id = customerId });
        }
    }
}
