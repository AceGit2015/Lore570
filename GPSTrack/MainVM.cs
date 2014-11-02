using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace GPSTrack
{
    class MainVM : BaseVM
    {
        private const int m_nInterval = 200; // ms
        private string m_strLogFileName;
        private StreamReader m_fsReader;
        private Timer m_timer;
        private StringBuilder m_sbBuf;
        private int m_nBufLine = 0;
        private NEMA0183 m_nema;
        private SatelliteVM[] m_satellites = new SatelliteVM[32];

        public MainVM() {
            m_sbBuf = new StringBuilder();
            m_nema = new NEMA0183();
            m_nema.OnGPGSV += OnGPGSV;
            m_timer = new Timer(m_nInterval);
            m_timer.Elapsed += OnElapsed;
            for (int i = 0; i < m_satellites.Length; i++) {
                m_satellites[i] = new SatelliteVM();
                m_satellites[i].ID = i + 1;
            }
        }

        public string LogFileName
        {
            get { return m_strLogFileName; }
            set {
                m_strLogFileName = value;
                
                // update the show box
                m_sbBuf.Clear();
                m_nBufLine = 0;
                m_sbBuf.AppendLine(m_strLogFileName);
                m_nBufLine++;
                OnPropertyChanged("Text");
            }
        }

        public string Text
        {
            get { return m_sbBuf.ToString(); }
        }

        public void Play() {
            if (!File.Exists(m_strLogFileName)) return;

            m_sbBuf.Clear();
            m_nBufLine = 0;

            m_nema.reset();

            m_fsReader = new StreamReader(m_strLogFileName, Encoding.ASCII);
            m_timer.Start();
        }

        public void Stop() {
            m_timer.Stop();
            if (m_fsReader != null)
            {
                m_fsReader.Close();
                m_fsReader = null;
            }
        }

        private void OnElapsed(object sender, ElapsedEventArgs e)
        {
            //m_syncCtx.Send((state) =>
            //{
                
                var strLine = m_fsReader.ReadLine();
                if (strLine != null)
                {
                    m_nema.push(strLine);
                    m_sbBuf.AppendLine(strLine);
                    m_nBufLine++;
                    while (m_nBufLine > 4)
                    {
                        // only keep 4 lines
                        var pos = m_sbBuf.ToString().IndexOf("\n");
                        if (pos >= 0)
                        {
                            m_sbBuf.Remove(0, pos + 1);
                            m_nBufLine--;
                        }
                    }
                    OnPropertyChanged("Text");
                }
                else
                {
                    this.Stop();
                }
            //}, null);
        }

        private void OnGPGSV(object sender, GPGSV dataGSV)
        {   
            foreach (var satellite in dataGSV.Satellites)
            {
                var id = satellite.ID;
                if (id >= 0 && id <= 32)
                {
                    m_satellites[id-1].Update(satellite);
                }
            }
        }

        public SatelliteVM[] GSV
        {
            get { return m_satellites; }
        }
    }
}
