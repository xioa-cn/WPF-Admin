using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Forms;
// using Cognex.VisionPro;
// using Cognex.VisionPro.Display;
// using Cognex.VisionPro3D;
using System.Windows.Forms;
namespace Xioa.VisionView.Views
{
    /// <summary>
    /// VisionControl.xaml 的交互逻辑
    /// </summary>
    public partial class VisionControl : System.Windows.Controls.UserControl
    {
        //private CogRecordDisplay? cogDisplay;
        private System.Windows.Forms.TextBox t;

        public VisionControl()
        {
            InitializeComponent();
            t = new System.Windows.Forms.TextBox();
            //InitializeVisionPro();
        }

        private void InitializeVisionPro()
        {
            try
            {
                // cogDisplay = new CogRecordDisplay();
                // cogDisplay.BackColor = System.Drawing.Color.Black;



                //if (DisplayHost != null)
                //{
                //    DisplayHost.Child = cogDisplay;
                //}
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show($"初始化失败: {ex.Message}");
            }
        }

        private void Open_Vision(object sender, System.Windows.RoutedEventArgs e)
        {
            // Debug.WriteLine("sd");
            // CogAcqFifoEditV2 cogAcqFifoEditV2 = new CogAcqFifoEditV2();
            // this.displayhost.Child = cogAcqFifoEditV2;
        }
    }
}
