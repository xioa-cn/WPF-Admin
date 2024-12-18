using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Search;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace DataAcquisition.Core.Views.ContentPage
{
    /// <summary>
    /// WriteMLPage.xaml 的交互逻辑
    /// </summary>
    public partial class WriteMLPage : Page
    {
        public WriteMLPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //快速搜索功能
            SearchPanel.Install(textEditor.TextArea);
            //设置语法规则
            string name = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".Assets.Config.Lua.xshd";
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            using (System.IO.Stream s = assembly.GetManifestResourceStream(name))
            {
                using (System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(s))
                {
                    var xshd = HighlightingLoader.LoadXshd(reader);
                    textEditor.SyntaxHighlighting = HighlightingLoader.Load(xshd, HighlightingManager.Instance);
                }
            }

        }

        private void TextEditor_LostFocus(object sender, RoutedEventArgs e)
        {
            
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;//该值确定是否可以选择多个文件
            dialog.Title = "请选择文件夹";
            dialog.Filter = "所有文件(*.cs*)|*.cs*";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file = dialog.FileName;
                textEditor.Load(file);
            }
        }
    }
}
