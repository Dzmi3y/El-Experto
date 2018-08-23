using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expert
{
    class Rule
    {
     
        public string Name { get;  }
        public string Purpose { get; set; }
        public Dictionary<string, string> ListOfEqualitiesForRule { get; }
        public Dictionary<string, int> ListResultAndCF { get; }

        public event Action<List<string>> EvetNotEnoughData;

        public Rule( string name)
        {
            Name = name;
            Purpose = "";
            ListOfEqualitiesForRule = new Dictionary<string, string>();
            ListResultAndCF = new Dictionary<string, int>() ;
        }

        public bool Change(Dictionary<string, string> listOfEqualities)
        {
            bool Result=true;
            bool NotEnoughData = false;
            List<string> ListObjectsForQuestions = new List<string>();

            foreach (var EqualitiesForRule in ListOfEqualitiesForRule)
            {
                if (listOfEqualities.Keys.Contains(EqualitiesForRule.Key))
                {
                    if (listOfEqualities[EqualitiesForRule.Key] != EqualitiesForRule.Value)
                    {
                        Result &= false;
                        break;
                    }
                }
                else
                {
                    NotEnoughData = true;
                    Result &= false;
                    ListObjectsForQuestions.Add(EqualitiesForRule.Key);
                }
            }

            if (NotEnoughData)  EvetNotEnoughData?.Invoke(ListObjectsForQuestions);

            return Result;
        }

    }
}
