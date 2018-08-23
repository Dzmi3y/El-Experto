using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expert
{
    class Parenthesis
    {
        public char Begin { get; private set; }
        public char End { get; private set; }
        public bool Only { get; private set; }

        public string Result { set { StringIsFoundDeleg?.Invoke(value); } }

        public Action<string> StringIsFoundDeleg { get;  set; }

        public Parenthesis( char begin, char end, bool only)
        {
            Begin = begin;
            End = end;
            Only = only;
        }
    }
}
