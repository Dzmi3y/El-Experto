using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expert
{

    static class KeyWords
    {
        public static string BeginRule;
        public static string RuleIf;
        public static string RuleThen;
        public static string BeginAllowedValues;
        public static string BeginQuestion;


        public static int GetMaxLenghtKeyWord()
        {
            int[] LengthOfBeginWords = new int[]
            {
                BeginRule.Length,
                BeginAllowedValues.Length,
                BeginQuestion.Length
             };

            return LengthOfBeginWords.Max();
        }
    }
}
