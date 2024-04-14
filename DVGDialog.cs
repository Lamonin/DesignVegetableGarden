using ModernWpf.Controls;
using System.Windows.Controls;
using System.Windows;
using ModernWpf.Controls.Primitives;

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
        }
    }
}
