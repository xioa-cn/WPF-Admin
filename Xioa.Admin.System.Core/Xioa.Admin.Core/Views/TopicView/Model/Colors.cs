using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xioa.Admin.Core.Views.TopicView.Model
{
    public class SystemtColors
    {
        public string Demo1 { get; set; }
        public string Demo2 { get; set; }
        public string Demo3 { get; set; }
        public string Demo4 { get; set; }
        public string Demo5 { get; set; }
        public SystemtColors(string demo1, string demo2, string demo3, string demo4, string demo5)
        {
            this.Demo1 = demo1;
            this.Demo2 = demo2;
            this.Demo3 = demo3;
            this.Demo4 = demo4;
            this.Demo5 = demo5;
        }
    }
}
