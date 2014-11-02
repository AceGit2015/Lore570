using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GPSTrack
{
    public class SatelliteVM:BaseVM
    {
        public SatelliteVM() {
            ID = -1;
            Pitch = -1;
            Bearing = -1;
            CI = -1;
            Used = false;
        }

        public void Update(GPGSV_Satellite src)
        {
            ID = src.ID;
            Pitch = src.Pitch;
            Bearing = src.Bearing;
            CI = src.CI;
            Used = src.Used;
        }

        public int ID
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(); }
        }
        private int _id;

        public int Pitch
        {
            get { return _pitch; }
            set { _pitch = value; OnPropertyChanged(); }
        }
        private int _pitch;

        public int Bearing
        {
            get { return _bearing; }
            set { _bearing = value; OnPropertyChanged(); }
        }
        private int _bearing;

        public int CI
        {
            get { return _ci; }
            set {
                _ci = value;
                OnPropertyChanged();
                OnPropertyChanged("CI_Height");
            }
        }
        private int _ci;

        public bool Used
        {
            get { return _used; }
            set {
                _used = value;
                OnPropertyChanged();
                OnPropertyChanged("MarkBrush");
            }
        }
        private bool _used;

        public int CI_Height
        {
            get {
                if (_ci >= 0) return _ci * 6;
                else return 0;
            }
        }

        public Brush MarkBrush {
            get {
                if (_used) return Brushes.Green;
                else return Brushes.Blue;
            }
        }
    }
}
