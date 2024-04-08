using DVG_MITIPS.Types;
using ModernWpf.Controls;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace DVG_MITIPS
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Vegetable> Vegetables { get; set; }
        public ObservableCollection<Vegetable> Requirements { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            // Collections
            Vegetables = new ObservableCollection<Vegetable>();
            Requirements = new ObservableCollection<Vegetable>();

            // Binding
            vegetableDataGrid.DataContext = this;
            requirementDataGrid.DataContext = this;

            // Events subscription
            Vegetables.CollectionChanged += Vegetables_CollectionChanged;
        }

        private void CheckForVegetablesPlaceholder()
        {
            vegetableDataGridPlaceholder.Visibility = Vegetables.Count > 0 ? Visibility.Hidden : Visibility.Visible;
            vegetableDataGrid.Visibility = Vegetables.Count > 0 ? Visibility.Visible : Visibility.Hidden;
        }

        private void Vegetables_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CheckForVegetablesPlaceholder();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (knowledgeEditorTab.IsSelected)
            {
                Title = "Проектирование огорода. Решатель задач.";

                checkCompletenessButton.Visibility = Visibility.Visible;
                CheckForVegetablesPlaceholder();
            }

            if (problemSolverTab.IsSelected)
            {
                Title = "Проектирование огорода. Редактор знаний.";
                checkCompletenessButton.Visibility = Visibility.Hidden;
            }
        }

        #region VegetableMethods

        private void AddVegetable_Click(object sender, RoutedEventArgs e)
        {
            DVGDialog.Prompt(this, "Добавить растение", "Введите название растения", "", "Добавить", "Отмена", (responseText) =>
            {
                if (!string.IsNullOrWhiteSpace(responseText))
                {
                    if (Vegetables.FirstOrDefault(v => v.Name.Equals(responseText, StringComparison.InvariantCultureIgnoreCase)) != null)
                    {
                        ModernWpf.MessageBox.Show($"{responseText} уже было добавлено в базу знаний", "Внимание");
                    }
                    else
                    {
                        Vegetables.Add(new Vegetable { Name = responseText });
                    }
                }
            });
        }

        private void EditVegetable_Click(object sender, RoutedEventArgs e)
        {

            if (vegetableDataGrid.SelectedItem != null)
            {
                Vegetable selectedVegetable = (Vegetable)vegetableDataGrid.SelectedItem;
                DVGDialog.Prompt(this, "Изменить название растения", "Введите новое название растения", selectedVegetable.Name, "Изменить", "Отмена", (responseText) =>
                {
                    if (!string.IsNullOrWhiteSpace(responseText))
                    {
                        selectedVegetable.Name = responseText;
                        vegetableDataGrid.Items.Refresh();
                    }
                });
            }
            else
            {
                ModernWpf.MessageBox.Show("Для начала выберите растение для редактирования.", "Внимание");
            }
        }

        private void RemoveVegetable_Click(object sender, RoutedEventArgs e)
        {
            if (vegetableDataGrid.SelectedItem != null)
            {
                Vegetable selectedVegetable = (Vegetable)vegetableDataGrid.SelectedItem;
                DVGDialog.Confirm(this, "Подтверждение удаления", $"Вы уверены, что хотите удалить растение {selectedVegetable.Name}?", "Удалить", "Отмена", () =>
                {
                    Vegetables.Remove(selectedVegetable);
                });
            }
            else
            {
                ModernWpf.MessageBox.Show("Для начала выберите растение для удаления.", "Внимание");
            }
        }

        #endregion


        #region RequirementMethods

        private void AddRequirement_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditRequirement_Click(object sender, RoutedEventArgs e)
        {
            if (vegetableDataGrid.SelectedItem != null)
            {
            }
            else
            {
                ModernWpf.MessageBox.Show("Для начала выберите растение для редактирования.", "Внимание");
            }
        }

        private void RemoveRequirement_Click(object sender, RoutedEventArgs e)
        {
            if (vegetableDataGrid.SelectedItem != null)
            {
            }
            else
            {
                ModernWpf.MessageBox.Show("Для начала выберите растение для удаления.", "Внимание");
            }
        }

        #endregion

        private void TabControl_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}