using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WMS.Share.Models.Security;

namespace WMS.Share.DTOs
{
    public class FormDTO
    {
        public long Id { get; set; }

        public string Name { get; set; } = null!;

        public int FormCode { get; set; }

        public string Icon { get; set; } = null!;

        public long FormSubParentId { get; set; }

        public string Href { get; set; } = null!;

        public int Secuence { get; set; }

        public bool Create { get; set; }

        public bool Read { get; set; }

        public bool Update { get; set; }

        public bool Delete { get; set; }

        //public FormParentDTO FormParent { get; set; } = null!;
    }
}
