using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Share.Interfaces;
using WMS.Share.Models.Magister;

namespace WMS.Share.Models.Location
{
    public class City : IEntityWithName
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "Ciudad")]
        [MaxLength(100, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Name { get; set; } = null!;

        public long StateId { get; set; }

        public State? State { get; set; }

        public ICollection<Company>? Companies { get; set; }
        //public ICollection<User>? Users { get; set; }
    }
}
