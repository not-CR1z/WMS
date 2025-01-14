﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Share.DTOs
{
    public class GenericSearchDTO
    {
        public long Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }
    }
}
