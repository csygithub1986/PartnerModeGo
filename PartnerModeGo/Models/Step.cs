using PartnerModeGo.WcfService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnerModeGo
{
    public class Step
    {
        public int StepNum { get; set; }
        public Player Player { get; set; }

        public Position Position { get; set; }

        public float WinRate { get; set; }

        public string Comment { get; set; }
    }
}
