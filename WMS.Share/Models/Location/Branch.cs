using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Share.Models.Location
{
    public class Branch : UserUpdate
    {
        [Key]
        public long Id { get; set; }

        [Required(ErrorMessage = "El campo {0} Es Requerido")]
        [Display(Name = "Nombre")]
        [MaxLength(20,ErrorMessage ="El campo {0} maximo admite {1} caracteres")]
        public string Name { get; set; } = null!;

        [Display(Name = "Descripción")]
        [MaxLength(200, ErrorMessage = "El campo {0} maximo admite {1} caracteres")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "El campo {0} Es Requerido")]
        [Display(Name = "Contacto")]
        [MaxLength(200, ErrorMessage = "El campo {0} maximo admite {1} caracteres")]
        public string Contact { get; set; } = null!;

        [Required(ErrorMessage = "El campo {0} Es Requerido")]
        [Display(Name = "Telefono Contacto")]
        [MaxLength(50, ErrorMessage = "El campo {0} maximo admite {1} caracteres")]
        public string PhoneContact { get; set; } = null!;

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "El campo {0} Es Requerido")]
        [Display(Name = "Email Contacto")]
        [MaxLength(100, ErrorMessage = "El campo {0} maximo admite {1} caracteres")]
        public string EmailContact { get; set; } = null!;

        [Display(Name = "Contingencia")]
        public bool Contingency { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Correo Envio de Notificaciones")]
        [MaxLength(100, ErrorMessage = "El campo {0} maximo admite {1} caracteres")]
        public string? EmailFromNotification { get; set; }

        [Display(Name = "Clave Para Envio de Notificaciones")]
        [DataType(DataType.Password)]
        [MaxLength(50, ErrorMessage = "El campo {0} maximo admite {1} caracteres")]
        public string? EmailFromNotificationPassword { get; set; }

        [Display(Name = "Host")]
        [MaxLength(100, ErrorMessage = "El campo {0} maximo admite {1} caracteres")]
        public string? EmailFromHost { get; set; }

        [Display(Name = "Puerto")]
        public int EmailFromPort { get; set; }

        [Display(Name = "Ssl")]
        public bool EmailFromSsl { get; set; }

        public ICollection<Winery>? Wineries { get; set; }

    }
}
