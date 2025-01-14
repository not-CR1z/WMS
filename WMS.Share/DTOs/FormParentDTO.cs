using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Share.Models.Security;

namespace WMS.Share.DTOs
{
    public class FormParentDTO
    {
        public long Id { get; set; }

        public string Name { get; set; } = null!;

        public string Icon { get; set; } = null!;
        public bool Expand { get; set; }
        public List<FormSubParentDTO>? FormSubParentDTOs { get; set; }
       

    }
}
