using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Project
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scores
    {
        private MainPage parent;
        public Scores(MainPage parent)
        {
            InitializeComponent();
            this.parent = parent;
            LoadScores();
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            parent.Children.Add(parent.mainMenu);
            parent.Children.Remove(this);
        }

        private void LoadScores()
        {
            /*
            string text =
            List<String> names = new List<String>();
            List<int> scores = new List<int>();
            while ((line = score_file.ReadLine()) != null)
            {
                string[] ns = line.Split('|');
                names.Add(ns[0]);
                scores.Add(Convert.ToInt32(ns[1]));
            }
            string score_display = "";
            while (names.Count != 0)
            {
                int index = 0;
                int max_score = 0;
                int i = 0;
                while (i < names.Count)
                {
                    if (scores[index] > max_score)
                    {
                        max_score = scores[index];
                        index = i;
                    }
                    i++;
                }
                score_display += (names[index] + " " + Convert.ToString(scores[index]) + System.Environment.NewLine);
                names.RemoveAt(index);
                scores.RemoveAt(index);
            }
            txtScoreBoard.Text = score_display;
            */
        }
    }
}
