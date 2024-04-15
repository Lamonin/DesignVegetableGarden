using System.ComponentModel;

namespace DVG_MITIPS.Types
{

    public class GardenCharacteristic : INotifyPropertyChanged, IDataErrorInfo
    {
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    NotifyPropertyChanged(nameof(Name));
                }
            }
        }

        private bool inRange;
        public bool InRange
        {
            get { return inRange; }
            set
            {
                if (inRange != value)
                {
                    if (value)
                    {
                        if (RangeMax < RangeMin)
                        {
                            RangeMax = RangeMin;
                        }
                    }
                    inRange = value;
                    NotifyPropertyChanged(nameof(InRange));
                }
            }
        }

        private double rangeMin;
        public double RangeMin
        {
            get { return rangeMin; }
            set
            {
                if (rangeMin != value)
                {
                    rangeMin = value;
                    NotifyPropertyChanged(nameof(RangeMin));
                }
            }
        }

        private double rangeMax;
        public double RangeMax
        {
            get { return rangeMax; }
            set
            {
                if (rangeMax != value)
                {
                    rangeMax = value;
                    NotifyPropertyChanged(nameof(RangeMax));
                }
            }
        }

        public string Error => "Некорректные данные";

        public string this[string columnName] {
            get
            {
                string error = String.Empty;
                switch (columnName)
                {
                    case "RangeMin":
                        if (RangeMin > RangeMax && InRange)
                        {
                            RangeMin = RangeMax;
                        }
                        break;
                    case "RangeMax":
                        if (RangeMax < RangeMin && InRange)
                        {
                            RangeMax = RangeMin;
                        }
                        break;
                }
                return error;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
