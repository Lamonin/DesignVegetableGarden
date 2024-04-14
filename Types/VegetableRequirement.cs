using System.ComponentModel;

namespace DVG_MITIPS.Types
{
    public class VegetableRequirement : INotifyPropertyChanged
    {
        private bool _inRange;
        private double _rangeMin;
        private double _rangeMax;

        public int Id { get; set; }

        public int VegetableId { get; set; }

        public Vegetable? Vegetable { get; set; }

        public int RequirementId { get; set; }

        public Requirement? Requirement { get; set; }

        public bool InRange
        {
            get => _inRange; set
            {
                if (_inRange == value) return;

                _inRange = value;
                NotifyPropertyChanged(nameof(InRange));
            }
        }

        public double RangeMin
        {
            get => _rangeMin; set
            {
                if (_rangeMin == value) return;

                _rangeMin = value;
                NotifyPropertyChanged(nameof(RangeMin));
            }
        }

        public double RangeMax
        {
            get => _rangeMax; set
            {
                if (_rangeMax == value) return;

                _rangeMax = value;
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
