using System.ComponentModel.DataAnnotations;
using WMS.Share.Interfaces;

namespace WMS.Share.Models.Location
{
    public class State : IEntityWithName
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "Departamento / Estado")]
        [MaxLength(100, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Name { get; set; } = null!;

        public long CountryId { get; set; }

        public Country? Country { get; set; }

        public ICollection<City>? Cities { get; set; }

        [Display(Name = "Ciudades")]
        public int CitiesNumber => Cities == null || Cities.Count == 0 ? 0 : Cities.Count;
    }
}