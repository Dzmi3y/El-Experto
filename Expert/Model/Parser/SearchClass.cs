using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expert
{
    class SearchClass
    {

        public bool FlagIsRuleRow;
        public bool FlagRuleRowIf;
        public bool FlagRuleRowThen;
        public Rule CurrentRule;

        private List<Parenthesis> ParenthesisForIf;
        private List<Parenthesis> ParenthesisForThen;
        private List<Parenthesis> ParenthesisForRule;
        private List<Parenthesis> ParenthesisForAllowedValues;
        private List<Parenthesis> ParenthesisForQuestion;

        public void SearchRuleIf(string row, int curentIndex)
        {
            ParenthesisForIf = new List<Parenthesis>();
            string obj = "";
            ParenthesisForIf.Add(new Parenthesis(' ', '=', true) { StringIsFoundDeleg = (s) => { obj = s;  } });
            ParenthesisForIf.Add(new Parenthesis('=', ' ', true)
            {
                StringIsFoundDeleg = (s) =>
                {
                    CurrentRule.ListOfEqualitiesForRule.Add(obj, s);
                }
            });
            SearchParenthesis(ParenthesisForIf, row, curentIndex);
        }

        public void SearchRuleThen(string row, int curentIndex)
        {
            ParenthesisForThen = new List<Parenthesis>();

            string Answer = "";

            Action<string, string> CFDeleg = (ans, cf) =>
            {
                CurrentRule.ListResultAndCF.Add(ans, Convert.ToInt32(cf));

            };

            ParenthesisForThen.Add(new Parenthesis(' ', '=', true) { StringIsFoundDeleg = (s) => { CurrentRule.Purpose = s.Replace(" ", string.Empty); } });


            if (row.Contains("кд="))
            {
                ParenthesisForThen.Add(new Parenthesis('=', ',', true) { StringIsFoundDeleg = (s) => { Answer = s; } });
                ParenthesisForThen.Add(new Parenthesis('=', '.', true) { StringIsFoundDeleg = (s) => { CFDeleg(Answer, s); } });
                ParenthesisForThen.Add(new Parenthesis('=', ' ', true) { StringIsFoundDeleg = (s) => { CFDeleg(Answer, s); } });
            }

            else
            {
                ParenthesisForThen.Add(new Parenthesis('=', '.', true) { StringIsFoundDeleg = (s) => { CFDeleg(s, "100"); } });
                ParenthesisForThen.Add(new Parenthesis('=', ' ', true) { StringIsFoundDeleg = (s) => { CFDeleg(s, "100"); } });
            }

            SearchParenthesis(ParenthesisForThen, row, curentIndex);

            if (row.Contains(".")) EndRule();
        }

        public void EndRule()
        {

            FlagIsRuleRow = false;
            FlagRuleRowIf = false;
            FlagRuleRowThen = false;
            ParserResult.ListRules.Add(CurrentRule);
            ParserResult.AddPurposes(CurrentRule.Purpose);
            CurrentRule = null;
        }


        public void SearchRuleName(string row, int curentIndex) // убрать
        {

            ParenthesisForRule = new List<Parenthesis>
               { new Parenthesis('о', ':', true) { StringIsFoundDeleg = (s) => { CurrentRule = new Rule(s); } } };
            SearchParenthesis(ParenthesisForRule, row, curentIndex - 1);

        }

        public void SearchQuestions(string row, int curentIndex)
        {
            ParenthesisForQuestion = new List<Parenthesis>();
            string obj = null;
            string question = null;
            ParenthesisForQuestion.Add(new Parenthesis('(', ')', true) { StringIsFoundDeleg = (s) => { obj = s; } });
            ParenthesisForQuestion.Add(new Parenthesis('=', '?', true) { StringIsFoundDeleg = (s) => { question = s; } });

            SearchParenthesis(ParenthesisForQuestion, row, curentIndex);
            ParserResult.AddObjectQuesion(obj, question + " ?");
        }

        public void SearchAllowedValues(string row, int curentIndex)
        {
            ParenthesisForAllowedValues = new List<Parenthesis>();
            string obj = null;
            List<string> ListValues = new List<string>();
            Action<string> DelegForListValues = (s) => { ListValues.Add(s); };

            ParenthesisForAllowedValues.Add(new Parenthesis('(', ')', true) { StringIsFoundDeleg = (s) => { obj = s; } });
            ParenthesisForAllowedValues.Add(new Parenthesis('=', ',', false) { StringIsFoundDeleg = DelegForListValues });
            ParenthesisForAllowedValues.Add(new Parenthesis(',', ',', false) { StringIsFoundDeleg = DelegForListValues });
            ParenthesisForAllowedValues.Add(new Parenthesis(',', ' ', true) { StringIsFoundDeleg = DelegForListValues });
            ParenthesisForAllowedValues.Add(new Parenthesis('=', ' ', true) { StringIsFoundDeleg = DelegForListValues });


            SearchParenthesis(ParenthesisForAllowedValues, row, curentIndex);
            ParserResult.AddObectAllowedValues(obj, ListValues);
        }



        public void SearchParenthesis(List<Parenthesis> listParenthesis, string row, int curentIndex)
        {
            string currentRow = row.Substring(curentIndex);

            foreach (var bracket in listParenthesis)
            {
                bool FlagEnd = false;
                int IndexBegin = 0;
                int IndexEnd = 0;
                int IndexLength = 0;

                while (!FlagEnd)
                {
                    if (!((currentRow.Contains(bracket.Begin)) && ((currentRow.Substring(1)).Contains(bracket.End)))) break;

                    IndexBegin = currentRow.IndexOf(bracket.Begin);
                    IndexEnd = (currentRow.Substring(1)).IndexOf(bracket.End) + 1;
                    IndexLength = IndexEnd - IndexBegin - 1;
                    bracket.Result = currentRow.Substring(IndexBegin + 1, IndexLength);
                    currentRow = currentRow.Substring(IndexEnd);

                    if (bracket.Only) FlagEnd = true;

                }
            }
        }




    }
}
