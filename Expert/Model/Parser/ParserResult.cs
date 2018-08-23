using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Expert
{
    static class ParserResult
    {
        public static List<Rule> ListRules { get; private set; }
        public static ObservableCollection<string> ListPurposes { get; private set; }
        public static Dictionary<string, Ask> ListAsks { get; private set; }

        static ParserResult()
        {
            ListRules = new List<Rule>();
            ListPurposes = new ObservableCollection<string>(); 
            ListAsks = new Dictionary<string, Ask>();
        }

        public static void AddObjectQuesion(string obj, string question)
        {
            if (ListAsks.Keys.Contains(obj))
            {
                ListAsks[obj].Question = question;
            }
            else
            {
                ListAsks.Add(obj, new Ask() { Question = question });
            }

        }

        public static void AddObectAllowedValues(string obj, List<string> listValues)
        {

            if (ListAsks.Keys.Contains(obj))
            {
                ListAsks[obj].ListValues = listValues;
            }
            else
            {
                ListAsks.Add(obj, new Ask() { ListValues = listValues });
            }

        }

        public static void AddPurposes(string purpose)
        {
            if (!(ListPurposes.Contains(purpose))) ListPurposes.Add(purpose);

        }


        public static void ClearAll()
        {
            ListRules.Clear();
            ListPurposes.Clear();
            ListAsks.Clear();
        }

    }
}
