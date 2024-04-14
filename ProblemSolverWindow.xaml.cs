using System.Windows;

namespace DVG_MITIPS
{
    public partial class ProblemSolverWindow : Window
    {
        public ProblemSolverWindow()
        {
            InitializeComponent();
            Loaded += ProblemSolver_Loaded;
            Closed += ProblemSolver_Closed;
        }

        private void ProblemSolver_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void ProblemSolver_Closed(object? sender, EventArgs e)
        {
            this.Owner.Show();
        }
    }
}
