using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Share.Models.Security
{
    public class FormParent
    {
        public long Id { get; set; }

        public string Name { get; set; } = null!;

        public string Icon { get; set; } = null!;

        public int Secuencie { get; set; }

        public ICollection<FormSubParent>? FormSubParents { get; set; }
    }
}
