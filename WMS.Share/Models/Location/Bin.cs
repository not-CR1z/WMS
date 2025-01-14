using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Share.Models.Magister;

namespace WMS.Share.Models.Location
{
    public class Bin : UserUpdate
    {
        [Key]
        public long Id { get; set; }

        [Required(ErrorMessage = "El campo {0} Es Requerido")]
        [Display(Name = "Sub-Bodega")]
        public long SubWineryId { get; set; }

        [Display(Name = "Tipo Ubicación")]
        public long BinTypeId { get; set; }

        [Required(ErrorMessage = "{0} Obligatorio")]
        [Display(Name = "Codigo ABC")]
        [MaxLength(1, ErrorMessage = "El campo {0} maximo admite {1} caracteres")]
        [MinLength(1, ErrorMessage = "El campo {0} minimo admite {1} caracteres")]
        [RegularExpression("^[A-Z]*$", ErrorMessage = "Solo se permiten letras mayusculas A-Z")]
        public string BinCodeABC { get; set; } = null!;

        [Required(ErrorMessage = "{0} Obligatorio")]
        [Display(Name = "Codigo Ubicación")]
        [RegularExpression("^[A-Z0-9]*$", ErrorMessage = "Solo se permiten letras mayusculas A-Z y numeros 0-9")]
        [MaxLength(9, ErrorMessage = "El campo {0} solo admite {1} caracteres")]
        [MinLength(9, ErrorMessage = "El campo {0} solo admite {1} caracteres")]
        [StringLength(9, ErrorMessage = "El campo {0} solo admite {1} caracteres")]
        public string BinCode { get; set; } = null!;

        [Display(Name = "Descripcion Ubicación")]
        [MaxLength(100, ErrorMessage = "El campo {0} maximo admite {1} caracteres")]
        public string? BinDescription { get; set; }

        [Display(Name = "Largo Centimetros")]
        public int HeightCM { get; set; }

        [Display(Name = "Ancho Centimetros")]
        public int WidthCM { get; set; }

        [Display(Name = "Profundida Centimetros")]
        public int DepthCM { get; set; }

        [Display(Name = "Peso Kilogramos")]
        public int WeightKG { get; set; }

        [Display(Name = "Porcentaje USO")]
        public int PercentUsed { get; set; }

        [Display(Name = "Activa")]
        public bool Active { get; set; }

        public BinType? BinType { get; set; }
        public SubWinery? SubWinery { get; set; }
    }
}
