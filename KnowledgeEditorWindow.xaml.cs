using DVG_MITIPS.Types;
using ModernWpf.Controls.Primitives;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DVG_MITIPS
{
    public partial class KnowledgeEditorWindow : Window
    {
        private readonly DvgViewModel _viewModel;

        public KnowledgeEditorWindow()
        {
            InitializeComponent();

            // Collections
            _viewModel = new DvgViewModel();

            // Binding
            this.DataContext = _viewModel;

            // Events subscription
            _viewModel.Vegetables.CollectionChanged += Vegetables_CollectionChanged;
            _viewModel.Requirements.CollectionChanged += Requirements_CollectionChanged;

            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            this.Owner.Show();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var v in _viewModel.Vegetables)
            {
                Console.WriteLine(v.Name);
                foreach (var requirement in v.Requirements)
                {
                    Console.WriteLine("\t" + requirement.Name);
                }
            }
        }

        //private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (knowledgeEditorTab.IsSelected)
        //    {
        //        Title = "Проектирование огорода. Решатель задач.";

        //        checkCompletenessButton.Visibility = Visibility.Visible;
        //    }
        //    else if (problemSolverTab.IsSelected)
        //    {
        //        Title = "Проектирование огорода. Редактор знаний.";
        //        checkCompletenessButton.Visibility = Visibility.Collapsed;
        //    }
        //}

        private void KnowledEditorTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (vegetablesTab.IsSelected)
            {
                VegetablesTabOpen();
            }
            else if (requirementTab.IsSelected)
            {
                RequirementsTabOpen();
            }
            else if (requirementDescriptionTab.IsSelected)
            {
                RequirementDescriptionTabOpen();
            }
            else if (valueCharacteristicsTab.IsSelected)
            {
                ValueCharacteristicsTabOpen();
            }
            else if (plantCompatibilityTab.IsSelected)
            {
                PlantCompatibilityTabOpen();
            }
        }

        #region VegetableMethods

        private void CheckForVegetablesPlaceHolder()
        {
            vegetableDataGridPlaceholder.Visibility = _viewModel.Vegetables.Count > 0 ? Visibility.Hidden : Visibility.Visible;
            vegetableDataGrid.Visibility = _viewModel.Vegetables.Count > 0 ? Visibility.Visible : Visibility.Hidden;
        }

        private void Vegetables_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CheckForVegetablesPlaceHolder();
        }

        private void VegetablesTabOpen()
        {
            CheckForVegetablesPlaceHolder();
        }

        private void AddVegetable_Click(object sender, RoutedEventArgs e)
        {
            DvgDialog.Prompt(this, "Добавить растение", "Введите название растения", "", "Добавить", "Отмена", (responseText) =>
            {
                // TODO Replace on validators
                if (!string.IsNullOrWhiteSpace(responseText))
                {
                    if (_viewModel.Vegetables.FirstOrDefault(v => v.Name.Equals(responseText, StringComparison.InvariantCultureIgnoreCase)) != null)
                    {
                        ModernWpf.MessageBox.Show($"Растение {responseText} уже есть в базе знаний", "Внимание");
                        return;
                    }
                    _viewModel.Vegetables.Add(new Vegetable { Name = responseText });
                    _viewModel.SaveDatabase();
                }
            });
        }

        private void EditVegetable_Click(object sender, RoutedEventArgs e)
        {
            if (vegetableDataGrid.SelectedItem is Vegetable selectedVegetable)
            {
                DvgDialog.Prompt(this, "Изменить название растения", "Введите новое название растения", selectedVegetable.Name, "Изменить", "Отмена", (responseText) =>
                {
                    if (!string.IsNullOrWhiteSpace(responseText))
                    {
                        selectedVegetable.Name = responseText;
                        _viewModel.SaveDatabase();
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
            if (vegetableDataGrid.SelectedItem is Vegetable selectedVegetable)
            {
                DvgDialog.Confirm(this, "Подтверждение удаления", $"Вы уверены, что хотите удалить растение {selectedVegetable.Name}?", "Удалить", "Отмена", () =>
                {
                    _viewModel.Vegetables.Remove(selectedVegetable);
                    _viewModel.SaveDatabase();
                });
            }
            else
            {
                ModernWpf.MessageBox.Show("Для начала выберите растение для удаления.", "Внимание");
            }
        }

        #endregion

        #region RequirementMethods

        private void CheckForRequirementsPlaceholder()
        {
            requirementDataGridPlaceholder.Visibility = _viewModel.Requirements.Count > 0 ? Visibility.Hidden : Visibility.Visible;
            requirementDataGrid.Visibility = _viewModel.Requirements.Count > 0 ? Visibility.Visible : Visibility.Hidden;
        }

        private void Requirements_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CheckForRequirementsPlaceholder();
        }

        private void RequirementsTabOpen()
        {
            CheckForRequirementsPlaceholder();
        }

        private void AddRequirement_Click(object sender, RoutedEventArgs e)
        {
            DvgDialog.Specified.RequirementPromptDialog(this, "Добавить требование", "Добавить", "Отмена", (name, min, max) =>
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    if (_viewModel.Requirements.FirstOrDefault(r => r.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) != null)
                    {
                        ModernWpf.MessageBox.Show($"Требование {name} уже было добавлено в базу знаний", "Внимание");
                    }
                    else
                    {
                        _viewModel.Requirements.Add(new Requirement { Name = name, MinValue = min, MaxValue = max });
                        _viewModel.SaveDatabase();
                    }
                }
            });
        }

        private void EditRequirement_Click(object sender, RoutedEventArgs e)
        {
            if (requirementDataGrid.SelectedItem != null)
            {
                var selectedRequirement = (Requirement)requirementDataGrid.SelectedItem;
                DvgDialog.Specified.RequirementPromptDialog(this, "Изменить требование", "Изменить", "Отмена", (name, min, max) =>
                {
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        selectedRequirement.Name = name;
                        selectedRequirement.MinValue = min;
                        selectedRequirement.MaxValue = max;
                        requirementDataGrid.Items.Refresh();
                        _viewModel.SaveDatabase();
                    }
                }, selectedRequirement.Name, selectedRequirement.MinValue, selectedRequirement.MaxValue);
            }
            else
            {
                ModernWpf.MessageBox.Show("Для начала выберите требование для редактирования.", "Внимание");
            }
        }

        private void RemoveRequirement_Click(object sender, RoutedEventArgs e)
        {
            if (requirementDataGrid.SelectedItem != null)
            {
                var selectedRequirement = (Requirement)requirementDataGrid.SelectedItem;
                DvgDialog.Confirm(this, "Подтверждение удаления", $"Вы уверены, что хотите удалить требование {selectedRequirement.Name}?", "Удалить", "Отмена", () =>
                {
                    _viewModel.Requirements.Remove(selectedRequirement);
                    _viewModel.SaveDatabase();
                });
            }
            else
            {
                ModernWpf.MessageBox.Show("Для начала выберите требование для удаления.", "Внимание");
            }
        }

        #endregion

        #region RequirementDescriptionMethods

        public void RequirementDescriptionTabOpen()
        {
            rd_vegetableComboBoxWarningLabel.Visibility = Visibility.Collapsed;
            rd_vegetableComboBox.Visibility = Visibility.Visible;
            rd_requirementComboBoxWarningLabel.Visibility = Visibility.Collapsed;
            rd_requirementStackPanel.Visibility = Visibility.Visible;
            rd_ListBox.Visibility = Visibility.Collapsed;

            if (_viewModel.Vegetables.Count == 0)
            {
                rd_vegetableComboBoxWarningLabel.Visibility = Visibility.Visible;
                rd_vegetableComboBox.Visibility = Visibility.Collapsed;
                rd_requirementStackPanel.Visibility = Visibility.Collapsed;
                return;
            }
            if (rd_vegetableComboBox.SelectedItem == null)
            {
                rd_vegetableComboBox.SelectedIndex = 0;
            }

            if (_viewModel.Requirements.Count == 0)
            {
                rd_requirementComboBoxWarningLabel.Visibility = Visibility.Visible;
                rd_requirementStackPanel.Visibility = Visibility.Collapsed;
                return;
            }

            RehandleVegetableRequirements();

            rd_ListBox.Visibility = Visibility.Visible;
        }

        private void RehandleVegetableRequirements()
        {
            if (rd_vegetableComboBox.SelectedIndex == -1) return;

            var v = (Vegetable)rd_vegetableComboBox.SelectedItem;

            var excludedIds = new HashSet<int>(v.Requirements.Select(r => r.Id));
            var filteredRequirements = _viewModel.Requirements.Where(r => !excludedIds.Contains(r.Id)).ToList();
            rd_requirementComboBox.ItemsSource = filteredRequirements;

            rd_requirementStackPanel.IsEnabled = filteredRequirements.Count != 0;
            rd_ListBox.Items.Refresh();

            if (rd_requirementComboBox.SelectedItem == null && filteredRequirements.Count != 0)
            {
                rd_requirementComboBox.SelectedIndex = 0;
            }
        }

        private void rd_vegetableComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RehandleVegetableRequirements();
        }

        private void rd_addRequirement_Click(object sender, RoutedEventArgs e)
        {
            var v = (Vegetable)rd_vegetableComboBox.SelectedItem;
            var r = (Requirement)rd_requirementComboBox.SelectedItem;

            //_viewModel.Vegetables.First(vg => vg.Id == v.Id).Requirements.Add(r);
            _viewModel.VegetableRequirements.Add(new VegetableRequirement()
            {
                VegetableId = v.Id,
                Vegetable = v,
                RequirementId = r.Id,
                Requirement = r,
                RangeMin = r.MinValue,
                RangeMax = r.MaxValue,
            });

            _viewModel.SaveDatabase();

            RehandleVegetableRequirements();
        }

        private void rd_deleteRequirementButton_Click(object sender, RoutedEventArgs e)
        {
            var rId = (int)((Button)sender).Tag;
            var v = (Vegetable)rd_vegetableComboBox.SelectedItem;
            var vo = _viewModel.Vegetables.First(vg => vg.Id == v.Id);
            vo.Requirements.RemoveAt(vo.Requirements.FindIndex(r => r.Id == rId));

            _viewModel.SaveDatabase();

            RehandleVegetableRequirements();
        }

        #endregion

        #region ValueCharacteristicsMethods

        private void ValueCharacteristicsTabOpen()
        {
            vc_plantsComboBoxWarningLabel.Visibility = Visibility.Collapsed;
            vc_plantsComboBox.Visibility = Visibility.Visible;
            vc_ListBox.Visibility = Visibility.Collapsed;

            if (_viewModel.Vegetables.Count == 0)
            {
                vc_plantsComboBoxWarningLabel.Visibility = Visibility.Visible;
                vc_plantsComboBox.Visibility = Visibility.Collapsed;
                return;
            }

            if (vc_plantsComboBox.SelectedItem == null)
            {
                vc_plantsComboBox.SelectedIndex = 0;
            }

            vc_ListBox.Visibility = Visibility.Visible;
        }

        private void vc_editValueCharacteristicButton_Click(object sender, RoutedEventArgs e)
        {
            var vcId = (int)((Button)sender).Tag;

            var vc = _viewModel.VegetableRequirements.First(vc => vc.Id == vcId);

            DvgDialog.Specified.VegetableCharacteristicPromptDialog(this, vc, (flag, min, max) =>
            {
                Console.WriteLine(flag + "  " + min + "  " + max);
                vc.InRange = flag != null && (bool) flag;
                vc.RangeMin = min;
                vc.RangeMax = max;
                _viewModel.SaveDatabase();
            });
        }

        #endregion

        #region PlantCompatibilityMethods

        private void PlantCompatibilityTabOpen()
        {
            cp_plantsComboBoxWarningLabel.Visibility = Visibility.Collapsed;
            cp_plantsComboBox.Visibility = Visibility.Visible;
            cp_compatiblePlantsComboBox.Visibility = Visibility.Visible;
            cp_ListBox.Visibility = Visibility.Collapsed;

            if (_viewModel.Vegetables.Count == 0)
            {
                cp_plantsComboBoxWarningLabel.Visibility = Visibility.Visible;
                cp_plantsComboBox.Visibility = Visibility.Collapsed;
                cp_compatiblePlantsComboBox.Visibility = Visibility.Collapsed;
                return;
            }

            if (cp_plantsComboBox.SelectedItem == null)
            {
                cp_plantsComboBox.SelectedIndex = 0;
            }

            RehandleVegetableCompatibility();

            cp_ListBox.Visibility = Visibility.Visible;
        }

        private void RehandleVegetableCompatibility()
        {
            if (cp_plantsComboBox.SelectedIndex == -1) return;

            var v = (Vegetable)cp_plantsComboBox.SelectedItem;

            var excludedIds = new HashSet<int>(v.CompatibleVegetables.Select(r => r.Id));
            excludedIds.Add(v.Id);

            var filteredVegetables = _viewModel.Vegetables.Where(v => !excludedIds.Contains(v.Id)).ToList();
            cp_compatiblePlantsComboBox.ItemsSource = filteredVegetables;

            cp_compatibleStackPanel.IsEnabled = filteredVegetables.Count != 0;
            cp_ListBox.Items.Refresh();

            if (cp_compatiblePlantsComboBox.SelectedItem == null && filteredVegetables.Count != 0)
            {
                cp_compatiblePlantsComboBox.SelectedIndex = 0;
            }
        }

        private void cp_addCompatiblePlant_Click(object sender, RoutedEventArgs e)
        {
            var v = (Vegetable)cp_plantsComboBox.SelectedItem;
            var cp = (Vegetable)cp_compatiblePlantsComboBox.SelectedItem;

            _viewModel.Vegetables.First(vg => vg.Id == v.Id).CompatibleVegetables.Add(cp);
            _viewModel.SaveDatabase();

            RehandleVegetableCompatibility();
        }

        private void cp_deleteCompatiblePlantButton_Click(object sender, RoutedEventArgs e)
        {
            var vId = (int)((Button)sender).Tag;
            var v = (Vegetable)cp_plantsComboBox.SelectedItem;
            var vo = _viewModel.Vegetables.First(vg => vg.Id == v.Id);
            vo.CompatibleVegetables.RemoveAt(vo.CompatibleVegetables.FindIndex(v => v.Id == vId));

            _viewModel.SaveDatabase();

            RehandleVegetableCompatibility();
        }

        #endregion

    }
}