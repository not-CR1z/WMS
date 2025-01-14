using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Share.DTOs
{
    public class UploadFileDTO
    {
        public MemoryStream FileStream { get; set; } = null!;
    }
}
