using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcquisition.Core.Views.MainView.Model
{
    public class NaviSendMessenger<T>
    {
        public T Model { get; set; }
        public NaviSendMessenger(T data)
        {
            this.Model = data;
        }
    }
}
