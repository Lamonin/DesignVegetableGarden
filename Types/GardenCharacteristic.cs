using System.ComponentModel;

namespace DVG_MITIPS.Types
{

    public class GardenCharacteristic : INotifyPropertyChanged
    {
        private string _name = "";
        private double _value;

        public string Name
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

        public double Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    NotifyPropertyChanged(nameof(Value));
                }
            }
        }

        public double MinValue { get; set; } = 0;
        public double MaxValue { get; set; } = 1;

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
