using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuT_and_Trololo.Models
{
    public class Kniga
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CoverPath { get; set; }
        public string Content { get; set; }
        public int AuthorId { get; set; }
        public string AutorImya { get; set; }
        public bool IsFrozen { get; set; }       
        public double SrednyayaOtsenka { get; set; }
    }
}
