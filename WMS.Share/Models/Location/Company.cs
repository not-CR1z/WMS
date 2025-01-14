using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WMS.Share.Models.Security;

namespace WMS.Share.Models.Location
{
    public class Company : UserUpdate
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} Es Requerido")]
        [MaxLength(100, ErrorMessage = "EL campo {0} no puede exceder 100 caracteres")]
        public string Name { get; set; } = null!;

        [Display(Name = "Descripción")]
        [MaxLength(200, ErrorMessage = "EL campo {0} no puede exceder 200 caracteres")]
        public string Description { get; set; } = null!;

        [Display(Name = "Licencia Desde")]
        public DateTime StarLicence { get; set; }

        [Display(Name = "Licencia Hasta")]
        public DateTime EndLicence { get; set; }

        [Display(Name = "Licencia")]
        [StringLength(36, ErrorMessage = "El campo {0} admite {1} caracteres")]
        public string Licence { get; set; } = null!;

        [Display(Name = "Logo")]
        public string? Logo { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; }
        public City? City { get; set; }

        [Display(Name = "Ciudad")]
        [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar una {0}.")]
        public long CityId { get; set; }

    }
}
