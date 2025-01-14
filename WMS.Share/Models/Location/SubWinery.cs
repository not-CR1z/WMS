using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Share.Models.Location
{
    public class SubWinery : UserUpdate
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "Bodega")]
        public long WineryId { get; set; }

        [Required(ErrorMessage = "El campo {0} Es Requerido")]
        [Display(Name = "Codigo")]
        [Range(1,999, ErrorMessage = "El campo {0} solo admite numeros 1 a 999")]
        public int  Code { get; set; }

        [Display(Name = "Descripción")]
        [MaxLength(200, ErrorMessage = "El campo {0} maximo admite {1} caracteres")]
        public string? Description { get; set; }

        [Display(Name = "Activa")]
        public bool Active { get; set; }

        public Winery? Winery { get; set; }

        public ICollection<Bin>? Bins { get; set; }
    }
}
