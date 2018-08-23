using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Expert
{
    class TreeAnswers
    {
        public ObservableCollection<TreeViewItem> Tree { get; private set; }

        public TreeAnswers()
        {
            Tree = new ObservableCollection<TreeViewItem>();
        }

        public void RefreshTree(Dictionary<string, ReadyAnswer> listReadyAnswers)
        {
            Reset();

            foreach (var oneAns in listReadyAnswers)
            {
                List<string> ContentNode = new List<string>();
                ContentNode.Add("Если:");

                foreach (var resultAndCF in listReadyAnswers[oneAns.Key].ListEquals)
                {

                    ContentNode.Add("   - "+resultAndCF.Key + "=" + resultAndCF.Value);
                }
                ContentNode.Add("То "+oneAns.Key + ":");

                foreach (var resultAndCF in listReadyAnswers[oneAns.Key].ListResultAndCF)
                {

                    ContentNode.Add("   - " + resultAndCF.Key + " КД=" + resultAndCF.Value);
                }

                Tree.Add(
                    new TreeViewItem()
                    {
                        Header = oneAns.Key,
                        ItemsSource = ContentNode
                    });
            }
        }

        public void Reset()
        {
            Tree.Clear();
        }
    }
}
