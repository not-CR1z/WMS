using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Share.Models.Location;

namespace WMS.Share.Models.Magister
{
    public class Product:UserUpdate
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "Tipo Producto")]
        public long ProductTypeId { get; set; }
        public virtual ProductType? ProductType { get; set; }

        [Required(ErrorMessage = "{0} Obligatorio")]
        [MaxLength(20, ErrorMessage = "El campo {0} maximo admite {1} caracteres")]
        [MinLength(1, ErrorMessage = "El campo {0} minimo admite {1} caracteres")]
        [Display(Name = "Referencia")]
        public string Reference { get; set; } = null!;

        [Required(ErrorMessage = "{0} Obligatorio")]
        [MaxLength(100, ErrorMessage = "El campo {0} maximo admite {1} caracteres")]
        [MinLength(1, ErrorMessage = "El campo {0} minimo admite {1} caracteres")]
        [Display(Name = "Descripción")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "{0} Obligatorio")]
        [MaxLength(20, ErrorMessage = "El campo {0} maximo admite {1} caracteres")]
        [Display(Name = "Codigo Externo")]
        public string? ExternalCode { get; set; }

        [Display(Name = "Bajo Llave")]
        public bool IsKey { get; set; }

        [Display(Name = "Fuera Dimension")]
        public bool Fdimen { get; set; }

        [Display(Name = "Maneja Lote")]
        public bool WithLot { get; set; }

        [Display(Name = "Maneja Serial")]
        public bool WithSerial { get; set; }

        [Display(Name = "Largo")]
        public decimal Length { get; set; }

        [Display(Name = "Ancho")]
        public decimal Width { get; set; }

        [Display(Name = "Alto")]
        public decimal Height { get; set; }

        [Display(Name = "Volumen")]
        public decimal Volume
        {
            get
            {
                return Length * Width * Height;
            }
        }

        [Display(Name = "Peso")]
        public decimal Weight { get; set; }

        [Display(Name = "Activo")]
        public bool Active { get; set; }

        public ICollection<ProductProductClassificationDetail>? ProductProductClassificationDetails { get; set; }
        public ICollection<ProductUM>? ProductUMs { get; set; }
    }
}
