using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Share.Models.Magister
{
    public class ProductUM: UserUpdate
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "Producto")]
        public long ProductId { get; set; }
        public virtual Product? Product { get; set; }

        [Display(Name = "Unidad de Medida")]
        public long UMId { get; set; }
        public virtual UM? UM { get; set; }

    }
}
