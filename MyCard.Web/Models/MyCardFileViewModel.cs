using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyCard.Web.Models
{
    public class MyCardFileViewModel
    {
        public byte[] FileContent { get; set; }
        public string FileName { get; set; }
        
    }
}