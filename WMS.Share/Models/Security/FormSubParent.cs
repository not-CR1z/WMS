using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Share.Models.Security
{
    public class FormSubParent
    {
        public long Id { get; set; }

        public string Name { get; set; } = null!;

        public string Icon { get; set; } = null!;

        public long FormParentId { get; set; }

        public int Secuence { get; set; }

        public FormParent? FormParent { get; set; } = null!;

        public ICollection<Form>? Forms { get; set; }
    }
}
