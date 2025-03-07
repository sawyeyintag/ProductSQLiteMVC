using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProductMVC.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public string? Price { get; set; }
        [Column("ExpiredDate")]
        [DisplayName("Expired Date")]
        public DateTime ExpiredDate { get; set; } = DateTime.Now.AddDays(7);
        [Column("ImageFilename")]
        [DisplayName("Image")]
        public string? ImageFilename { get; set;}
        [Column("Source")]
        [DisplayName("Source")]
        public string? Source { get; set;}

    }
}
