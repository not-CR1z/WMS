using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Share.Models.Magister
{
    public class ProductClassificationDetail:UserUpdate
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "Clasificación Producto")]
        public long ProductClassificationId { get; set; }
        public virtual ProductClassification? ProductClassification { get; set; }

        [Display(Name = "Nombre")]
        [MaxLength(100, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Name { get; set; } = null!;

        public ICollection<ProductProductClassificationDetail>? ProductProductClassificationDetails { get; set; }
    }
}
