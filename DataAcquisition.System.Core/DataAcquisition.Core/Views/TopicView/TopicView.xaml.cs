using System.Windows.Controls;

namespace DataAcquisition.Core.Views.TopicView;

public partial class TopicView : Page
{
    public TopicView()
    {
        this.DataContext = new TopicViewModel();
        InitializeComponent();
    }

    private void Themes_Click(object sender, System.Windows.RoutedEventArgs e)
    {
       if(sender is Button button)
        {
            var content = button.Content as string;
            (this.DataContext as TopicViewModel).Use(content);
        }
    }
}