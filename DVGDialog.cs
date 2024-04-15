using ModernWpf.Controls;
using System.Windows.Controls;
using System.Windows;
using ModernWpf.Controls.Primitives;
using DVG_MITIPS.Types;
using System.ComponentModel;
using System.Xml.Linq;
using System.Windows.Controls.Primitives;
using System.Globalization;

namespace DVG_MITIPS
{
    internal class DvgDialog
    {
        public static async void Confirm(Window owner, string title, string message, string primaryLabel, string closeLabel, Action? primaryAction = null, Action? closeAction = null)
        {
            ContentDialog dialog = new ContentDialog()
            {
                Title = title,
                Content = message,
                PrimaryButtonText = primaryLabel,
                CloseButtonText = closeLabel,
                Owner = owner
            };

            dialog.PrimaryButtonClick += (o, e) =>
            {
                dialog.Hide();
                primaryAction?.Invoke();
            };

            dialog.CloseButtonClick += (o, e) =>
            {
                dialog.Hide();
                closeAction?.Invoke();
            };

            await dialog.ShowAsync();
        }

        public static async void Prompt(Window owner, string title, string message, string initialValue, string primaryLabel, string closeLabel, Action<string>? primaryAction = null, Action? closeAction = null)
        {
            var container = new StackPanel();

            Label inputBoxHeader = new Label()
            {
                Content = message
            };

            TextBox inputBox = new TextBox()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            inputBox.Text = initialValue;

            container.Children.Add(inputBoxHeader);
            container.Children.Add(inputBox);

            ContentDialog dialog = new ContentDialog()
            {
                Title = title,
                Content = container,
                PrimaryButtonText = primaryLabel,
                CloseButtonText = closeLabel,
                Owner = owner
            };

            dialog.PrimaryButtonClick += (o, e) =>
            {
                dialog.Hide();
                primaryAction?.Invoke(inputBox.Text);
            };

            dialog.CloseButtonClick += (o, e) =>
            {
                dialog.Hide();
                closeAction?.Invoke();
            };

            await dialog.ShowAsync();
        }

        public static class Specified
        {
            public static async void RequirementPromptDialog(Window owner, string title, string primaryLabel, string closeLabel, Action<string, double, double> primaryAction, string? defaultName = null, double? defaultMinValue = null, double? defaultMaxValue = null)
            {
                var container = new SimpleStackPanel() { Spacing = 8 };

                TextBox requirementNameBox = new TextBox() { HorizontalAlignment = HorizontalAlignment.Stretch };
                ControlHelper.SetHeader(requirementNameBox, "Название требования");
                if (defaultName != null) { requirementNameBox.Text = defaultName; }

                TextBox minValueBox = new TextBox() { HorizontalAlignment = HorizontalAlignment.Stretch };
                ControlHelper.SetHeader(minValueBox, "Минимальное значение");
                ControlHelper.SetPlaceholderText(minValueBox, "0");
                minValueBox.PreviewTextInput += TextBox_PreviewTextInput;
                if (defaultMinValue != null) { minValueBox.Text = defaultMinValue.ToString(); }

                TextBox maxValueBox = new TextBox() { HorizontalAlignment = HorizontalAlignment.Stretch };
                ControlHelper.SetHeader(maxValueBox, "Максимальное значение");
                ControlHelper.SetPlaceholderText(maxValueBox, "1");
                maxValueBox.PreviewTextInput += TextBox_PreviewTextInput;
                if (defaultMaxValue != null) { maxValueBox.Text = defaultMaxValue.ToString(); }

                var sp = new SimpleStackPanel() { Spacing = 8, Orientation = Orientation.Horizontal };
                sp.Children.Add(minValueBox);
                sp.Children.Add(maxValueBox);

                container.Children.Add(requirementNameBox);
                container.Children.Add(sp);

                ContentDialog dialog = new ContentDialog()
                {
                    Title = title,
                    Content = container,
                    PrimaryButtonText = primaryLabel,
                    CloseButtonText = closeLabel,
                    Owner = owner
                };

                dialog.PrimaryButtonClick += (o, e) =>
                {
                    dialog.Hide();
                    primaryAction?.Invoke(
                        requirementNameBox.Text,
                        double.Parse(string.IsNullOrWhiteSpace(minValueBox.Text) ? "0" : minValueBox.Text, CultureInfo.InvariantCulture),
                        double.Parse(string.IsNullOrWhiteSpace(maxValueBox.Text) ? "1" : maxValueBox.Text, CultureInfo.InvariantCulture)
                    );
                };

                await dialog.ShowAsync();

                return;

                void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
                {
                    var textBox = (TextBox)sender;

                    var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

                    e.Handled = !double.TryParse(fullText, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out _);
                }
            }

            public static async void VegetableCharacteristicPromptDialog(Window owner, VegetableRequirement vegetableRequirement, Action<double, double> primaryAction)
            {
                Console.WriteLine(vegetableRequirement.Requirement.MinValue + "  " +  vegetableRequirement.Requirement.MaxValue);

                var container = new SimpleStackPanel() { Spacing = 8 };

                TextBox minValueBox = new TextBox() { HorizontalAlignment = HorizontalAlignment.Stretch };
                ControlHelper.SetHeader(minValueBox, "Значение От");
                ControlHelper.SetPlaceholderText(minValueBox, vegetableRequirement.RangeMin.ToString());
                minValueBox.Text = vegetableRequirement.RangeMin.ToString(CultureInfo.InvariantCulture);
                minValueBox.PreviewTextInput += TextBox_PreviewTextInput;

                TextBox maxValueBox = new TextBox() { HorizontalAlignment = HorizontalAlignment.Stretch };
                ControlHelper.SetHeader(maxValueBox, "Значение До");
                ControlHelper.SetPlaceholderText(maxValueBox, vegetableRequirement.RangeMax.ToString());
                maxValueBox.Text = vegetableRequirement.RangeMax.ToString(CultureInfo.InvariantCulture);
                maxValueBox.PreviewTextInput += TextBox_PreviewTextInput;

                var grd = new Grid();
                grd.ColumnDefinitions.Add(new ColumnDefinition());
                grd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(8) });
                grd.ColumnDefinitions.Add(new ColumnDefinition());

                grd.Children.Add(minValueBox);
                grd.Children.Add(maxValueBox);

                Grid.SetColumn(minValueBox, 0);
                Grid.SetColumn(maxValueBox, 2);

                container.Children.Add(grd);

                ContentDialog dialog = new ContentDialog()
                {
                    Title = $"Изменения значения {vegetableRequirement.Requirement?.Name} для растения {vegetableRequirement.Vegetable?.Name}",
                    Content = container,
                    PrimaryButtonText = "Изменить",
                    CloseButtonText = "Отмена",
                    Owner = owner
                };

                dialog.PrimaryButtonClick += (o, e) =>
                {
                    dialog.Hide();
                    primaryAction?.Invoke(
                        double.Parse(string.IsNullOrWhiteSpace(minValueBox.Text) ? vegetableRequirement.RangeMin.ToString() : minValueBox.Text, CultureInfo.InvariantCulture),
                        double.Parse(string.IsNullOrWhiteSpace(maxValueBox.Text) ? vegetableRequirement.RangeMax.ToString() : maxValueBox.Text, CultureInfo.InvariantCulture)
                    );
                };

                await dialog.ShowAsync();

                return;

                void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
                {
                    var textBox = (TextBox)sender;

                    var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

                    e.Handled = !double.TryParse(fullText, CultureInfo.InvariantCulture, out _);
                }
            }

            public static async void CompletenesResultDialog(Window owner, DvgViewModel viewModel)
            {
                // Check completenes logic
                var plantsWithoutCompatibles = new List<Vegetable>();
                var plantsWithoutRequirements = new List<Vegetable>();

                foreach (var vegetable in viewModel.Vegetables)
                {
                    if (vegetable.CompatibleVegetables.Count == 0)
                    {
                        plantsWithoutCompatibles.Add(vegetable);
                    }
                    if (vegetable.Requirements.Count == 0)
                    {
                        plantsWithoutRequirements.Add(vegetable);
                    }
                }

                // Build result text
                var resultText = "";

                if (plantsWithoutRequirements.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(resultText)) resultText += "\n\n";
                    resultText = "Растения, у которых не установлено ни одного требования:";
                    for (int i = 0; i < plantsWithoutRequirements.Count; i++)
                    {
                        resultText += $"\n{i + 1}. {plantsWithoutRequirements[i].Name}";
                    }
                }

                if (plantsWithoutCompatibles.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(resultText)) resultText += "\n\n";
                    resultText += "Растения, которые не совместимы ни с одним растением:";
                    for (int i = 0; i < plantsWithoutCompatibles.Count; i++)
                    {
                        resultText += $"\n{i + 1}. {plantsWithoutCompatibles[i].Name}";
                    }
                }

                if (string.IsNullOrWhiteSpace(resultText))
                {
                    resultText = "Проверка прошла успешно. Все разделы корректно заполнены.";
                }
                else
                {
                    resultText = "Проверка завершена. В ходе проверки были выявлены ошибки.\n" + resultText;
                }

                // Dialog build logic
                var scrollViewer = new ScrollViewer
                {
                    MaxHeight = 200,
                    Padding = new Thickness(0, 0, 16, 0)
                };
                var container = new SimpleStackPanel() { Spacing = 8 };
                var resultTextBlock = new TextBlock() { Text = resultText };

                container.Children.Add(resultTextBlock);
                scrollViewer.Content = container;

                var dialog = new ContentDialog()
                {
                    Title = "Результаты проверки полноты",
                    Content = scrollViewer,
                    CloseButtonText = "Принять",
                    Owner = owner
                };

                dialog.CloseButtonClick += (o, e) =>
                {
                    dialog.Hide();
                };

                await dialog.ShowAsync();
            }

            public static async void GardenProjectDialog(Window owner, GardenProject project)
            {
                var resultText = project.Vegetables.Count > 0
                    ? "На огороде можно разместить следующие растения:"
                    : "На огороде нельзя разместить ни одного растения.";


                for (int i = 0; i < project.Vegetables.Count; i++)
                {
                    Vegetable v = project.Vegetables[i];
                    resultText += $"\n{i + 1}. {v.Name}";
                }

                var scrollViewer = new ScrollViewer
                {
                    MaxHeight = 200,
                    Padding = new Thickness(0, 0, 16, 0)
                };

                var container = new SimpleStackPanel() { Spacing = 8 };
                var resultTextBlock = new TextBlock() { Text = resultText };

                container.Children.Add(resultTextBlock);
                scrollViewer.Content = container;

                var dialog = new ContentDialog()
                {
                    Title = project.Name,
                    Content = scrollViewer,
                    CloseButtonText = "Закрыть",
                    Owner = owner
                };

                dialog.CloseButtonClick += (o, e) =>
                {
                    dialog.Hide();
                };

                await dialog.ShowAsync();
            }
        }
    }
}
