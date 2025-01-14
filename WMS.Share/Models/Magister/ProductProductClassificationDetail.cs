using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Share.Models.Magister
{
    public class ProductProductClassificationDetail:UserUpdate
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "Producto")]
        public long ProductId { get; set; }
        public virtual Product? Product { get; set; }

        [Display(Name = "Clasificación Producto")]
        public long ProductClassificationId { get; set; }
        public virtual ProductClassification? ProductClassification { get; set; }

        [Display(Name = "Clasificación Producto")]
        public long ProductClassificationDetailId { get; set; }
        public virtual ProductClassificationDetail? ProductClassificationDetail { get; set; }

        [NotMapped]
        public List<ProductClassificationDetail>? ListDetail { get; set; }
    }
}
