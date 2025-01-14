using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WMS.Share.Models.Security
{
    public class Form
    {
        public long Id { get; set; }

        public string Name { get; set; } = null!;

        public string Icon { get; set; } = null!;

        public long FormSubParentId { get; set; }

        public FormSubParent? FormSubParent { get; set; } = null!;

        public string Href { get; set; } = null!;

        public int Secuence { get; set; }

        public int FormCode { get; set; }

        [NotMapped]
        [Display(Name="Roles Asignados")]
        public int RolesNumber => FormUserTypes == null || FormUserTypes.Count == 0 ? 0 : FormUserTypes.Count;

        public ICollection<FormUserType>? FormUserTypes { get; set; }
    }
}
