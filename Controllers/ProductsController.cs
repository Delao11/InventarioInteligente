using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventarioInteligente.Data;
using InventarioInteligente.Models;

namespace InventarioInteligente.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private const int LowStockThreshold = 5;

        public ProductsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /Products/Index  (listado + búsqueda)
        public async Task<IActionResult> Index(string? search)
        {
            var query = _db.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(p => p.Name.Contains(search));
                ViewData["Search"] = search;
            }

            var list = await query
                .OrderBy(p => p.Name)
                .ToListAsync();

            return View(list);
        }

        // GET: /Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            product.CreatedAt = DateTime.UtcNow;

            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Products/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: /Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product input)
        {
            if (id != input.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return View(input);
            }

            var product = await _db.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            product.Name = input.Name;
            product.Description = input.Description;
            product.Price = input.Price;
            product.Stock = input.Stock;
            product.UpdatedAt = DateTime.UtcNow;

            _db.Products.Update(product);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Products/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: /Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Products/Statistics
        public async Task<IActionResult> Statistics()
        {
            var products = await _db.Products.ToListAsync();

            var orderedByPriceDesc = products
                .OrderByDescending(p => p.Price)
                .ToList();

            var averagePrice = products.Any()
                ? products.Average(p => p.Price)
                : 0m;

            var totalInventoryValue = products.Sum(p => p.Price * p.Stock);

            var lowStock = products
                .Where(p => p.Stock < LowStockThreshold)
                .ToList();

            var vm = new StatisticsViewModel
            {
                OrderedByPriceDesc = orderedByPriceDesc,
                AveragePrice = averagePrice,
                TotalInventoryValue = totalInventoryValue,
                LowStockProducts = lowStock
            };

            return View(vm);
        }
    }
}
