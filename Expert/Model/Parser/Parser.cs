using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Documents;

namespace Expert
{
    class Parser
    {
        public Action<List<Rule>> GetListRulesDeleg { get; set; }
        public Action<ObservableCollection<string>, Dictionary<string, Ask>> GetPurposesAndAsksDeleg { get; set; }
        public FlowDocument TextDocument { get; set; }

        private SearchClass Searcher;

        private event Action GetDataEvent;
        public event Action AllReadyEvent;
        public event Action<string> MessageEvent;


        public Parser()
        {
            Searcher = new SearchClass
            {
                FlagIsRuleRow = false,
                FlagRuleRowIf = false,
                FlagRuleRowThen = false
            };

            KeyWords.BeginRule = " правило";
            KeyWords.BeginAllowedValues = " разрешзн";
            KeyWords.BeginQuestion = " вопрос";
            KeyWords.RuleIf = ":если";
            KeyWords.RuleThen = " то ";

            GetDataEvent = () =>
             {
                 if (ParserResult.ListRules.Count == 0) throw new Exception("No_Reles");
                 GetPurposesAndAsksDeleg?.Invoke(ParserResult.ListPurposes, ParserResult.ListAsks);
                 GetListRulesDeleg?.Invoke(ParserResult.ListRules);
                 AllReadyEvent?.Invoke();
             };
        }


        public void ToParse(string url)
        {
            try
            {
                ParserResult.ClearAll();
                TextDocument = new FlowDocument();
                Searcher.FlagIsRuleRow = false;
                using (StreamReader document = new StreamReader(url,Encoding.Default))
                {

                    while (!document.EndOfStream)
                    {
                        string TextLine =  document.ReadLine();
                        AnalysisOfTheRow(TextLine);

                        Paragraph paragraph = new Paragraph();
                        paragraph.Inlines.Add(new Bold(new Run(TextLine)));
                        TextDocument.Blocks.Add(paragraph);
                    }
                }

                GetDataEvent?.Invoke();
            }
            catch (Exception e)
            {
                if (e.Message == "No_Reles")
                    MessageEvent?.Invoke("В тексте нет ни одного правила!");
                else
                    MessageEvent?.Invoke("Синтаксическая ошибка");
            }
        }

        public void ToParseFromTextRedactor(string text)
        {
            try
            {
                ParserResult.ClearAll();
                TextDocument = null;
                string CurrentText = text;
                int NumberN;
                while (CurrentText.Length > 0)
                {
                    if (CurrentText.Contains('\n'))
                    {
                        NumberN = CurrentText.IndexOf('\n');
                        AnalysisOfTheRow(CurrentText.Substring(0, NumberN));
                        CurrentText = CurrentText.Substring(NumberN + 1);
                    }
                    else
                    {
                        AnalysisOfTheRow(CurrentText);
                        CurrentText = "";
                    }

                }
                GetDataEvent?.Invoke();
            }
            catch (Exception e)
            {
                if (e.Message == "No_Reles")
                    MessageEvent?.Invoke("В тексте нет ни одного правила!");
                else
                    MessageEvent?.Invoke("Синтаксическая ошибка");
            }

        }

        private void AnalysisOfTheRow(string row)
        {

            string CurrentRow = (" " + row + " ").Replace("\t", " ");
            CurrentRow = CurrentRow.Replace("\r", " ");
            CurrentRow = CurrentRow.Replace("\n", " ");
            if (!CurrentRow.Contains(KeyWords.BeginQuestion))
            {
                CurrentRow = CurrentRow.Replace(" и ", "#и");
                CurrentRow = CurrentRow.Replace(" ", string.Empty);
                CurrentRow = CurrentRow.Replace("#и", " и");
                CurrentRow = " " + CurrentRow + " ";
            }

            int MaxLengthBeginWord = KeyWords.GetMaxLenghtKeyWord();
            string SelectedSymbols = "";

            if (Searcher.FlagIsRuleRow)
            {
                AnalyzeRowRule(CurrentRow);

            }
            else
            {
                for (int i = 0; i < CurrentRow.Length; i++)
                {
                    if (i >= MaxLengthBeginWord) SelectedSymbols.Remove(0, 1);

                    SelectedSymbols += CurrentRow[i];

                    if (CheckKeyWord(SelectedSymbols, CurrentRow, i + 1)) break;
                }
            }
        }

        private void AnalyzeRowRule(string row)
        {
            if (row.Contains(KeyWords.RuleIf))
            {
                Searcher.FlagRuleRowIf = true;
                Searcher.FlagRuleRowThen = false;
            };

            if (row.Contains(KeyWords.RuleThen))
            {
                Searcher.FlagRuleRowIf = false;
                Searcher.FlagRuleRowThen = true;
            };


            if (Searcher.FlagRuleRowIf)
            {
                if (row.Contains("="))
                {

                    Searcher.SearchRuleIf(row, SearchFirstLetter(row));
                }
            }

            if (Searcher.FlagRuleRowThen)
            {
                if (row.Contains("="))
                {
                    Searcher.SearchRuleThen(row, SearchFirstLetter(row));
                }
            }
        }

        private int SearchFirstLetter(string row)
        {
            for (int i = 0; i < row.Length - 1; i++)
            {
                if ((row[i] == ' ') && (row[i + 1] != ' ')) return i;

            }
            return row.Length - 1;
        }

        private bool CheckKeyWord(string selectedSymbols, string row, int curentIndex)
        {

            if (selectedSymbols.Contains(KeyWords.BeginRule))
            {
                Searcher.SearchRuleName(row, curentIndex);
                Searcher.FlagIsRuleRow = true;

                if (row.Contains(KeyWords.RuleIf))
                {
                    Searcher.FlagRuleRowIf = true;
                    Searcher.FlagRuleRowThen = false;
                };
                AnalysisOfTheRow(row);
                return true;
            };

            if (selectedSymbols.Contains(KeyWords.BeginAllowedValues))
            {
                Searcher.SearchAllowedValues(row, curentIndex);
                return true;
            };

            if (selectedSymbols.Contains(KeyWords.BeginQuestion))
            {
                Searcher.SearchQuestions(row, curentIndex);
                return true;
            };

            return false;
        }
















    }
}
