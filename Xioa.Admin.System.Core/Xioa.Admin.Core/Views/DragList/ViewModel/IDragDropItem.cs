namespace Xioa.Admin.Core.Views.DragList.ViewModel;

public interface IDragDropItem
{
    // 如果需要自定义显示文本，可以实现这个属性
    string DisplayText { get; }
}