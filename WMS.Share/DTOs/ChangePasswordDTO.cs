using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Share.DTOs
{
    public class ChangePasswordDTO
    {
        public long Id_Local { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        [StringLength(200, MinimumLength = 6, ErrorMessage = "El campo {0} debe tener entre {2} y {1} carácteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Email { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        [StringLength(20,ErrorMessage = "El campo {0} debe tener entre {2} y {1} carácteres.")]
        public string NewPassword { get; set; } = null!;

        [Compare("NewPassword", ErrorMessage = "La nueva contraseña y la confirmación no son iguales.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmación nueva contraseña")]
        [StringLength(20, ErrorMessage = "El campo {0} debe tener entre {2} y {1} carácteres.")]
        public string Confirm { get; set; } = null!;
    }
}
