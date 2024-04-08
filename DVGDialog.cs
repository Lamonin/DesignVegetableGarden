using ModernWpf.Controls;
using System.Windows.Controls;
using System.Windows;

namespace DVG_MITIPS
{
    internal class DVGDialog
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
    }
}
