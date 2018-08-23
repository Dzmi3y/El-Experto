using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Expert
{
    class UserData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public Dictionary<string, ReadyAnswer> ListReadyAnswers { get; set; }

        public Dictionary<string, string> ListOfCurrentEqualities { get; set; }

        public ObservableCollection<string> ListPurposes { get { return listPurposes; } set { listPurposes = value; OnPropertyChanged("listPurposes"); } }

        private ObservableCollection<string> listPurposes;

        public Dictionary<string, Ask> ListAsks { get; set; }

        public UserData()
        {
            ListPurposes = new ObservableCollection<string>();
            ListOfCurrentEqualities = new Dictionary<string, string>();
            ListReadyAnswers = new Dictionary<string, ReadyAnswer>();
            ListAsks = new Dictionary<string, Ask>();

        }

        public void Reset()
        {
            ListOfCurrentEqualities.Clear();
            ListReadyAnswers.Clear();
        }

        public void LoadUserData(ObservableCollection<string> listPurposes, Dictionary<string, Ask> listAsks)
        {
            ListPurposes = listPurposes;
            ListAsks = listAsks;
        }

        public void AddResult(string currentPurpose, ReadyAnswer result)
        {
            ListReadyAnswers.Add(currentPurpose, result);
            AddResultToListOfCurrentEqualities(currentPurpose, result.ListResultAndCF);
        }


        private void AddResultToListOfCurrentEqualities(string currentPurpose, Dictionary<string, double> listResultAndCF)
        {

            string Result = "";
            double CF = 0;

        
            List<string> RulesWithIdenticalCF = new List<string>();
            bool IsFirstPass = true;

            foreach (var CurrentResultAndCF in listResultAndCF)
            {
                if (CurrentResultAndCF.Key != "Таких правил не существует")
                {
                    if (IsFirstPass)
                    {
                        Result = CurrentResultAndCF.Key;
                        CF = CurrentResultAndCF.Value;
                        RulesWithIdenticalCF.Add(CurrentResultAndCF.Key);
                        IsFirstPass = false;
                    }
                    else
                    {

                        if (CF < CurrentResultAndCF.Value)
                        {
                            Result = CurrentResultAndCF.Key;
                            CF = CurrentResultAndCF.Value;
                            RulesWithIdenticalCF.Clear();
                            RulesWithIdenticalCF.Add(Result);
                        }
                        else
                        {
                            if (CF == CurrentResultAndCF.Value)
                                RulesWithIdenticalCF.Add(CurrentResultAndCF.Key);
                        }

                    }
                }
            }

            if (!IsFirstPass)
            {
                if (ListOfCurrentEqualities.Keys.Contains(currentPurpose))
                    ListOfCurrentEqualities[currentPurpose] = Result;
                else
                    ListOfCurrentEqualities.Add(currentPurpose, Result);
            }

        }


    }
}
