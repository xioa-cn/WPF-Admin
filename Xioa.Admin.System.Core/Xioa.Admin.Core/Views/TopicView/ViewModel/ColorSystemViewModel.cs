using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xioa.Admin.Core.Views.TopicView.Model;

namespace Xioa.Admin.Core.Views.TopicView.ViewModel
{

    public partial class ColorSystemViewModel : Xioa.Admin.Core.Services.ViewModels.ViewModelBase
    {
        public ObservableCollection<SystemtColors> Colors { get; set; } = new ObservableCollection<SystemtColors>();
        public ColorSystemViewModel()
        {
            InitializedColors();
        }

        private void InitializedColors()
        {
            Colors.Add(new SystemtColors("#4885A4", "#72A2BB", "#9CBED2", "#C6DBE8", "#F0F7FF"));
            Colors.Add(new SystemtColors("#4A3C2B", "#766136", "#AD9056", "#BCA052", "#D5C7A8"));
            Colors.Add(new SystemtColors("#2D584B", "#448870", "#87C1AA", "#B4DBCA", "#DAEDE4"));
            Colors.Add(new SystemtColors("#EFFBF2", "#DAF9E3", "#B9F0C7", "#86E49E", "#54CD72"));
            Colors.Add(new SystemtColors("#1A4526", "#1E542C", "#216C34", "#278D40", "#33B051"));
            Colors.Add(new SystemtColors("#0971B4", "#3794C4", "#66B8D4", "#94DCE4", "#C3FFF4"));
            Colors.Add(new SystemtColors("#00999F", "#2FAEAD", "#5EC3BC", "#8DD8CA", "#BCEDD8"));
            Colors.Add(new SystemtColors("#E3FAFC", "#C5F6FA", "#99E9F2", "#66D9E8", "#3BC9DB"));
            Colors.Add(new SystemtColors("#22B8CF", "#15AABF", "#1098AD", "#0C8599", "#0B7285"));
            Colors.Add(new SystemtColors("#E6FCF5", "#C3FAE8", "#96F2D7", "#63E6BE", "#38D9A9"));
            Colors.Add(new SystemtColors("#087F5B", "#099268", "#0CA678", "#12B886", "#20C997"));
            Colors.Add(new SystemtColors("#EBFBEE", "#D3F9D8", "#B2F2BB", "#8CE99A", "#69DB7C"));
            Colors.Add(new SystemtColors("#2B8A3E", "#2F9E44", "#37B24D", "#40C057", "#51CF66"));
            Colors.Add(new SystemtColors("#F4FCE3", "#E9FAC8", "#D8F5A2", "#C0EB75", "#A9E34B"));
            Colors.Add(new SystemtColors("#5C940D", "#66A80F", "#74B816", "#82C91E", "#94D82D"));
            Colors.Add(new SystemtColors("#FFF9DB", "#FFF3BF", "#FFEC99", "#FFE066", "#FFD43B"));
            Colors.Add(new SystemtColors("#E67700", "#F08C00", "#F59F00", "#FAB005", "#FCC419"));
            Colors.Add(new SystemtColors("#FFF4E6", "#FFE8CC", "#FFD8A8", "#FFC078", "#FFA94D"));
            Colors.Add(new SystemtColors("#D9480F", "#E8590C", "#F76707", "#FD7E14", "#FF922B"));
            Colors.Add(new SystemtColors("#F3F0FF", "#E5DBFF", "#D0BFFF", "#B197FC", "#9775FA"));
            Colors.Add(new SystemtColors("#5F3DC4", "#6741D9", "#7048E8", "#7950F2", "#845EF7"));
            Colors.Add(new SystemtColors("#F8F0FC", "#F3D9FA", "#EEBEFA", "#E599F7", "#DA77F2"));
            Colors.Add(new SystemtColors("#862E9C", "#9C36B5", "#AE3EC9", "#BE4BDB", "#CC5DE8"));
            Colors.Add(new SystemtColors("#FFF0F6", "#FFDEEB", "#FCC2D7", "#FAA2C1", "#F783AC"));
            Colors.Add(new SystemtColors("#A61E4D", "#C2255C", "#D6336C", "#E64980", "#F06595"));
            Colors.Add(new SystemtColors("#FFF5F5", "#FFE3E3", "#FFC9C9", "#FFA8A8", "#FF8787"));
            Colors.Add(new SystemtColors("#C92A2A", "#E03131", "#F03E3E", "#FA5252", "#FF6B6B"));
        }
    }
}
