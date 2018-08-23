using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expert
{
    class Ask
    {
        public string Question { get; set; }
        
        public List<string> ListValues { get; set; }

        public Ask( )
        {
            Question = null;
            
            ListValues = null;
        }


    }
}
