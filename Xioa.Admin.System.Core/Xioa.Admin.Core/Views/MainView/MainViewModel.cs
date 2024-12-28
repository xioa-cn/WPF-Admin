using CommunityToolkit.Mvvm.ComponentModel;
using Xioa.Admin.Core.Views.ContentPage;
using Xioa.Admin.Core.Views.MainView.Model;
using System.Collections.ObjectModel;
using Xioa.Admin.Core.Views.AntDiagram;
using Xioa.Admin.Core.Views.BaiDuMap;
using Xioa.Admin.Core.Views.DataSearch;
using Xioa.Admin.Core.Views.DataSkip;
using Xioa.Admin.Core.Views.DataVsualization;
using Xioa.Admin.Core.Views.DialogView;
using Xioa.Admin.Core.Views.LiveChartsTemplate;
using Xioa.Admin.Model.Model.Login;
using Xioa.Admin.Core.Views.ExcelView;
using Xioa.Admin.Core.Views.InfiniteScrolling;
using Xioa.Admin.Core.Views.QrCode;
using Xioa.Admin.Core.Views.VisionView;
using Xioa.Admin.Core.Views.WeldingMonitor;
using Xioa.Admin.Core.Views.XioaIcon;
using Xioa.Admin.Core.Views.VsuaButton;
using Xioa.Admin.Core.Views.XUserControls.ControlsPage;
using Xioa.Admin.Core.Views.RefreshTokens;

namespace Xioa.Admin.Core.Views.MainView;

/// <summary>
/// @author Xioa
/// @date  2024年11月27日
/// </summary>
public partial class MainViewModel : ObservableObject
{
    public static ObservableCollection<TreeItemModel> TreeItemModels { get; } =
        new ObservableCollection<TreeItemModel>() {
            new TreeItemModel() {
                Content = "数据管理",
                Icon = IconPaths.DataList,
                Children = {
                    new TreeItemModel() {
                        Page = new DataSearchView(),
                        Content = "数据查询",
                        Icon = IconPaths.Search
                    },
                    new TreeItemModel() {
                        Page = new DataSkipView(),
                        Content = "数据列表",
                        Icon = IconPaths.Pagination
                    },
                    new TreeItemModel() {
                        Page = new ExcelPage(),
                        Content = "Excel导入",
                        Icon = IconPaths.Excel
                    }
                }
            },
            new TreeItemModel() {
                Content = "数据分析",
                Icon = IconPaths.Charts,
                Children = {
                    new TreeItemModel() {
                        Page = new DataVisualization(),
                        Content = "数据可视化",
                        Icon = IconPaths.Visualization
                    },
                    new TreeItemModel() {
                        Page = new ChartsTest(),
                        Content = "图表分析",
                        IsPersistence = false,
                        Icon = IconPaths.Charts
                    },
                    new() {
                        Content = "焊接可视化",
                        Page = new WeldingPage(),
                        Icon = IconPaths.Visualization
                    },
                    new() {
                        Content = "可视化图形",
                        Children = {
                            new TreeItemModel() {
                                Page = new AntDiagramView(),
                                Content = "蚂蚁线",
                            }
                        }
                    }
                }
            },

            new TreeItemModel() {
                Content = "无线滚动",
                Icon = IconPaths.InfiniteScroll,
                Children = {
                    new TreeItemModel() {
                        Content = "无线滚动条",
                        Page = new InfiniteScrollingView(),
                        IsPersistence = false,
                    }
                }
            },

            new TreeItemModel() {
                Content = "组件",
                Icon = IconPaths.CustomComponent,
                Children = {
                    new TreeItemModel() {
                        Content = "登录组件",
                        Page = new UseQQComboBox(),
                    }
                }
            },
            new TreeItemModel()
            {
                Content = "服务",
                Icon= IconPaths.RefreshToken,
                Children =
                {
                    new TreeItemModel()
                    {
                        Content = "Token刷新",
                        Page =new RefreshTokenPage(),
                    }
                }
            },

            new TreeItemModel() {
                Content = "系统工具",
                Icon = IconPaths.CodeEdit,
                Children = {
                    new TreeItemModel() {
                        Page = new DialogPage(),
                        Content = "对话框"
                    },
                    new TreeItemModel() {
                        Page = new WriteMLPage(),
                        Content = "代码编辑器",
                        Icon = IconPaths.CodeEdit
                    },
                    new TreeItemModel() {
                        Page = new VsuaButtonPage(),
                        Content = "自定义按钮",
                    },
                    new TreeItemModel() {
                        Page = new IconPage(),
                        Content = "Icon"
                    },
                    new TreeItemModel() {
                        Page = new QrCodeView(),
                        Content = "二维码生成",
                        Icon = IconPaths.QrCode
                    },
                    new TreeItemModel() {
                        Page = new BaiDuMap.BaiDuMapView(),
                        Content = "网页浏览器",
                        Icon = IconPaths.WebView
                    },
                    new TreeItemModel() {
                        Content = "G-Map",
                        Children = {
                            new TreeItemModel() {
                                Page = new GMapTest(),
                                Content = "G-Map",
                                Icon = IconPaths.WebView
                            }
                        }
                    },
                    new TreeItemModel() {
                        Page = new PrintView.PrintView(),
                        Content = "Zebra-ZPL",
                        Icon = IconPaths.CodeEdit,
                    }
                }
            },
            new TreeItemModel() {
                Content = "流程设计",
                Icon = IconPaths.FlowChart,
                Children = {
                    new TreeItemModel() {
                        Page = new FlowView.FlowView(),
                        Content = "流程图编辑",
                        Icon = IconPaths.FlowChart
                    },
                    new TreeItemModel() {
                        Page = new ErrorView.ErrorView(),
                        Content = "错误诊断",
                        Icon = IconPaths.Error,
                    }
                }
            },
            new TreeItemModel() {
                Content = "软件主题",
                Icon = IconPaths.Theme,

                Children = {
                    new TreeItemModel() {
                        Content = "主题颜色",
                        Icon = IconPaths.ColorPalette,

                        Children = {
                            new TreeItemModel() {
                                Page = new TopicView.TopicView(),
                                Content = "配色方案",
                                Icon = IconPaths.ColorPalette, LoginAuth = LoginAuth.Admin,
                            }
                        }
                    }
                }
            },

            new TreeItemModel() {
                Icon = IconPaths.Carousel,
                Content = "轮播界面",
                Children = {
                    new TreeItemModel() {
                        Page = new CarouselView.CarouselView(),
                        Content = "轮播图",
                        Icon = IconPaths.Carousel
                    },
                }
            },

            new TreeItemModel() {
                Icon = IconPaths.Vision,
                Content = "视觉集成",
                Children = {
                    new TreeItemModel() {
                        Page = new VisionProView(),
                        Content = "VisionPro",
                        Icon = IconPaths.VisionPro
                    }
                }
            }
        };


    private static LoginUser? _loginUser = null;

    public static LoginUser? LoginUser
    {
        get => _loginUser;
        set
        {
            AuthChangeView();
            _loginUser = value;
        }
    }

    private static void AuthChangeView()
    {
    }
}