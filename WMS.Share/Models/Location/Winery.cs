using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Share.DTOs;

namespace WMS.Share.Models.Location
{
    public class Winery : UserUpdate
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "Sucursal")]
        public long BranchId { get; set; }

        [Required(ErrorMessage = "El campo {0} Es Requerido")]
        [Display(Name = "Nombre")]
        [MaxLength(20, ErrorMessage = "El campo {0} maximo admite {1} caracteres")]
        public string Name { get; set; } = null!;

        [Display(Name = "Descripción")]
        [MaxLength(200, ErrorMessage = "El campo {0} maximo admite {1} caracteres")]
        public string? Description { get; set; }

        [Display(Name = "Activa")]
        public bool Active { get; set; }

        [Display(Name = "Virtual")]
        public bool Virtual { get; set; }

        public  Branch? Branch { get; set; }
        public ICollection<SubWinery>? SubWineries { get; set; }
    }
}
