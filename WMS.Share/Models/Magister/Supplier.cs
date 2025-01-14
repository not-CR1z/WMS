using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Share.Models.Magister
{
    public class Supplier
    {
        [Key]
        public long Id { get; set; }

        public long DocumentTypeUserId { get; set; }

        [Display(Name = "Tipo de Documento")]
        public DocumentTypeUser? DocumentTypeUser { get; set; } = null!;

        [Display(Name = "Documento")]
        [MaxLength(20, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Document { get; set; } = null!;

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Nombre Compañia")]
        [MaxLength(200, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        public string CompanyName { get; set; } = null!;

        [Display(Name = "Nombres")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        public string? FirstName { get; set; }

        [Display(Name = "Apellidos")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        public string? LastName { get; set; }

        [Display(Name = "Dirección")]
        public string? Address { get; set; }

        [Display(Name = "Teléfono")]
        public string? Phone { get; set; }

        [Display(Name = "Correo Electrónico")]
        public string? Email { get; set; }

        [Display(Name = "Adjunto1")]
        public string? Attachment1 { get; set; }

        [Display(Name = "Adjunto2")]
        public string? Attachment2 { get; set; }

        [Display(Name = "Adjunto3")]
        public string? Attachment3 { get; set; }

        [Display(Name = "Adjunto4")]
        public string? Attachment4 { get; set; }

        [Display(Name = "Adjunto5")]
        public string? Attachment5 { get; set; }

    }
}
