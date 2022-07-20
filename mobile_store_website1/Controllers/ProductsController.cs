using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using mobile_store_website1.Models;
using mobile_store_website1.Data;
using System.Collections;

namespace mobile_store_website1.Controllers
{
    public class ProductsController : Controller
    {
        OrdersController ordersController;
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            ordersController = new OrdersController(_context);
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Product.Include(p => p.Model);
          
                return View(await applicationDbContext.ToListAsync());
        }


        [HttpPost]
        public async Task<IActionResult> Index(int hfMin, int hfMax)
        {
            return View("index", await _context.Product.Where(p => p.ProductPrice <= hfMax && p.ProductPrice >= hfMin).ToListAsync());
        }


        // GET: Products/ShowSearchForm
        public async Task<IActionResult> ShowSearchForm()
        {

            return View(await _context.Product.ToListAsync());
        }

        // Post: Products/ShowSearchResults
        public async Task<IActionResult> ShowSearchResults(string SearchPhrase)
        {
          
            return View("index", await _context.Product.Where( p => p.ProductName.Contains(SearchPhrase) || p.ProductYear.Contains(SearchPhrase) || p.ProductColor.Contains(SearchPhrase)).ToListAsync());


        }
     /*   public async Task<IActionResult> ShowSearchResults2(string SearchPhrase)
        {
            //searching prices
            var listofProducts = new List<Product>();
         //   if (SearchPhrase.contains("$")) { var list2 = _context.Product.Where(p => p.ProductPrice.Contains(SearchPhrase)).ToListAsync();
          //      return View("index", await list2);
        //    }

            var list = _context.Product.Where(p => p.ProductName.Contains(SearchPhrase)).ToListAsync();
            //var list2 = _context.Product.Where(p => p.ProductPrice.Contains(SearchPhrase)).ToListAsync();
            var list3 = _context.Product.Where(p => p.ProductYear.Contains(SearchPhrase)).ToListAsync();

            return View("index", await list2);

        }
     */
        public async Task<IActionResult> ShowSearchResults3(string SearchPhrase)
        {//searching years
            var listofProducts = new List<Product>();


            var list = _context.Product.Where(p => p.ProductName.Contains(SearchPhrase)).ToListAsync();
           // var list2 = _context.Product.Where(p => p.ProductPrice.Contains(SearchPhrase)).ToListAsync();
            var list3 = _context.Product.Where(p => p.ProductYear.Contains(SearchPhrase)).ToListAsync();

            return View("index", await list3);


        }







        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.Model)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["ModelId"] = new SelectList(_context.Model, "ModelId", "ModelId");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ProductColor,ProductYear,ProductPrice,ProductAvailability,ProductImage,ProductImagePath,ModelId")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ModelId"] = new SelectList(_context.Model, "ModelId", "ModelId", product.ModelId);
            return View(product);
        }




        // GET: Products/Create
        [Authorize]
        public IActionResult CreateOrder()
        {


            return View();
        }




        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["ModelId"] = new SelectList(_context.Model, "ModelId", "ModelId", product.ModelId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ProductColor,ProductYear,ProductPrice,ProductAvailability,ProductImage,ProductImagePath,ModelId")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            ViewData["ModelId"] = new SelectList(_context.Model, "ModelId", "ModelId", product.ModelId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.Model)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Product == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Product'  is null.");
            }
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return (_context.Product?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }






    }
}
