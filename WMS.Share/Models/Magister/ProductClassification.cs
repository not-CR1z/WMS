using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Share.Models.Location;

namespace WMS.Share.Models.Magister
{
    public class ProductClassification:UserUpdate
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "Nombre")]
        [MaxLength(100, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Name { get; set; } = null!;

        public ICollection<ProductClassificationDetail>? ProductClassificationDetails { get; set; }

        [Display(Name = "Clasificaciones")]
        public int ClassificatioNumber => ProductClassificationDetails == null || ProductClassificationDetails.Count == 0 ? 0 : ProductClassificationDetails.Count;

    }
}
