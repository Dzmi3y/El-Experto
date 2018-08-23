using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expert
{
    class ReadyAnswer
    {
        public Dictionary<string, double> ListResultAndCF { get; set; }
        public Dictionary<string, string> ListEquals { get; set; }

        public ReadyAnswer()
        {
            ListResultAndCF = new Dictionary<string, double>();
            ListEquals = new Dictionary<string, string>();
        }
    }
}
