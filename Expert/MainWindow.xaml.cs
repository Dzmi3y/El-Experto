using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Expert
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewModel VM;

        public MainWindow()
        {
            InitializeComponent();
            VM=new ViewModel();
            VM.MessageErorDeleg = SetMessage;
            DataContext = VM;

            VM.ParserHasCompletedWorkEvent += () => { RedactorRichTextBox.Document = VM.TextDocument;};
            VM.GetTextFromTextRedactorDeleg = () => 
            {
                return new TextRange(RedactorRichTextBox.Document.ContentStart, 
                    RedactorRichTextBox.Document.ContentEnd).Text;
            }; 
        }


        private void SetMessage(string message)
        {
            MessageBox.Show(message);
        }

        private void QuestionsGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Enter)
            {
                VM.VariantAnsverText = AnswerVariantTextBox.Text;

                SaveUserAnswerButton.Command.Execute(null);
            }
        }

    }
}
