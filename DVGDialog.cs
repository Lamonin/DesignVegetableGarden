using ModernWpf.Controls;
using System.Windows.Controls;
using System.Windows;
using ModernWpf.Controls.Primitives;
using DVG_MITIPS.Types;
using System.Globalization;
using Xceed.Wpf.Toolkit;

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

        public static class Misc
        {
            public static UIElement WrapWithHeader(UIElement element, string text)
            {
                var ssp = new SimpleStackPanel() { Spacing = 4 };
                ssp.Children.Add(new Label() { Content = text });
                ssp.Children.Add(element);
                return ssp;
            }
        }

        public static class Specified
        {
            public static async void RequirementPromptDialog(Window owner, string title, string primaryLabel, string closeLabel, Action<string, double, double> primaryAction, string? defaultName = null, double? defaultMinValue = null, double? defaultMaxValue = null)
            {
                var container = new SimpleStackPanel() { Spacing = 8 };

                var minValueUpDown = new DoubleUpDown()
                {
                    Height = 32,
                    ShowButtonSpinner = false,
                    Value = defaultMinValue != null ? defaultMinValue : 0,
                    DefaultValue = 0
                };

                var maxValueUpDown = new DoubleUpDown()
                {
                    Height = 32,
                    ShowButtonSpinner = false,
                    Value = defaultMaxValue != null ? defaultMaxValue : 1,
                    DefaultValue = 1
                };

                TextBox requirementNameBox = new TextBox() { HorizontalAlignment = HorizontalAlignment.Stretch };
                ControlHelper.SetHeader(requirementNameBox, "Название требования");
                if (defaultName != null) { requirementNameBox.Text = defaultName; }

                var grd = new Grid();
                grd.ColumnDefinitions.Add(new ColumnDefinition());
                grd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(8) });
                grd.ColumnDefinitions.Add(new ColumnDefinition());

                var ssp1 = Misc.WrapWithHeader(minValueUpDown, "Минимальное значение");
                var ssp2 = Misc.WrapWithHeader(maxValueUpDown, "Максимальное значение");
                grd.Children.Add(ssp1);
                grd.Children.Add(ssp2);

                Grid.SetColumn(ssp1, 0);
                Grid.SetColumn(ssp2, 2);

                container.Children.Add(requirementNameBox);
                container.Children.Add(grd);

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
                    primaryAction?.Invoke(requirementNameBox.Text, (double)minValueUpDown.Value, (double)maxValueUpDown.Value);
                };

                await dialog.ShowAsync();
            }

            public static async void VegetableCharacteristicPromptDialog(Window owner, VegetableRequirement vegetableRequirement, Action<double, double> primaryAction)
            {
                var container = new SimpleStackPanel() { Spacing = 8 };

                var minValueUpDown = new DoubleUpDown()
                {
                    Height = 32,
                    Watermark = vegetableRequirement.Requirement.MinValue,
                    Minimum = vegetableRequirement.Requirement.MinValue,
                    Maximum = vegetableRequirement.Requirement.MaxValue,
                    ClipValueToMinMax = true,
                    ShowButtonSpinner = false,
                    Value = vegetableRequirement.RangeMin,
                    DefaultValue = vegetableRequirement.Requirement.MinValue
                };

                var maxValueUpDown = new DoubleUpDown()
                {
                    Height = 32,
                    Watermark = vegetableRequirement.Requirement.MaxValue,
                    Minimum = vegetableRequirement.Requirement.MinValue,
                    Maximum = vegetableRequirement.Requirement.MaxValue,
                    ClipValueToMinMax = true,
                    ShowButtonSpinner = false,
                    Value = vegetableRequirement.RangeMax,
                    DefaultValue = vegetableRequirement.Requirement.MaxValue
                };

                var grd = new Grid();
                grd.ColumnDefinitions.Add(new ColumnDefinition());
                grd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(8) });
                grd.ColumnDefinitions.Add(new ColumnDefinition());

                var ssp1 = Misc.WrapWithHeader(minValueUpDown, "Значение От");
                var ssp2 = Misc.WrapWithHeader(maxValueUpDown, "Значение До");
                grd.Children.Add(ssp1);
                grd.Children.Add(ssp2);

                Grid.SetColumn(ssp1, 0);
                Grid.SetColumn(ssp2, 2);

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
                    primaryAction?.Invoke((double)minValueUpDown.Value, (double)maxValueUpDown.Value);
                };

                await dialog.ShowAsync();
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

            public static async void GardenProjectDialog(Window owner, DvgViewModel viewModel, GardenProject project)
            {
                var resultText = "";

                var plantWithCorrectGround = viewModel.Vegetables.Where(v => !project.DeclinedVegetables.ContainsKey(v)).ToList();

                if (plantWithCorrectGround.Count > 0)
                {
                    resultText += "Почва подходит следующим растениям:";
                    for (int i = 0; i < plantWithCorrectGround.Count; i++)
                    {
                        resultText += $"\n{i + 1}. {plantWithCorrectGround[i].Name}";
                    }
                    resultText += "\n\n";
                }

                resultText += "На огороде можно одновременно разместить следующие растения:";

                for (int i = 0; i < project.Vegetables.Count; i++)
                {
                    Vegetable v = project.Vegetables[i];
                    resultText += $"\n{i + 1}. {v.Name}";
                }

                if (project.DeclinedVegetables.Count > 0)
                {
                    resultText += "\n\nХод решения:";
                    resultText += "\nСледующим растениям не подходит почва:";
                    var idx = 1;
                    foreach (var dv in project.DeclinedVegetables)
                    {
                        resultText += $"\n{idx++}. {dv.Key.Name} (не подходят значения {string.Join(", ", dv.Value.Select(r => r.Name))})";
                    }
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

            public static async void GardenProjectFailedDialog(Window owner, DvgViewModel viewModel, List<GardenCharacteristic> gardenCharacteristics, List<Requirement> unnasignedRequirements, List<(GardenCharacteristic gc, Requirement r)> badList)
            {
                var resultText = "Причины:";
                var flag = false;

                if (unnasignedRequirements.Count > 0)
                {
                    flag = true;
                    resultText += $"\nСледующие требования нужны растениям, но не описаны для участка:";

                    for (int i = 0; i < unnasignedRequirements.Count; i++)
                    {
                        resultText += $"\n{i + 1}. {unnasignedRequirements[i].Name}";
                    }
                }

                if (badList.Count > 0)
                {
                    flag = true;
                    resultText += $"\n\nСледующие значения характеристик не соответствуют ограничениям:";
                    for (int i = 0; i < badList.Count; i++)
                    {
                        (GardenCharacteristic gc, Requirement r) = badList[i];
                        resultText += $"\n{i + 1}. Значение характеристики {r.Name} равное {gc.Value} не попадает в диапазон от {r.MinValue} до {r.MaxValue}";
                    }
                }


                if (!flag)
                {
                    if (viewModel.Vegetables.Count == 0)
                    {
                        resultText += "\nБаза знаний не содержит растений.";
                    }
                    else
                    {
                        resultText += "\nНи одно из растений не соотвествует требованиям участка.";

                    }
                }

                var scrollViewer = new ScrollViewer
                {
                    MaxHeight = 200,
                    Padding = new Thickness(0, 0, 16, 0)
                };

                var container = new SimpleStackPanel() { Spacing = 8 };
                var resultTextBlock = new TextBlock() { Text = resultText, TextWrapping = TextWrapping.Wrap };

                container.Children.Add(resultTextBlock);
                scrollViewer.Content = container;

                var dialog = new ContentDialog()
                {
                    Title = "Невозможно составить проект огорода",
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
