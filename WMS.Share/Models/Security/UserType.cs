using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Share.Interfaces;

namespace WMS.Share.Models.Security
{
    public class UserType : IEntityWithName
    {
        public long Id { get; set; }

        [Display(Name = "Tipo de Usuario")]
        [MaxLength(100, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Name { get; set; } = null!;

        public ICollection<UserTypeUser>? Users { get; set; }

        public ICollection<FormUserType>? FormUserTypes { get; set; }
    }
}
