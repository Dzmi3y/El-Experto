using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using Microsoft.Win32;
using System.IO;

namespace Expert
{
    class ViewModel : INotifyPropertyChanged
    {
        #region public Property Filds
        public TreeAnswers TreeAns { get; set; }

        public FlowDocument TextDocument
        {
            get { return textDocument; }
            set { textDocument = value; OnPropertyChanged("TextDocument"); }
        }

        public string VariantAnsverText
        {
            get { return variantAnsverText; }
            set { variantAnsverText = value; OnPropertyChanged("VariantAnsverText"); }
        }

        public string StartPanel
        {
            get { return startPanel; }
            set { startPanel = value; OnPropertyChanged("StartPanel"); }
        }
        public string QuestionsPanel
        {
            get { return questionsPanel; }
            set { questionsPanel = value; OnPropertyChanged("QuestionsPanel"); }
        }
        public bool CheckMode
        {
            get { return checkMode; }
            set
            {
                checkMode = value;
                if (value)
                {
                    StartPanel = "Hidden";
                    QuestionsPanel = "Visible";
                }
                else
                {
                    QuestionsPanel = "Hidden";
                    StartPanel = "Visible";
                }
                OnPropertyChanged("CheckMode");
            }
        }

        public string StatusString
        {
            get { return statusString; }
            set { statusString = value; OnPropertyChanged("StatusString"); }
        }

        public string FlagVariantsAnswersList
        {
            get { return flagVariantsAnswersList; }
            set { flagVariantsAnswersList = value; OnPropertyChanged("FlagVariantsAnswersList"); }
        }
        public string FlagVariantAnsverText
        {
            get { return flagVariantAnsverText; }
            set { flagVariantAnsverText = value; OnPropertyChanged("FlagVariantAnsverText"); }
        }
        public bool ObjectIsExist
        {
            get { return objectIsExist; }
            set
            {
                objectIsExist = value;
                if (value)
                {
                    FlagVariantAnsverText = "Hidden";
                    FlagVariantsAnswersList = "Visible";
                }
                else
                {
                    FlagVariantsAnswersList = "Hidden";
                    FlagVariantAnsverText = "Visible";
                }
                OnPropertyChanged("ObjectIsExist");
            }
        }

        public string CurrentPurpose
        {
            get { return currentPurpose; }
            set { currentPurpose = value; OnPropertyChanged("CurrentPurpose"); }
        }
        public UserData User
        {
            get { return user; }
            set { user = value; OnPropertyChanged("User"); }
        }
        public string Question
        {
            get { return question; }
            set { question = value; OnPropertyChanged("Question"); }
        }
        public int IndexCurrentPurpose
        {
            get { return indexCurrentPurpose; }
            set
            {
                indexCurrentPurpose = value;
                OnPropertyChanged("IndexCurrentPurpose");
            }
        }

        public int IndexVariantsAnswers
        {
            get { return indexVariantsAnswers; }
            set { indexVariantsAnswers = value; OnPropertyChanged("IndexVariantsAnswers"); }
        }

        public ObservableCollection<string> MessageAnsver { get; set; }
        public ObservableCollection<string> VariantsAnswers { get; set; }

        public Command CommandParseFromTextRedactor { get; set; }
        public Command CommandAchievementOfPurpose { get; set; }
        public Command CommandToAnswerTheQuestion { get; set; }
        public Command CommandReset { get; set; }
        public Command CommandOpendialog { get; set; }
        public Command CommandSavedialog { get; set; }

        public Action<string> MessageErorDeleg;
        public Func<string> GetTextFromTextRedactorDeleg;
        #endregion

        #region private Property Filds
        private bool FileIsSelected;
        private FlowDocument textDocument;
        private string statusString;
        private string variantAnsverText;
        private bool checkMode;
        private string startPanel;
        private string questionsPanel;

        private bool objectIsExist;
        private string flagVariantsAnswersList;
        private string flagVariantAnsverText;

        private int indexCurrentPurpose;
        private int indexVariantsAnswers;
        private List<string> ListObjectsForQuestions { get; set; }
        private string currentPurpose;
        private string question;
        private UserData user;

        private Rules AllRules;
        private int NumberQuestion;
        private Parser TextParser;
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action ParserHasCompletedWorkEvent;

        public ViewModel()
        {

            MessageAnsver = new ObservableCollection<string>();
            VariantsAnswers = new ObservableCollection<string>();
            User = new UserData();
            AllRules = new Rules();
            NumberQuestion = 0;
            CheckMode = false;
            VariantAnsverText = "";


            TextParser = new Parser();
            TextParser.GetPurposesAndAsksDeleg = User.LoadUserData;
            TextParser.GetListRulesDeleg = AllRules.LoadRules;
            TextParser.MessageEvent += (message) => 
            {
                FileIsSelected = false;
                StatusString = "Файл не выбран";
                User.ListPurposes.Clear();
                
                MessageErorDeleg?.Invoke(message);
                
            };
            TextParser.AllReadyEvent += () =>
            {
                if (TextParser.TextDocument != null)
                {
                    TextDocument = TextParser.TextDocument;
                    ParserHasCompletedWorkEvent?.Invoke();
                }
            };

            FileIsSelected = false;
            StatusString = "Файл не выбран";
            TreeAns = new TreeAnswers();

            AllRules.EvetNotEnoughData += (LOFQ) =>
            {
                ListObjectsForQuestions = LOFQ;
                NumberQuestion = 0;
                ToAsk();
            };
            AllRules.EventAnswerIsReady += Answer;



            CommandAchievementOfPurpose = new Command();
            CommandToAnswerTheQuestion = new Command();
            CommandReset = new Command();
            CommandParseFromTextRedactor = new Command();
            CommandOpendialog = new Command();
            CommandSavedialog = new Command();

            CommandSavedialog.Deleg += SaveDialog;
            CommandOpendialog.Deleg += OpenDialog;
            CommandAchievementOfPurpose.Deleg += AchievementOfPurpose;
            CommandToAnswerTheQuestion.Deleg += ToAnswerTheQuestion;
            CommandReset.Deleg += Reset;
            CommandParseFromTextRedactor.Deleg += ParseFromTextRedactor;

        }

        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void ToAsk()
        {

            if (NumberQuestion < ListObjectsForQuestions.Count)
            {

                string key = ListObjectsForQuestions[NumberQuestion];

                
              
                if (User.ListAsks.Keys.Contains(key))
                {
                    if (User.ListAsks[key].ListValues != null)
                    {
                        ObjectIsExist = true;
                        Question = (User.ListAsks[key].Question != null) ? User.ListAsks[key].Question : "_Выберите значение для '" + key + "'.";

                        foreach (var varAns in User.ListAsks[key].ListValues)
                        {
                            VariantsAnswers.Add(varAns);
                        }
                    }
                    else
                    {
                        ObjectIsExist = false;
                        Question = (User.ListAsks[key].Question != null) ? User.ListAsks[key].Question : "_Выберите значение для '" + key + "'.";
                    }

                }
                else
                {
                    ObjectIsExist = false;
                    Question = "_Введите значение для '" + key + "' ";
                }
            }
            else
            {
                VariantsAnswers.Clear();
                Question = "";
                Change();
            }

        }

        private void Answer(ReadyAnswer result, bool isNewAnswer)
        {
            CheckMode = false;
            MessageAnsver.Clear();
            foreach (var res in result.ListResultAndCF)
            {
                MessageAnsver.Add(res.Key + "  КД=" + res.Value);
            }
            if (isNewAnswer)
            {
                User.AddResult(CurrentPurpose, result);
                TreeAns.RefreshTree(User.ListReadyAnswers);
            }
        }

        private void Change()
        {
            if (User.ListReadyAnswers.Keys.Contains(CurrentPurpose))
                Answer(User.ListReadyAnswers[CurrentPurpose], false);
            else
                AllRules.Change(CurrentPurpose, User.ListOfCurrentEqualities);
        }

        private void AchievementOfPurpose()
        {
            try
           {
                VariantsAnswers.Clear();
                Question = "";
                CurrentPurpose = User.ListPurposes[IndexCurrentPurpose];
                CheckMode = true;
                Change();
            }
            catch
            {
                MessageErorDeleg?.Invoke("Цель не выбрана");
            }
        }

        private void ToAnswerTheQuestion()
        {

            if (ObjectIsExist)
            {
                User.ListOfCurrentEqualities.Add(ListObjectsForQuestions[NumberQuestion], VariantsAnswers[IndexVariantsAnswers]);
            }
            else
            {
                User.ListOfCurrentEqualities.Add(ListObjectsForQuestions[NumberQuestion], VariantAnsverText);
            }

            NumberQuestion++;
            VariantsAnswers.Clear();
            VariantAnsverText = "";
            ToAsk();
        }

        private void Reset()
        {
            User.Reset();
            TreeAns.Reset();
            MessageAnsver.Clear();
        }

        public void OpenFile(string url)
        {
            FileIsSelected = true;
            CheckMode = false;
            user.ListPurposes.Clear();
            Reset();
            TextParser.ToParse(url);
            if (FileIsSelected)
                StatusString = "Открыт файл: " + url;     
        }

        private void ParseFromTextRedactor()
        {
            FileIsSelected = true;
            CheckMode = false;
            user.ListPurposes.Clear();
            Reset();
            TextParser.ToParseFromTextRedactor(GetTextFromTextRedactorDeleg.Invoke());
            if (FileIsSelected) StatusString = "Загружен файл из текстового редактора.";
        }

        public void OpenDialog()
        {
            OpenFileDialog myDialog = new OpenFileDialog();
            myDialog.Filter = "Text(*.TXT)|*.txt" + "|Все файлы (*.*)|*.* ";
            myDialog.CheckFileExists = true;
            myDialog.Multiselect = true;
            if (myDialog.ShowDialog() == true)
            {
               OpenFile(myDialog.FileName);
            }
        }

        public void SaveDialog()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text(*.TXT)|*.txt";

            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(
                    saveFileDialog.FileName,
                   
                    new TextRange(TextDocument.ContentStart, TextDocument.ContentEnd).Text,
                    Encoding.Default);
        }
    }
}
