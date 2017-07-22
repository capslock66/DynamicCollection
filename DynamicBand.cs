using System.Collections.ObjectModel;

namespace DynamicCollection
{
    public class DynamicBand : ObservableObject
    {
        public DynamicBand()
        {
            OverlayHeaderByChildren = false;
            Columns = new ObservableCollection<DynamicColumn>();
            Bands   = new ObservableCollection<DynamicBand>();
            Width   = -1 ;
            Visible = true ;
        }

        public string BandHeader { get; set; }
        public bool Visible {get;set; }
        public int Width {get;set; }
        public ObservableCollection<DynamicColumn> Columns { get; set; }
        public ObservableCollection<DynamicBand> Bands { get; set; }
        public bool OverlayHeaderByChildren {get;set;}
    }
}
