using DVG_MITIPS.Types;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace DVG_MITIPS
{
    public partial class ProblemSolverWindow : Window
    {
        private readonly DvgViewModel _viewModel;
        public ObservableCollection<GardenCharacteristic> GardenCharacteristics { get; set; } = new();
        public ObservableCollection<GardenProject> GardenProjects { get; set; } = new();

        public ProblemSolverWindow()
        {
            InitializeComponent();

            _viewModel = new DvgViewModel();

            // Binding
            this.DataContext = _viewModel;
            characteristicsDataGrid.DataContext = this;
            gardenProjectsItemsControl.DataContext = this;

            Loaded += ProblemSolverWindow_Loaded;
            Closed += ProblemSolver_Closed;
        }

        private void ProblemSolverWindow_Loaded(object sender, RoutedEventArgs e)
        {
            problemSolverDataInputGrid.Visibility = Visibility.Visible;
            problemSolverResultGrid.Visibility = Visibility.Collapsed;

            RehandleRequirementCombobox();
        }

        private void ProblemSolver_Closed(object? sender, EventArgs e)
        {
            this.Owner.Show();
            this.Owner.Focus();
        }

        private void makeGardenProjectButton_Click(object sender, RoutedEventArgs e)
        {
            var vegetablesThatCanBePlant = new List<Vegetable>();
            var gardenCharacteristics = GardenCharacteristics.ToDictionary(gc => gc.Name, gc => gc);

            foreach (var gc in gardenCharacteristics)
            {
                Console.WriteLine(gc.Key);
                Console.WriteLine("\t" + gc.Value.Value);
            }

            var unnasignedRequirements = new HashSet<Requirement>();
            var declinedVegetables = new Dictionary<Vegetable, List<Requirement>>();

            foreach (var vegetable in _viewModel.Vegetables)
            {
                bool isCanPlanted = true;

                foreach (var vr in vegetable.VegetableRequirements)
                {
                    if (!gardenCharacteristics.ContainsKey(vr.Requirement.Name))
                    {
                        unnasignedRequirements.Add(vr.Requirement);
                        continue;
                    }

                    var gc = gardenCharacteristics[vr.Requirement.Name];

                    var isGroundCheck = (vr.RangeMin <= gc.Value && gc.Value <= vr.RangeMax);

                    isCanPlanted = isCanPlanted && isGroundCheck;

                    if (!isGroundCheck)
                    {
                        if (declinedVegetables.ContainsKey(vegetable))
                        {
                            declinedVegetables[vegetable].Add(vr.Requirement);
                        }
                        else
                        {
                            declinedVegetables.Add(vegetable, new List<Requirement>() { vr.Requirement });
                        }
                    }
                }

                if (isCanPlanted)
                {
                    vegetablesThatCanBePlant.Add(vegetable);
                }
            }

            var badList = new List<(GardenCharacteristic gc, Requirement r)>();

            foreach (var characteristic in GardenCharacteristics)
            {
                var r = _viewModel.Requirements.First(r => r.Name == characteristic.Name);
                if (characteristic.Value < r.MinValue || characteristic.Value > r.MaxValue)
                {
                    badList.Add((characteristic, r));
                }
            }

            if (vegetablesThatCanBePlant.Count == 0 || unnasignedRequirements.Count > 0 || badList.Count > 0)
            {
                DvgDialog.Specified.GardenProjectFailedDialog(this, _viewModel, declinedVegetables, GardenCharacteristics.ToList(), unnasignedRequirements.ToList(), badList);
                return;
            }

            problemSolverDataInputGrid.Visibility = Visibility.Collapsed;
            problemSolverResultGrid.Visibility = Visibility.Visible;

            gardenProjectsScrollView.Visibility = Visibility.Visible;
            problemSolverResultTextBlock.Visibility = Visibility.Collapsed;

            var compatibleSets = FindCompatibleSets(vegetablesThatCanBePlant).OrderByDescending(set => set.Count);

            GardenProjects.Clear();

            int idx = 0;
            var maxSetSize = compatibleSets.First().Count;

            foreach (var compatibleSet in compatibleSets)
            {
                if (compatibleSet.Count != maxSetSize)
                {
                    break;
                }

                GardenProjects.Add(new GardenProject()
                {
                    Id = idx,
                    Name = "Проект огорода " + (idx + 1),
                    Vegetables = compatibleSet,
                    DeclinedVegetables = declinedVegetables
                });

                idx += 1;
            }

            gardenProjectsItemsControl.Items.Refresh();
        }

        public static List<List<Vegetable>> FindCompatibleSets(List<Vegetable> vegetables)
        {
            var compatibleSets = new List<List<Vegetable>>();

            foreach (var veg in vegetables)
            {
                var newSets = new List<List<Vegetable>>();
                foreach (var set in compatibleSets)
                {
                    if (set.All(v => v.CompatibleVegetables.Contains(veg) && veg.CompatibleVegetables.Contains(v)))
                    {
                        var newSet = new List<Vegetable>(set) { veg };
                        newSets.Add(newSet);
                    }
                }

                compatibleSets.AddRange(newSets);
                compatibleSets.Add([veg]);
            }

            return compatibleSets;
        }

        private void backToInputDataGridButton_Click(object sender, RoutedEventArgs e)
        {
            problemSolverDataInputGrid.Visibility = Visibility.Visible;
            problemSolverResultGrid.Visibility = Visibility.Collapsed;
        }

        private void RehandleRequirementCombobox()
        {
            var selectedRequirements = GardenCharacteristics.Select(gc => gc.Name).ToHashSet();

            var itemsSource = _viewModel.Requirements.Where(r => !selectedRequirements.Contains(r.Name)).ToList();

            requirementsStackPanel.IsEnabled = itemsSource.Count() > 0;

            requirementsComboBox.ItemsSource = itemsSource;

            if (itemsSource.Count > 0 && requirementsComboBox.SelectedIndex == -1)
            {
                requirementsComboBox.SelectedIndex = 0;
            }

            if (GardenCharacteristics.Count > 0)
            {
                requirementWarning.Visibility = Visibility.Collapsed;
                characteristicsDataGrid.Visibility = Visibility.Visible;
                characteristicsDataGrid.Items.Refresh();
            }
            else
            {
                requirementWarning.Visibility = Visibility.Visible;
                characteristicsDataGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void addRequirementToGardenButton_Click(object sender, RoutedEventArgs e)
        {
            var requirement = (Requirement)requirementsComboBox.SelectedItem;

            GardenCharacteristics.Add(new GardenCharacteristic()
            {
                Name = requirement.Name,
                Value = requirement.MinValue,
                MinValue = requirement.MinValue,
                MaxValue = requirement.MaxValue,
            });

            RehandleRequirementCombobox();
        }

        private void deleteRequirementButton_Click(object sender, RoutedEventArgs e)
        {
            var rName = (string)((Button)sender).Tag;
            GardenCharacteristics.Remove(GardenCharacteristics.First(gc => gc.Name == rName));
            RehandleRequirementCombobox();
        }

        private void showGardenProjectButton(object sender, RoutedEventArgs e)
        {
            var gardenProjectId = (int)((Button)sender).Tag;
            DvgDialog.Specified.GardenProjectDialog(this, _viewModel, GardenProjects.First(gp => gp.Id == gardenProjectId));
        }
    }
}
