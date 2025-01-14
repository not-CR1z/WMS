using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WMS.Share.Models
{

    public class UserUpdate
    {
        public long CreateUserId { get; set; }

        public DateTime CreateDate { get; set; }

        public long UpdateUserId { get; set; }

        public DateTime UpdateDate { get; set; }

        public long DeleteUserId { get; set; }

        public DateTime? DeleteDate { get; set; }

        public long ChangeStateUserId { get; set; }

        public DateTime? ChangeStateDate { get; set; }

        public bool Delete { get; set; }

        [NotMapped]
        [Display(Name = "Error")]
        public string? StrError { get; set; }

        [NotMapped]
        [Display(Name = "Fila")]
        public int? Row { get; set; }

        [NotMapped]
        [Display(Name = "Actualiza")]
        public bool? Update { get; set; }

        [NotMapped]
        public long GenericSearchId { get; set; }

        [NotMapped]
        public string? GenericSearchName { get; set; }

        [NotMapped]
        public string? GenericSearchDescription { get; set; }

        [NotMapped]
        public string? GenericSearchName1 { get; set; }

        [NotMapped]
        public string? GenericSearchName2 { get; set; }

        [NotMapped]
        public string? GenericSearchName3 { get; set; }

        [NotMapped]
        public string? GenericSearchName4 { get; set; }

        [NotMapped]
        public string? GenericSearchName5 { get; set; }

    }
}
