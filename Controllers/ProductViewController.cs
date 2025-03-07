using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ProductMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ProductMVC.Controllers {
    //[Route("[controller]")]
    [Route("[controller]/{action=Index}/{Id=0}")]
    public class productController : Controller {
        private readonly ProductDbContext _context;
        private readonly IWebHostEnvironment environment;

        public productController(ProductDbContext ProductDbContext, IWebHostEnvironment environment) {
            this._context = ProductDbContext;
            this.environment = environment;
        }

        // Get:productController
        [HttpGet]
        public async Task<IActionResult> Index() {
            var productList = await _context.NorthRegion.ToListAsync();
            productList = productList.OrderByDescending(x => x.Id).ToList();
            foreach (var product in productList) {
                if (string.IsNullOrEmpty(product.ImageFilename)) {
                    product.ImageFilename = "NoData.jpg";
                }
            }
            return View(productList);
        }

        // GET:productController/Create
        [HttpGet]
        public IActionResult Create() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel addProductViewModel, IFormFile ImageFile) {
            try {
                string strImageFile = "NoData.jpg";
                if (ImageFile != null) {
                    string strDateTime = DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                    strImageFile = strDateTime + "_" + ImageFile.FileName;
                    string ImageFullPath = environment.WebRootPath + "/images/" + strImageFile;
                    using (var fileStream = new FileStream(ImageFullPath, FileMode.Create)) {
                        await ImageFile.CopyToAsync(fileStream);
                    }
                }

                var ProductViewModel = new ProductViewModel() {
                    Id = addProductViewModel.Id,
                    Name = addProductViewModel.Name,
                    Description = addProductViewModel.Description,
                    Price = addProductViewModel.Price,
                    ExpiredDate = DateTime.Now.AddDays(7),
                    ImageFilename = strImageFile,
                    Source = addProductViewModel.Source
                };

                await _context.AddAsync(ProductViewModel);
                await _context.SaveChangesAsync();
                TempData["successMessage"] = $"New Product was Created ({addProductViewModel.Name}).";
                return RedirectToAction(nameof(Index));
            } catch (Exception ex) {
                TempData["errorMessage"] = $"Message: {ex.Message}{Environment.NewLine}Stack Trace:{Environment.NewLine}{ex.StackTrace}";
                return View();
            }
        }

        // GET:productController/Edit/4
        [HttpGet]
        public async Task<IActionResult> Edit(int id) {
            try {
                var product = await _context.NorthRegion.SingleOrDefaultAsync(f => f.Id == id);
                if (product != null) {
                    TempData["ImageFilePath"] = "/images/" + product.ImageFilename;
                    TempData["Source"] = product.Source ?? "Unknown";
                    return View(product);
                }
                TempData["errorMessage"] = "Product not found.";
                return View("Error");
            } catch (Exception ex) {
                TempData["errorMessage"] = ex.Message + "<br/>" + ex.StackTrace;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductViewModel editProductViewModel, IFormFile ImageFile) {
            try {
                var product = await _context.NorthRegion.SingleOrDefaultAsync(f => f.Id == editProductViewModel.Id);
                if (product == null) {
                    TempData["errorMessage"] = $"Product Not Found with Id {editProductViewModel.Id}";
                    return View("No data");
                } else {
                    product.Name = editProductViewModel.Name;
                    product.Description = editProductViewModel.Description;
                    product.Price = editProductViewModel.Price;
                    product.ExpiredDate = product.ExpiredDate;
                    product.Source = editProductViewModel.Source;

                    if (ImageFile != null) {
                        string strDateTime = DateTime.Now.ToString("yyyyMMsHHmmss", CultureInfo.InvariantCulture);
                        string strImageFile = strDateTime + "_" + ImageFile.FileName;
                        string ImageFullPath = environment.WebRootPath + "/images/" + strImageFile;
                        using (var fileStream = new FileStream(ImageFullPath, FileMode.Create)) {
                            await ImageFile.CopyToAsync(fileStream);
                        }
                        product.ImageFilename = strImageFile;
                    } else {
                        product.ImageFilename = editProductViewModel.ImageFilename;
                    }

                    await _context.SaveChangesAsync();
                    TempData["successMessage"] = $"Product Record was Edited ({editProductViewModel.Name}).";
                    return RedirectToAction(nameof(Index));
                }
            } catch (Exception ex) {
                TempData["errorMessage"] = ex.Message + "<br/>" + ex.StackTrace;
                return View();
            }
        }

        // GET:productController/Delete/4
        [HttpGet]
        public async Task<IActionResult> Delete(int id) {
            try {
                var product = await _context.NorthRegion.SingleOrDefaultAsync(f => f.Id == id);
                if (product != null) {
                    TempData["ImageFilePath"] = "/images/" + product.ImageFilename;
                    TempData["Source"] = product.Source ?? "Unknown";
                    return View(product);
                }
                TempData["errorMessage"] = "Product not found.";
                return View("Error");
            } catch (Exception ex) {
                TempData["errorMessage"] = ex.Message + "<br/>" + ex.StackTrace;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ProductViewModel deleteProductViewModel) {
            try {
                var product = await _context.NorthRegion.SingleOrDefaultAsync(f => f.Id == deleteProductViewModel.Id);
                if (product == null) {
                    TempData["errorMessage"] = $"Product Not Found with Id {deleteProductViewModel.Id}";
                    return View("No data");
                } else {
                    _context.NorthRegion.Remove(product);
                    await _context.SaveChangesAsync();
                    TempData["successMessage"] = $"Product Record was Deleted ({deleteProductViewModel.Name}).";
                    return RedirectToAction(nameof(Index));
                }
            } catch (Exception ex) {
                TempData["errorMessage"] = ex.Message + "<br/>" + ex.StackTrace;
                return View();
            }
        }
    }
}
