using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Share.Models.Security
{
    public class FormUserType
    {
        public long Id { get; set; }

        [Display(Name = "Formulario")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public long FormId { get; set; }

        public Form? Form { get; set; } = null!;

        [Display(Name = "Rol")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public long UserTypeId { get; set; }

        public UserType? UserType { get; set; } = null!;

        [Display(Name = "Crear")]
        public bool Create { get; set; }

        [Display(Name = "Leer")]
        public bool Read { get; set; }

        [Display(Name = "Actualizar")]
        public bool Update { get; set; }

        [Display(Name = "Eliminar")]
        public bool Delete { get; set; }
    }
}
