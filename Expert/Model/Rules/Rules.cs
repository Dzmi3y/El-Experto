using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expert
{
    class Rules
    {
        public List<Rule> ListRules { get; private set; }
        
        private List<int> NumbersTrueRules;
        private int LastRule;
        private List<string> ListObjectsForQuestions;
        private bool FlagForChange;
        private string CurrentUserPurpose;
        private Dictionary<string, string> ListOfEqualities;

        public event Action<ReadyAnswer,bool> EventAnswerIsReady;
        public event Action<List<string>> EvetNotEnoughData;


        public Rules()
        {
            ListOfEqualities = new Dictionary<string, string>();
            ListRules = new List<Rule>();
            NumbersTrueRules = new List<int>();
            LastRule = 0;
        }

        public void Change(string currentUserPurpose, Dictionary<string,string> listOfEqualities)
        {

            if (listOfEqualities.Keys.Contains(currentUserPurpose))
            {
                ReadyAnswer Ans = new ReadyAnswer();
                Ans.ListResultAndCF.Add(listOfEqualities[currentUserPurpose], 100);

                EventAnswerIsReady?.Invoke(Ans, true);
            }
            else
            {

                ListOfEqualities = listOfEqualities;
                FlagForChange = true;
                CurrentUserPurpose = currentUserPurpose;
                int i;
                for (i = LastRule; i < ListRules.Count; i++)
                {
                    ListRules[i].EvetNotEnoughData += NotEnoughDataForRule;

                    if (currentUserPurpose == ListRules[i].Purpose)
                    {
                        if (ListRules[i].Change(ListOfEqualities) == true) NumbersTrueRules.Add(i);
                    }

                    ListRules[i].EvetNotEnoughData -= NotEnoughDataForRule;
                    if (FlagForChange == false) break;
                    else LastRule = i + 1;
                }
                if (i == ListRules.Count)
                {
                    LastRule = 0;
                    Answer(currentUserPurpose);
                }
            }
        }

        public void LoadRules(List<Rule> loadListRules)
        {
            ListRules = loadListRules;
        }

        private void NotEnoughDataForRule (List<string> listObjectsForQuestions)
        {
            ListObjectsForQuestions = listObjectsForQuestions;
            FlagForChange = false;
            EvetNotEnoughData?.Invoke(listObjectsForQuestions);
        }

        private void Answer(string purpose)
        {
            ReadyAnswer Ans = new ReadyAnswer();

            if (NumbersTrueRules.Count == 0)
            {
                Ans.ListEquals = ListOfEqualities;
                Ans.ListResultAndCF.Add("Таких правил не существует", 100);
            }
            else
            {
                foreach (int numberTrueRule in NumbersTrueRules)
                {
                    foreach (var equals in ListRules[numberTrueRule].ListOfEqualitiesForRule)
                    {
                        if (!Ans.ListEquals.Keys.Contains(equals.Key))
                        {
                            Ans.ListEquals.Add(equals.Key, equals.Value);
                        }
                    }


                    foreach (var ResultAndCF in ListRules[numberTrueRule].ListResultAndCF)
                    {
                        if (Ans.ListResultAndCF.Keys.Contains(ResultAndCF.Key))
                        {
                            Ans.ListResultAndCF[ResultAndCF.Key] = CalculateCF(Ans.ListResultAndCF[ResultAndCF.Key], ResultAndCF.Value);
                        }
                        else
                        {
                            Ans.ListResultAndCF.Add(ResultAndCF.Key, ResultAndCF.Value);
                        }
                    }
                }
            }
            NumbersTrueRules.Clear();
            EventAnswerIsReady?.Invoke(Ans,true);
        }

        private double CalculateCF(double cf1,double cf2)
        {
            return((int)((cf1*100) +(cf2*100)-(cf1*cf2))/100);
        }
    }
}
