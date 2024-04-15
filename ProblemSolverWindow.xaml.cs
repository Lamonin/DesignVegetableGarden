using DVG_MITIPS.Types;
using System.Collections.ObjectModel;
using System.Globalization;
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
            problemSolverDataInputGrid.Visibility = Visibility.Collapsed;
            problemSolverResultGrid.Visibility = Visibility.Visible;

            var vegetablesThatCanBePlant = new List<Vegetable>();
            var gardenCharacteristics = GardenCharacteristics.ToDictionary(gc => gc.Name, gc => gc);

            foreach (var gc in gardenCharacteristics)
            {
                Console.WriteLine(gc.Key);
                Console.WriteLine("\t" + gc.Value.Value);
                Console.WriteLine("\t" + gc.Value.Value);
            }

            foreach (var vegetable in _viewModel.Vegetables)
            {
                bool isCanPlanted = true;

                foreach (var vr in vegetable.VegetableRequirements)
                {
                    if (!gardenCharacteristics.ContainsKey(vr.Requirement.Name))
                    {
                        Console.WriteLine("Ошибка. Ошибка. Ошибка.");
                        break;
                    }
                    var gc = gardenCharacteristics[vr.Requirement.Name];

                    isCanPlanted = isCanPlanted && (vr.RangeMin <= gc.Value && gc.Value <= vr.RangeMax);

                    if (!isCanPlanted)
                    {
                        break;
                    }
                }



                if (isCanPlanted)
                {
                    vegetablesThatCanBePlant.Add(vegetable);
                }
            }

            var compatibleSets = FindCompatibleSets(vegetablesThatCanBePlant).OrderByDescending(set => set.Count);
            //vegetablesThatCanBePlant = FindLargestCompatibleSet(vegetablesThatCanBePlant);

            //var resultText = vegetablesThatCanBePlant.Count > 0
            //    ? "На огороде можно разместить следующие растения:"
            //    : "На огороде нельзя разместить ни одного растения.";


            //for (int i = 0; i < vegetablesThatCanBePlant.Count; i++)
            //{
            //    Vegetable v = vegetablesThatCanBePlant[i];
            //    resultText += $"\n{i + 1}. {v.Name}";
            //}

            GardenProjects.Clear();
            var maxSetSize = compatibleSets.First().Count;
            int idx = 0;
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
                    Vegetables = compatibleSet
                });

                idx += 1;
            }

            gardenProjectsItemsControl.Items.Refresh();

            //problemSolverResultTextBlock.Text = resultText;
        }

        public static List<List<Vegetable>> FindCompatibleSets(List<Vegetable> vegetables)
        {
            var compatibleSets = new List<List<Vegetable>>();

            foreach (var veg in vegetables)
            {
                var newSets = new List<List<Vegetable>>();
                foreach (var set in compatibleSets)
                {
                    if (set.All(v => v.CompatibleVegetables.Contains(veg)))
                    {
                        var newSet = new List<Vegetable>(set);
                        newSet.Add(veg);
                        newSets.Add(newSet);
                    }
                }

                compatibleSets.AddRange(newSets);
                compatibleSets.Add(new List<Vegetable> { veg });
            }

            return compatibleSets;
        }

        public static List<Vegetable> FindLargestCompatibleSet(List<Vegetable> vegetables)
        {
            var compatibleSets = FindCompatibleSets(vegetables);
            if (compatibleSets.Count == 0)
                return new List<Vegetable>();

            for (int i = 0; i < compatibleSets.Count; i++)
            {
                List<Vegetable>? set = compatibleSets[i];
                Console.WriteLine(" возможный проект огорода № " + i);
                foreach (var veg in set)
                {
                    Console.WriteLine("\t" + veg.Name);
                }
            }

            return compatibleSets.OrderByDescending(set => set.Count).First();
        }

        private void backToInputDataGridButton_Click(object sender, RoutedEventArgs e)
        {
            problemSolverDataInputGrid.Visibility = Visibility.Visible;
            problemSolverResultGrid.Visibility = Visibility.Collapsed;
        }

        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            var textBox = (TextBox)sender;

            var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

            e.Handled = !double.TryParse(fullText, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out _);
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
                Value = requirement.MinValue
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
            var gardenProjectId = (int)((Button) sender).Tag;
            DvgDialog.Specified.GardenProjectDialog(this, GardenProjects.First(gp => gp.Id == gardenProjectId));
        }
    }
}
