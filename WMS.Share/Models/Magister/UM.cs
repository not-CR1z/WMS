using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Share.Models.Magister
{
    public class UM:UserUpdate
    {
        [Key]
        public long Id { get; set; }

        [Required(ErrorMessage = "{0} Obligatorio")]
        [MaxLength(10, ErrorMessage = "El campo {0} maximo admite {1} caracteres")]
        [MinLength(1, ErrorMessage = "El campo {0} minimo admite {1} caracteres")]
        [Display(Name = "Codigo")]
        public string Code { get; set; } = null!;

        [Display(Name = "Descripción")]
        [MaxLength(100, ErrorMessage = "El campo {0} maximo admite {1} caracteres")]
        public string Description { get; set; } = null!;

        [Display(Name = "Cantidad Decimales")]
        public int QtyDecimal { get; set; }

        [Precision(18, 2)]
        [Display(Name = "Unidad Empaque")]
        public decimal FactorUnit { get; set; }

        public virtual ICollection<ProductUM>? ProductUMs { get; set; }
    }
}
