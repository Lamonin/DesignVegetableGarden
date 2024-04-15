using DVG_MITIPS.Types;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace DVG_MITIPS
{
    internal class DvgViewModel
    {
        private readonly DvgContext _context;

        public ObservableCollection<Vegetable> Vegetables { get; set; }
        public ObservableCollection<Requirement> Requirements { get; set; }
        public ObservableCollection<VegetableRequirement> VegetableRequirements { get; set; }

        public DvgViewModel()
        {
            _context = new DvgContext();

            // Uncomment this to recreate database
            //_context.Database.EnsureDeleted();

            _context.Database.EnsureCreated();

            _context.Vegetables.Include(v => v.CompatibleVegetables).Include(v => v.Requirements).Include(v => v.VegetableRequirements).Load();
            _context.Requirements.Load();

            Vegetables = _context.Vegetables.Local.ToObservableCollection();
            Requirements = _context.Requirements.Local.ToObservableCollection();
            VegetableRequirements = _context.VegetableRequirements.Local.ToObservableCollection();
        }

        public void SaveDatabase()
        {
            _context.SaveChanges();
        }
    }
}
