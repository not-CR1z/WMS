using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WMS.Share.Models;
using WMS.Share.Models.Magister;

namespace WMS.Share.DTOs
{
    public class UserUpdateDTO
    {
        public User? CreateUser { get; set; }
        public long CreateUserId { get; set; }
        public DateTime CreateDate { get; set; }
        public User? UpdateUser { get; set; }
        public long UpdateUserId { get; set; }
        public DateTime UpdateDate { get; set; }
        public User? DeleteUser { get; set; }
        public long DeleteUserId { get; set; }
        public DateTime? DeleteDate { get; set; }
        public User? ChangeStateUser { get; set; }
        public long ChangeStateUserId { get; set; }
        public DateTime? ChangeStateDate { get; set; }
    }
}
