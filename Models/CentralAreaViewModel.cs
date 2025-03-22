using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProductMVC.Models
{

    public class CentralAreaViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public float BuyingPrice { get; set; } = 0;
        public string Supplier { get; set; } = "";
        public string? PictureFileName { get; set; }
        // [Column("PictureFileName")]
        // [DisplayName("Image")
        public DateTime ManufacturingDate { get; set; } = DateTime.Now.AddDays(1);
        public DateTime PurchasingDate { get; set; } = DateTime.Now.AddDays(30);
        public DateTime ExpirationDate { get; set; } = DateTime.Now.AddDays(90);
    }
}
