using ModernWpf.Controls;
using System.Windows.Controls;
using System.Windows;
using ModernWpf.Controls.Primitives;
using DVG_MITIPS.Types;
using System.ComponentModel;
using System.Xml.Linq;
using System.Windows.Controls.Primitives;

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
                if (defaultMinValue != null) { minValueBox.Text = defaultMinValue.ToString(); }

                TextBox maxValueBox = new TextBox() { HorizontalAlignment = HorizontalAlignment.Stretch };
                ControlHelper.SetHeader(maxValueBox, "Максимальное значение");
                ControlHelper.SetPlaceholderText(maxValueBox, "1");
                if (defaultMaxValue != null) { maxValueBox.Text = defaultMaxValue.ToString(); }

                var sp = new SimpleStackPanel() { Spacing = 8, Orientation = Orientation.Horizontal};
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
                        double.Parse(string.IsNullOrWhiteSpace(minValueBox.Text) ? "0" : minValueBox.Text),
                        double.Parse(string.IsNullOrWhiteSpace(maxValueBox.Text) ? "1" : maxValueBox.Text)
                    );
                };

                await dialog.ShowAsync();
            }

            public static async void VegetableCharacteristicPromptDialog(Window owner, VegetableRequirement vegetableRequirement, Action<bool?, double, double> primaryAction)
            {
                var container = new SimpleStackPanel() { Spacing = 8 };

                TextBox minValueBox = new TextBox() { HorizontalAlignment = HorizontalAlignment.Stretch };
                ControlHelper.SetHeader(minValueBox, "Значение От");
                ControlHelper.SetPlaceholderText(minValueBox, vegetableRequirement.RangeMin.ToString());
                minValueBox.Text = vegetableRequirement.RangeMin.ToString();

                TextBox maxValueBox = new TextBox() { HorizontalAlignment = HorizontalAlignment.Stretch };
                ControlHelper.SetHeader(maxValueBox, "Значение До");
                ControlHelper.SetPlaceholderText(maxValueBox, vegetableRequirement.RangeMax.ToString());
                maxValueBox.Text = vegetableRequirement.RangeMax.ToString();

                CheckBox chkBox = new CheckBox() { IsChecked = vegetableRequirement.InRange, Content = "В диапазоне" };
                chkBox.Checked += (s, e) => ChkBox_Checked();
                chkBox.Unchecked += (s, e) => ChkBox_Checked();
                ChkBox_Checked();

                var grd = new Grid();
                grd.ColumnDefinitions.Add(new ColumnDefinition());
                grd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(8) }) ;
                grd.ColumnDefinitions.Add(new ColumnDefinition());

                grd.Children.Add(minValueBox);
                grd.Children.Add(maxValueBox);

                Grid.SetColumn(minValueBox, 0);
                Grid.SetColumn(maxValueBox, 2);

                container.Children.Add(chkBox);
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
                        chkBox.IsChecked,
                        double.Parse(string.IsNullOrWhiteSpace(minValueBox.Text) ? vegetableRequirement.RangeMin.ToString() : minValueBox.Text),
                        double.Parse(string.IsNullOrWhiteSpace(maxValueBox.Text) ? vegetableRequirement.RangeMax.ToString() : maxValueBox.Text)
                    );
                };

                await dialog.ShowAsync();

                return;

                void ChkBox_Checked()
                {
                    bool isChecked = (bool) chkBox.IsChecked;
                    Grid.SetColumnSpan(minValueBox, isChecked ? 1 : 3);
                    ControlHelper.SetHeader(minValueBox, isChecked ? "Значение От" : "Значение");
                    maxValueBox.Visibility = isChecked ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }
    }
}
