using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SC_Player_Intel_App
{
    public class WhiteListedPlayer
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ReasonForWhitelist { get; set; }
        public decimal Fee { get; set; }
    }
}
