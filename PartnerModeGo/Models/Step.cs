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

        /// <summary>
        /// 黑棋胜率，0.0~1.0
        /// </summary>
        public float BlackWinRate { get; set; }

        /// <summary>
        /// 黑棋领先的目数（平均法）
        /// </summary>
        public float BlackLeadPoints { get; set; }

        /// <summary>
        /// 黑棋领先的目数（阈值法）
        /// </summary>
        public float BlackLeadPoints2 { get; set; }

        public string Comment { get; set; }

        public int[] Territory { get; set; }

        public List<Tuple<Position,float>> RecommendPoints { get; set; }
    }
}
