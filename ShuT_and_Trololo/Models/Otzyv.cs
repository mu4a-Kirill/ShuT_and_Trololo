using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuT_and_Trololo.Models
{
    public class Otzyv
    {
        public int ReviewId { get; set; }
        public int UserId { get; set; }
        public string PolzovatelImya { get; set; }  
        public int BookId { get; set; }
        public string ReviewText { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsFrozen { get; set; }
    }
}