using System.ComponentModel;

namespace Xioa.Admin.Core.Views.BindingPage
{
    public partial class BindingPage
    {
        public BindingPageViewModel ViewModel { get; }

        public BindingPage()
        {
            ViewModel = new BindingPageViewModel();
            InitializeComponent();
        }
    }
}