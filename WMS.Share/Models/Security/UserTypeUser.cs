using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Share.Models.Magister;

namespace WMS.Share.Models.Security
{
    public class UserTypeUser
    {
        public long Id { get; set; }

        [Display(Name = "Tipo de Usuario")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public long UserTypeId { get; set; }

        [Display(Name = "Tipo de usuario")]
        public UserType? UserType { get; set; } = null!;

        [Display(Name = "Usuario")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public long UserIdLocal { get; set; }

        public string UserId { get; set; } = null!;

        [Display(Name = "Usuario")]
        public User? User { get; set; } = null!;
    }
}
