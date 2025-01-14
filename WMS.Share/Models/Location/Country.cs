using System.ComponentModel.DataAnnotations;
using WMS.Share.Interfaces;

namespace WMS.Share.Models.Location
{
    public class Country : IEntityWithName
    {
        [Key]
        public long Id { get; set; }

        [Display(Name = "País")]
        [MaxLength(100, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Name { get; set; } = null!;

        public ICollection<State>? States { get; set; }

        [Display(Name = "Departamentos / Estados")]
        public int StatesNumber => States == null || States.Count == 0 ? 0 : States.Count;
    }
}