using System.ComponentModel;

namespace DVG_MITIPS.Types
{
    public class VegetableRequirement : INotifyPropertyChanged
    {
        private double _rangeMin;
        private double _rangeMax;

        public int Id { get; set; }

        public int VegetableId { get; set; }

        public Vegetable? Vegetable { get; set; }

        public int RequirementId { get; set; }

        public Requirement? Requirement { get; set; }


        public double RangeMin
        {
            get => _rangeMin; set
            {
                _rangeMin = value;

                if (Requirement != null)
                {
                    if (_rangeMin < Requirement.MinValue)
                    {
                        _rangeMin = Requirement.MinValue;
                    }
                    else if (_rangeMin > Requirement.MaxValue)
                    {
                        _rangeMin = Requirement.MaxValue;
                    }
                }

                if (_rangeMin > RangeMax)
                {
                    _rangeMin = RangeMax;
                }

                NotifyPropertyChanged(nameof(RangeMin));
            }
        }

        public double RangeMax
        {
            get => _rangeMax; set
            {
                _rangeMax = value;

                if (Requirement != null)
                {
                    if (_rangeMax < Requirement.MinValue)
                    {
                        _rangeMax = Requirement.MinValue;
                    }
                    else if (_rangeMax > Requirement.MaxValue)
                    {
                        _rangeMax = Requirement.MaxValue;
                    }
                }

                if (_rangeMax < RangeMin)
                {
                    _rangeMax = RangeMin;
                }

                NotifyPropertyChanged(nameof(RangeMax));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
