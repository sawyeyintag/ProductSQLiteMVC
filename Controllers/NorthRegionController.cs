using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductSQLiteMVC.Models;

namespace ProductSQLiteMVC.Controllers
{
    // [Route("[controller]")]
    public class NorthRegionController : Controller
    {
        private readonly ProductDbContext _context;

        public NorthRegionController(ProductDbContext ProductDbContext)
        {
            _context = ProductDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var productList = await _context.Product.ToListAsync();
            // Console.WriteLine(productList);
            return View(productList);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(NorthRegionViewModel addProductViewModel)
        {
            try
            {
                NorthRegionViewModel ProductViewModel = new NorthRegionViewModel()
                {
                    Id = addProductViewModel.Id,
                    Name = addProductViewModel.Name,
                    Description = addProductViewModel.Description,
                    Price = addProductViewModel.Price,
                };
                await _context.AddAsync(ProductViewModel);
                await _context.SaveChangesAsync();
                TempData["successMessage"] = $"New Product was Created ({addProductViewModel.Name}.)";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message + "<br/>" + ex.StackTrace;
                return View();
            }
        }

        // GET:ProductController/Edit/4
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var Product = await _context.Product.SingleOrDefaultAsync(f => f.Id == id);
                return View(Product);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message + "<br/>" + ex.StackTrace;
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(NorthRegionViewModel NorthRegionViewModel)
        {
            try
            {
                var Product = await _context.Product.SingleOrDefaultAsync(f => f.Id == NorthRegionViewModel.Id);
                if (Product == null)
                {
                    return View("No data");
                }
                else
                {
                    Product.Name = NorthRegionViewModel.Name;
                    Product.Description = NorthRegionViewModel.Description;
                    Product.Price = NorthRegionViewModel.Price;
                    await _context.SaveChangesAsync();
                    TempData["successMessage"] = $"Product Record was Edited ({NorthRegionViewModel.Name}).";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message + "<br/>" + ex.StackTrace;
                return View();
            }
        }
        // GET:ProductController/Delete/4
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var Product = await _context.Product.SingleOrDefaultAsync(f => f.Id == id);
                return View(Product);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message + "<br/>" + ex.StackTrace;
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete(NorthRegionViewModel ProductViewModel)
        {
            try
            {
                var Product = await _context.Product.SingleOrDefaultAsync(f => f.Id == ProductViewModel.Id);
                if (Product == null)
                {
                    return View("No data");
                }
                else
                {
                    _context.Product.Remove(Product);
                    await _context.SaveChangesAsync();
                    TempData["successMessage"] = $"Product Record was Deleted ({ProductViewModel.Name}).";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message + "<br/>" + ex.StackTrace;
                return View();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}