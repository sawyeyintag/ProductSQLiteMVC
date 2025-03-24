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

namespace ProductMVC.Controllers
{
    //[Route("[controller]")]
    [Route("[controller]/{action=Index}/{Id=0}")]
    public class CentralAreaController : Controller
    {
        private readonly ProductDbContext _context;
        private readonly IWebHostEnvironment environment;

        public CentralAreaController(ProductDbContext ProductDbContext, IWebHostEnvironment environment)
        {
            this._context = ProductDbContext;
            this.environment = environment;
        }

        // GET: CentralArea
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 2)
        {
            var query = _context.CentralArea
                .OrderByDescending(x => x.Id) // Sort in database
                .AsQueryable();

            // Get total count before applying pagination
            int totalRecords = await query.CountAsync();

            // Apply pagination
            var productList = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Ensure all products have a valid image filename
            foreach (var product in productList)
            {
                if (string.IsNullOrEmpty(product.PictureFileName))
                {
                    product.PictureFileName = "NoData.jpg";
                }
            }

            // Create a paginated list
            var paginatedList = new PagedList<CentralAreaViewModel>(productList, page, pageSize, totalRecords);

            return View(paginatedList);
        }

        // GET:centralAreaController/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CentralAreaViewModel addCentralAreaViewModel, IFormFile ImageFile)
        {
            try
            {
                string strImageFile = "NoData.jpg";
                if (ImageFile != null)
                {
                    string strDateTime = DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                    strImageFile = strDateTime + "_" + ImageFile.FileName;
                    string ImageFullPath = environment.WebRootPath + "/images/" + strImageFile;
                    using (var fileStream = new FileStream(ImageFullPath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }
                }

                var CentralAreaViewModel = new CentralAreaViewModel()
                {
                    Id = addCentralAreaViewModel.Id,
                    Name = addCentralAreaViewModel.Name,
                    Description = addCentralAreaViewModel.Description,
                    BuyingPrice = addCentralAreaViewModel.BuyingPrice,
                    Supplier = addCentralAreaViewModel.Supplier,
                    ManufacturingDate = DateTime.Now.AddDays(1),
                    PurchasingDate = DateTime.Now.AddDays(30),
                    ExpirationDate = DateTime.Now.AddDays(90),
                    PictureFileName = strImageFile,
                };

                await _context.AddAsync(CentralAreaViewModel);
                await _context.SaveChangesAsync();
                TempData["successMessage"] = $"New Product was Created ({addCentralAreaViewModel.Name}).";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = $"Message: {ex.Message}{Environment.NewLine}Stack Trace:{Environment.NewLine}{ex.StackTrace}";
                return View();
            }
        }

        // GET:centralAreaController/Edit/4
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var product = await _context.CentralArea.SingleOrDefaultAsync(f => f.Id == id);
                if (product != null)
                {
                    TempData["ImageFilePath"] = "/images/" + product.PictureFileName;
                    TempData["Source"] = product.PictureFileName ?? "Unknown";
                    return View(product);
                }
                TempData["errorMessage"] = "Product not found.";
                return View("Error");
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message + "<br/>" + ex.StackTrace;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CentralAreaViewModel editCentralAreaViewModel, IFormFile ImageFile)
        {
            try
            {
                var product = await _context.CentralArea.SingleOrDefaultAsync(f => f.Id == editCentralAreaViewModel.Id);
                if (product == null)
                {
                    TempData["errorMessage"] = $"Product Not Found with Id {editCentralAreaViewModel.Id}";
                    return View("No data");
                }
                else
                {
                    product.Name = editCentralAreaViewModel.Name;
                    product.Description = editCentralAreaViewModel.Description;
                    product.BuyingPrice = editCentralAreaViewModel.BuyingPrice;
                    product.Supplier = editCentralAreaViewModel.Supplier;
                    product.ManufacturingDate = DateTime.Now.AddDays(7);
                    product.PurchasingDate = DateTime.Now.AddDays(30);
                    product.ExpirationDate = DateTime.Now.AddDays(90);
                    if (ImageFile != null)
                    {
                        string strDateTime = DateTime.Now.ToString("yyyyMMsHHmmss", CultureInfo.InvariantCulture);
                        string strImageFile = strDateTime + "_" + ImageFile.FileName;
                        string ImageFullPath = environment.WebRootPath + "/images/" + strImageFile;
                        using (var fileStream = new FileStream(ImageFullPath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(fileStream);
                        }
                        product.PictureFileName = strImageFile;
                    }
                    else
                    {
                        product.PictureFileName = editCentralAreaViewModel.PictureFileName;
                    }

                    await _context.SaveChangesAsync();
                    TempData["successMessage"] = $"Product Record was Edited ({editCentralAreaViewModel.Name}).";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message + "<br/>" + ex.StackTrace;
                return View();
            }
        }

        // GET:centralAreaController/Delete/4
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var product = await _context.CentralArea.SingleOrDefaultAsync(f => f.Id == id);
                if (product != null)
                {
                    TempData["ImageFilePath"] = "/images/" + product.PictureFileName;
                    TempData["Source"] = product.PictureFileName ?? "Unknown";
                    return View(product);
                }
                TempData["errorMessage"] = "Product not found.";
                return View("Error");
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message + "<br/>" + ex.StackTrace;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(CentralAreaViewModel deleteCentralAreaViewModel)
        {
            try
            {
                var product = await _context.CentralArea.SingleOrDefaultAsync(f => f.Id == deleteCentralAreaViewModel.Id);
                if (product == null)
                {
                    TempData["errorMessage"] = $"Product Not Found with Id {deleteCentralAreaViewModel.Id}";
                    return View("No data");
                }
                else
                {
                    _context.CentralArea.Remove(product);
                    await _context.SaveChangesAsync();
                    TempData["successMessage"] = $"Product Record was Deleted ({deleteCentralAreaViewModel.Name}).";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message + "<br/>" + ex.StackTrace;
                return View();
            }
        }
    }
}
