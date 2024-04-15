using DVG_MITIPS.Types;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace DVG_MITIPS
{
    public partial class ProblemSolverWindow : Window
    {
        private readonly DvgViewModel _viewModel;
        public ObservableCollection<GardenCharacteristic> GardenCharacteristics { get; set; } = new();

        public ProblemSolverWindow()
        {
            InitializeComponent();

            _viewModel = new DvgViewModel();

            // Binding
            this.DataContext = this;

            Loaded += ProblemSolverWindow_Loaded;
            Closed += ProblemSolver_Closed;
        }

        private void ProblemSolverWindow_Loaded(object sender, RoutedEventArgs e)
        {
            problemSolverDataInputGrid.Visibility = Visibility.Visible;
            problemSolverResultGrid.Visibility = Visibility.Collapsed;

            foreach (var requirement in _viewModel.Requirements)
            {
                GardenCharacteristics.Add(
                    new GardenCharacteristic()
                    {
                        Name = requirement.Name,
                        InRange = true,
                        RangeMin = requirement.MinValue,
                        RangeMax = requirement.MaxValue,
                    }
                );
            }

            characteristicsDataGrid.Items.Refresh();
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
                Console.WriteLine("\t" + gc.Value.RangeMin);
                Console.WriteLine("\t" + gc.Value.RangeMax);
            }

            foreach (var vegetable in _viewModel.Vegetables)
            {
                bool isCanPlanted = true;

                foreach (var vr in vegetable.VegetableRequirements)
                {
                    var gc = gardenCharacteristics[vr.Requirement.Name];
                    if (vr.InRange)
                    {
                        if (gc.InRange)
                        {
                            // Если диапазоны хоть как-то пересекаются
                            isCanPlanted = isCanPlanted && (vr.RangeMin >= gc.RangeMin && vr.RangeMin <= gc.RangeMax || vr.RangeMax <= gc.RangeMax && vr.RangeMax >= gc.RangeMin);
                        }
                        else
                        {
                            // Если значение почвы попало в диапазон растения
                            isCanPlanted = isCanPlanted && (vr.RangeMin <= gc.RangeMin && gc.RangeMin <= vr.RangeMax);
                        }
                    }
                    else
                    {
                        if (gc.InRange)
                        {
                            // Если значение растения попало в диапазон участка
                            isCanPlanted = isCanPlanted && (gc.RangeMin >= vr.RangeMin && vr.RangeMin <= gc.RangeMax);
                        }
                        else
                        {
                            // Если значения характеристики совпали
                            isCanPlanted = isCanPlanted && (Math.Abs(vr.RangeMin - gc.RangeMin) < 0.0001f);
                        }
                    }

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

            var resultText = vegetablesThatCanBePlant.Count > 0
                ? "На огороде можно разместить следующие растения:"
                : "На огороде нельзя разместить ни одного растения.";

            for (int i = 0; i < vegetablesThatCanBePlant.Count; i++)
            {
                Vegetable v = vegetablesThatCanBePlant[i];
                resultText += $"\n{i + 1}. {v.Name}";
            }

            problemSolverResultTextBlock.Text = resultText;
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
    }
}
