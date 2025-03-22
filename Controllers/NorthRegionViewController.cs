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
    public class northRegionController : Controller
    {
        private readonly ProductDbContext _context;
        private readonly IWebHostEnvironment environment;

        public northRegionController(ProductDbContext ProductDbContext, IWebHostEnvironment environment)
        {
            this._context = ProductDbContext;
            this.environment = environment;
        }

        // GET: NorthRegion
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
        {
            var query = _context.NorthRegion
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
                if (string.IsNullOrEmpty(product.ImageFilename))
                {
                    product.ImageFilename = "NoData.jpg";
                }
            }

            // Create a paginated list
            var paginatedList = new PagedList<NorthRegionViewModel>(productList, page, pageSize, totalRecords);

            return View(paginatedList);
        }

        // GET:northRegionController/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(NorthRegionViewModel addNorthRegionViewModel, IFormFile ImageFile)
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

                var NorthRegionViewModel = new NorthRegionViewModel()
                {
                    Id = addNorthRegionViewModel.Id,
                    Name = addNorthRegionViewModel.Name,
                    Description = addNorthRegionViewModel.Description,
                    Price = addNorthRegionViewModel.Price,
                    ExpiredDate = DateTime.Now.AddDays(7),
                    ImageFilename = strImageFile,
                    Source = addNorthRegionViewModel.Source
                };

                await _context.AddAsync(NorthRegionViewModel);
                await _context.SaveChangesAsync();
                TempData["successMessage"] = $"New Product was Created ({addNorthRegionViewModel.Name}).";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = $"Message: {ex.Message}{Environment.NewLine}Stack Trace:{Environment.NewLine}{ex.StackTrace}";
                return View();
            }
        }

        // GET:northRegionController/Edit/4
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var product = await _context.NorthRegion.SingleOrDefaultAsync(f => f.Id == id);
                if (product != null)
                {
                    TempData["ImageFilePath"] = "/images/" + product.ImageFilename;
                    TempData["Source"] = product.Source ?? "Unknown";
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
        public async Task<IActionResult> Edit(NorthRegionViewModel editNorthRegionViewModel, IFormFile ImageFile)
        {
            try
            {
                var product = await _context.NorthRegion.SingleOrDefaultAsync(f => f.Id == editNorthRegionViewModel.Id);
                if (product == null)
                {
                    TempData["errorMessage"] = $"Product Not Found with Id {editNorthRegionViewModel.Id}";
                    return View("No data");
                }
                else
                {
                    product.Name = editNorthRegionViewModel.Name;
                    product.Description = editNorthRegionViewModel.Description;
                    product.Price = editNorthRegionViewModel.Price;
                    product.ExpiredDate = product.ExpiredDate;
                    product.Source = editNorthRegionViewModel.Source;

                    if (ImageFile != null)
                    {
                        string strDateTime = DateTime.Now.ToString("yyyyMMsHHmmss", CultureInfo.InvariantCulture);
                        string strImageFile = strDateTime + "_" + ImageFile.FileName;
                        string ImageFullPath = environment.WebRootPath + "/images/" + strImageFile;
                        using (var fileStream = new FileStream(ImageFullPath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(fileStream);
                        }
                        product.ImageFilename = strImageFile;
                    }
                    else
                    {
                        product.ImageFilename = editNorthRegionViewModel.ImageFilename;
                    }

                    await _context.SaveChangesAsync();
                    TempData["successMessage"] = $"Product Record was Edited ({editNorthRegionViewModel.Name}).";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message + "<br/>" + ex.StackTrace;
                return View();
            }
        }

        // GET:northRegionController/Delete/4
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var product = await _context.NorthRegion.SingleOrDefaultAsync(f => f.Id == id);
                if (product != null)
                {
                    TempData["ImageFilePath"] = "/images/" + product.ImageFilename;
                    TempData["Source"] = product.Source ?? "Unknown";
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
        public async Task<IActionResult> Delete(NorthRegionViewModel deleteNorthRegionViewModel)
        {
            try
            {
                var product = await _context.NorthRegion.SingleOrDefaultAsync(f => f.Id == deleteNorthRegionViewModel.Id);
                if (product == null)
                {
                    TempData["errorMessage"] = $"Product Not Found with Id {deleteNorthRegionViewModel.Id}";
                    return View("No data");
                }
                else
                {
                    _context.NorthRegion.Remove(product);
                    await _context.SaveChangesAsync();
                    TempData["successMessage"] = $"Product Record was Deleted ({deleteNorthRegionViewModel.Name}).";
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
