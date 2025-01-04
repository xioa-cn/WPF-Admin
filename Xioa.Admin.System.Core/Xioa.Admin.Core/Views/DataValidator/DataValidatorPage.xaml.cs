using System.Windows;
using System.Windows.Controls;
using Xioa.Admin.Core.Views.DataValidator.ViewModel;

namespace Xioa.Admin.Core.Views.DataValidator
{
    public partial class DataValidatorPage : Page
    {
        public DataValidatorPage()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ValidatorViewModel viewModel)
            {
               // viewModel.Password = ((PasswordBox)sender).Password;
            }
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ValidatorViewModel viewModel)
            {
                //viewModel.ConfirmPassword = ((PasswordBox)sender).Password;
            }
        }
    }
}