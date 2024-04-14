using System.ComponentModel;

namespace DVG_MITIPS.Types
{
    public class Requirement : INotifyPropertyChanged
    {
        private string? _name;
        private double _minValue;
        private double _maxValue;

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

        public double MinValue
        {
            get { return _minValue; }
            set
            {
                if (_minValue != value)
                {
                    _minValue = value;
                    NotifyPropertyChanged(nameof(MinValue));
                }
            }
        }

        public double MaxValue
        {
            get { return _maxValue; }
            set
            {
                if (_maxValue != value)
                {
                    _maxValue = value;
                    NotifyPropertyChanged(nameof(MaxValue));
                }
            }
        }

        public List<Vegetable> Vegetables { get; set; } = new();

        public Requirement()
        {
            Name = string.Empty;
            MinValue = 0;
            MaxValue = 0;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
