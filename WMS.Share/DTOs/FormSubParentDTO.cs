using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Share.DTOs
{
    public class FormSubParentDTO
    {
        public long Id { get; set; }

        public string Name { get; set; } = null!;

        public string Icon { get; set; } = null!;
        public bool Expand { get; set; }
        public List<FormDTO>? Forms { get; set; }
    }
}
