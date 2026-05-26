using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuT_and_Trololo.Models
{
    public class Zayavka
    {
        public int ApplicationId { get; set; }
        public int UserId { get; set; }
        public string PolzovatelImya { get; set; } 
        public int? BookId { get; set; }           
        public string KnigaTitle { get; set; }     
        public string Reason { get; set; }         
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
    }
}
