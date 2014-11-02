using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GPSTrack
{   public class BaseVM : INotifyPropertyChanged
    {
        public BaseVM()
        {
            m_syncCtx = SynchronizationContext.Current;
        }

        // 触发属性变更事件
        protected void OnPropertyChanged([CallerMemberName]string strProperty = "NA")
        {
            if (PropertyChanged != null)
            {
                PropertyChangedEventArgs e = new PropertyChangedEventArgs(strProperty);
                PropertyChanged(this, e);
            }
        }

        // 实现INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        // 同步环境
        protected readonly SynchronizationContext m_syncCtx;
    }
}
