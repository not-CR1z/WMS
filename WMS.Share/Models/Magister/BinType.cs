using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WMS.Share.Models.Location;

namespace WMS.Share.Models.Magister
{
    public class BinType : UserUpdate
    {
        [Key]
        public long Id { get; set; }

        [Required(ErrorMessage = "El campo {0} Es Requerido")]
        [Display(Name = "Tipo Ubicación")]
        [MaxLength(20, ErrorMessage = "El campo {0} maximo admite {1} caracteres")]
        public string Name { get; set; } = null!;

        [MaxLength(200, ErrorMessage = "El campo {0} maximo admite {1} caracteres")]
        [Display(Name = "Descripción")]
        public string? Description { get; set; }

        [Display(Name = "Se Realiza Picking")]
        public bool Picking { get; set; }

        [Display(Name = "Orden Picking")]
        public int OrderPicking { get; set; }

        [JsonIgnore]
        public virtual ICollection<Bin>? Bins { get; set; }
    }
}
