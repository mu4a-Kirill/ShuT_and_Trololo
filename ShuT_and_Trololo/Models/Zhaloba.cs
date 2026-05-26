using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuT_and_Trololo.Models
{
    public class Zhaloba
    {
        public int ComplaintId { get; set; }
        public int UserId { get; set; }
        public string PolzovatelImya { get; set; }  
        public string Reason { get; set; }
        public int? BookId { get; set; }           
        public int? ReviewId { get; set; } 
        public int? AuthorId { get; set; } 
        public string Status { get; set; }
    }
}
