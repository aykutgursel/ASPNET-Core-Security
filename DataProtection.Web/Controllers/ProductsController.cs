using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataProtection.Web.Models;
using Microsoft.AspNetCore.DataProtection;

namespace DataProtection.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ExampleDbContext _context;
        private readonly IDataProtector _dataProtector;

        public ProductsController(ExampleDbContext context, IDataProtectionProvider dataProtector)
        {
            _context = context;
            _dataProtector = dataProtector.CreateProtector("ProductsController");
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Product.ToListAsync();

            //Limitli protector, aşağıdaki gibi 5 saniye geçerli olmak kaydıtyla işlem yapılabilir
            var timeLimitedProtector = _dataProtector.ToTimeLimitedDataProtector();

            //Products tablosundaki Id değerinin şifrelenmiş halini foreach ile ekliyoruz
            products.ForEach(x =>
            {
                x.EncrypedId = timeLimitedProtector.Protect(x.Id.ToString(), TimeSpan.FromSeconds(5));
            });


            return View(products);
        }

        [HttpPost]
        public IActionResult Index(string searchText)
        {
            //sql injection
            var product = _context.Product.FromSqlRaw
                ("Select * from Product where Name= " + "'" + searchText + "'").ToList();

            return View(product);
        }


        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Süreli hash çözme işlemi
            var timeLimitedProtector = _dataProtector.ToTimeLimitedDataProtector();

            //Gelen hashlenmiş veriyi çözerek db de kayıtlı olan id değerine çeviriyoruz
            var decryptedId = int.Parse(timeLimitedProtector.Unprotect(id));

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == decryptedId);

            product.EncrypedId = _dataProtector.Protect(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Color,ProductCategoryId")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Color,ProductCategoryId")] Product product)
        {
            if (id != product.Id)
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
                    if (!ProductExists(product.Id))
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
            return View(product);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }
    }
}
