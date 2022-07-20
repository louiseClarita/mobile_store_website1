using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using mobile_store_website1.Data;
using mobile_store_website1.Models;

namespace mobile_store_website1.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index(int? idorder)
        {
            var applicationDbContext = _context.Order.Include(o => o.Product).Include(o => o.IdentityUser);
            if (User.IsInRole("Administrator"))
            {
                return View(await applicationDbContext.ToListAsync());
            }

            else
            {
               Order order1 = await _context.Order.FindAsync(idorder);
               Product product1 = await _context.Product.FindAsync(order1.ProductId);
                ViewData["orderdate"] = order1.OrderDate;
                ViewData["orderdesc"] = product1.ProductName + " " + product1.ProductColor  + "-" + product1.ProductYear;
                ViewData["orderstatus"] = order1.OrderStatus;
                ViewData["address"] = order1.OrderCity + " " + order1.OrderStreet + " " + order1.OrderBuilding;
                ViewData["total"] = order1.OrderBalance;

                return View();

            }

        }



        public async Task<IActionResult> ShowSearchResults(string SearchPhrase)
        {

          
                var list = await _context.Order.Where(p => p.OrderDate.Contains(SearchPhrase) || p.OrderCity.Contains(SearchPhrase) || p.OrderDeliveredData.Contains(SearchPhrase) || p.OrderStatus.Contains(SearchPhrase) || p.OrderBalance.Contains(SearchPhrase) || p.Product.ProductName.Contains(SearchPhrase)).Include(p => p.Product).Include(p => p.IdentityUser).ToListAsync();

            return View("index", list);


        }

        public async Task<IActionResult> Index1(string? id)
        {
          //  var applicationDbContext = _context.Order.Include(o => o.Product).Include(o => o.IdentityUser);
            var list =await _context.Order.Include(o => o.Product).Include(o => o.IdentityUser).Where(o => o.IdentityUserId.Equals(id)).ToListAsync();
            return View(list);
            
        }
            // GET: Orders/Details/5
            public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.Product)
                .Include(o => o.IdentityUser)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public async Task<IActionResult> CreateAsync(int? id)
        {
            Product product1 = await _context.Product.FindAsync(id);
            DateTime dt = DateTime.Now;

            string sDate = dt.ToShortDateString();
            ViewData["orderDate"] = sDate;
            ViewData["orderStatus"] = "InProgress";

            ViewData["orderBalance"] = product1.ProductPrice;
            ViewData["orderdesc"] = product1.ProductName +" "+ product1.ProductColor +" "+ product1.ProductYear;
            ViewData["ProductId"] = new SelectList(_context.Set<Product>(), "ProductId", "ProductId");
            ViewData["IdentityUserId"] = new SelectList(_context.Set<IdentityUser>(), "Id", "Id");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? id,[Bind("OrderId,OrderDate,OrderBalance,OrderStreet,OrderCity,OrderBuilding,OrderStatus,OrderDeliveredData,ProductId,IdentityUserId")] Order order)
        {
            if (ModelState.IsValid)
            {
                if (id == null || _context.Product == null)
                {
                    return NotFound();
                }
                _context.Add(order);
                await _context.SaveChangesAsync();

                Product product1 = await _context.Product.FindAsync(id);
                //   int prodavailable = product1.ProductAvailability.GetValueOrDefault() - 1;
                product1.ProductAvailability = product1.ProductAvailability - 1;
                try
                {
                    _context.Update(product1);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product1.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                var ids = order.OrderId;
                //    if (User.IsInRole("Administrator")) { 
                // return RedirectToAction(nameof(Index));
                //  }
                //  Details(order.OrderId);
                //   return View(v);
               return RedirectToAction("Index", new { idorder = ids });

                
                //   return View("~/Views/FolderName/ViewName.aspx");

            }
            ViewData["ProductId"] = new SelectList(_context.Set<Product>(), "ProductId", "ProductId", order.ProductId);
            ViewData["IdentityUserId"] = new SelectList(_context.Set<IdentityUser>(), "Id", "Id", order.IdentityUserId);
            return View(order);
        }



        // GET: Orders/Create
        public IActionResult Create1()
        {
            return View();
        }


        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }


            var prod1 = await _context.Product.FindAsync(order.ProductId);
            var use  = await _context.Users.FindAsync(order.IdentityUserId);
            ViewData["ProductName"] = prod1.ProductName;
            ViewData["ProductId"] = order.ProductId;
            ViewData["IdentityUserName"] = use.UserName;
            ViewData["IdentityUserId"] = order.IdentityUserId;
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,OrderDate,OrderBalance,OrderStreet,OrderCity,OrderBuilding,OrderStatus,OrderDeliveredData,ProductId,IdentityUserId")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            Product p = await _context.Product.FindAsync(order.ProductId);

            ViewData["ProductName"] = order.Product.ProductName;
            ViewData["ProductId"] = order.ProductId;
            ViewData["IdentityUserName"] = order.IdentityUser.UserName;
            ViewData["IdentityUserId"] = order.IdentityUserId;
            return View(order);
        }
        public async Task<IActionResult> Received(int? id)
        {
            var order = await _context.Order.FindAsync(id);
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    DateTime dt = DateTime.Now;

                    string sDate = dt.ToShortDateString();
                    order.OrderStatus = "Delivered";
                    order.OrderDeliveredData = sDate;
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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

            return View(order);
        }
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Received(int id)
        {
            var order = await _context.Order.FindAsync(id);
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    order.OrderStatus = "Delivered";

                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
     
            return View(order);
        }
        */
        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.Product)
                .Include(o => o.IdentityUser)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Order == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Order'  is null.");
            }
            var order = await _context.Order.FindAsync(id);
            if (order != null)
            {
                _context.Order.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return (_context.Order?.Any(e => e.OrderId == id)).GetValueOrDefault();
        }
        private bool ProductExists(int id)
        {
            return (_context.Product?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }
    }

}
