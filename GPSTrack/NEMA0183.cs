using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPSTrack
{
    class NEMA0183
    {
        private char[] _sep;
        private char[] _sepGSV;

        private GPGSV _dataGSV;
        private int[] _dataGSA;

        public NEMA0183()
        {
            _sep = new char[]{','};
            _sepGSV = new char[]{',', '*'};

            reset();
        }

        public void reset()
        {
            _dataGSV = new GPGSV();
            _dataGSA = new int[32];
        }

        public GPGSV DataGSV {
            get { return _dataGSV; }
        }

        public string push(string buf)
        {
            if (buf == null || buf.Length == 0 || !buf.StartsWith("$")) return null;


            var GPX = buf.Split(_sep, 2);
            if (GPX[0] == "$GPGSV") {
                return _GPGSV(buf);
            }
            else if (GPX[0] == "$GPGSA")
            {
                return _GPGSA(buf);
            }

            return null;
        }

        public event EventHandler<GPGSV> OnGPGSV;
        private string _GPGSV(string data)
        {
            var GPX = data.Split(_sepGSV);
            try{
                _dataGSV.TotalGSV = Int32.Parse(GPX[1]);
                _dataGSV.CurrentGSV = Int32.Parse(GPX[2]);
                if (_dataGSV.CurrentGSV == 1) {
                    // the 1st frame
                    _dataGSV.Satellites.Clear();
                }
                // _dataGSV.Count = Int32.Parse(GPX[3]);

                var i = 4;
                int value;
                while(GPX.Length - i > 4){
                    var satellite = new GPGSV_Satellite();
                    if (Int32.TryParse(GPX[i + 0], out value)) satellite.ID = value;
                    if (Int32.TryParse(GPX[i + 1], out value)) satellite.Pitch = value;
                    if (Int32.TryParse(GPX[i + 2], out value)) satellite.Bearing = value;
                    if (Int32.TryParse(GPX[i + 3], out value)) satellite.CI = value;
                    _dataGSV.Satellites.Add(satellite);
                    i += 4;
                }
                MarkUsed();

                // fire event after received the last frame
                if (_dataGSV.TotalGSV == _dataGSV.CurrentGSV)
                {
                    if (OnGPGSV != null)
                    {
                        OnGPGSV(this, _dataGSV);
                    }
                }

                return "$GPGSV";
            }
            catch(Exception ex){
                // invalid data
                Debugger.Log(1, "GPG", ex.Message);
                return null;
            }
        }

        private string _GPGSA(string data)
        {
            var GPX = data.Split(_sepGSV);
            try
            {
                // reset used
                for (var i = 0; i < _dataGSA.Length; i++) _dataGSA[i] = 0;

                // mark used
                int id;
                for (var i = 3; i < 15; i++) {
                    if (Int32.TryParse(GPX[i], out id)) {
                        if(id > 0 && id<= 32) _dataGSA[id - 1] = 1;
                    }
                }

                MarkUsed();

                // fire event
                OnGPGSV(this, _dataGSV);

                return "$GPGSA";
            }
            catch (Exception ex)
            {
                // invalid data
                Debugger.Log(1, "GPG", ex.Message);
                return null;
            }
        }

        private void MarkUsed()
        {
            foreach(var s in _dataGSV.Satellites){
                if(s.ID > 0 && s.ID <= 32)
                    s.Used = (_dataGSA[s.ID - 1] != 0);
            }
        }
    }

    class GPGSV
    {
        public int TotalGSV { get; set; }
        public int CurrentGSV { get; set; }

        public List<GPGSV_Satellite> Satellites = new List<GPGSV_Satellite>();
    }

    public class GPGSV_Satellite
    {
        public GPGSV_Satellite()
        {
            ID = 0;
            Pitch = -1;
            Bearing = -1;
            CI = -1;
            Used = false;
        }

        public int ID { get; set; }
        public int Pitch { get; set; }
        public int Bearing { get; set; }
        public int CI { get; set; }
        public bool Used { get; set; }
    }
}
