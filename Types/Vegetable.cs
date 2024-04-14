using System.ComponentModel;

namespace DVG_MITIPS.Types
{
    public class Vegetable : INotifyPropertyChanged
    {
        private string? _name;

        public int Id { get; set; }

        public string? Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    NotifyPropertyChanged(nameof(Name));
                }
            }
        }

        public List<Vegetable> CompatibleVegetables { get; set; } = new();

        public List<Requirement> Requirements { get; set; } = new();

        public Vegetable()
        {
            Name = string.Empty;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
