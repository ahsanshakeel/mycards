﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyCard.Web.Models
{
    public class LicenseTypeModel
    {
        public int Id { get; set; }
        public string LicenseTypeName { get; set; }
        public decimal Price { get; set; }
    }
}