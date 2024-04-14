using System.Windows;

namespace DVG_MITIPS
{
    public partial class AutorizeWindow : Window
    {
        public AutorizeWindow()
        {
            InitializeComponent();
        }

        private void openKnowledgeEditorButton_Click(object sender, RoutedEventArgs e)
        {
            var knowledgeEditorWindow = new KnowledgeEditorWindow { Owner = this };
            knowledgeEditorWindow.Show();
            this.Hide();
        }

        private void openProblemSolverButton_Click(object sender, RoutedEventArgs e)
        {
            var problemSolverWindow = new ProblemSolverWindow { Owner = this };
            problemSolverWindow.Show();
            this.Hide();
        }
    }
}
