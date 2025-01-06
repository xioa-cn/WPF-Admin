using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Forms.VisualStyles;
using CommunityToolkit.Mvvm.ComponentModel;
using Xioa.Admin.Core.Views.VirtualizingList.Model;

namespace Xioa.Admin.Core.Views.VirtualizingList.ViewModel;

public partial class VirtualizingListViewModel : ObservableObject
{
    public ObservableCollection<ItemModel> Items { get; }

        = new ObservableCollection<ItemModel>();

    public VirtualizingListViewModel()
    {

    }



    public void AddMoreItems(int count)
    {
        var nowCount = Items.Count;
        for (int i = Items.Count + 1; i <= nowCount + count; i++)
        {
            Items.Add(new ItemModel
            {
                Title = $"项目 {i}",
                Description = $"这是第 {i} 个项目的详细描述，包含更多文本来增加内存占用。" +
                             $"额外的描述信息：{i}"
            });
        }
    }
}