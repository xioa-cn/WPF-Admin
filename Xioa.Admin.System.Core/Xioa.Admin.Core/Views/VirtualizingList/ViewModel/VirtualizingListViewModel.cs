using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms.VisualStyles;
using CommunityToolkit.Mvvm.ComponentModel;
using Xioa.Admin.Core.Views.VirtualizingList.Model;

namespace Xioa.Admin.Core.Views.VirtualizingList.ViewModel;

public partial class VirtualizingListViewModel : Xioa.Admin.Core.Services.ViewModels.ViewModelBase
{
    public ObservableCollection<ItemModel> Items { get; }

        = new ObservableCollection<ItemModel>();

    public VirtualizingListViewModel()
    {
    }
    // 虚拟化的核心原理
    // 基本概念
    // 虚拟化是一种"按需渲染"的机制
    // 只为可见区域的数据项创建UI元素
    // 数据本身完整存在，但UI元素按需创建和销毁
    //
    // 核心组件
    // ItemContainerGenerator: 负责创建和回收项容器
    // VirtualizingPanel: 管理虚拟化布局的面板
    // IScrollInfo: 处理滚动相关的信息和操作
    //
    // 工作流程
    // 数据项 → 计算可见范围 → 创建/重用容器 → 渲染可见项
    //  ↑                                         |
    //  └─────────── 容器回收 ←─────────────────┘
    //
    //  关键步骤
    //  计算可见范围：根据滚动位置确定哪些项应该显示
    //         如果每项高度是50像素
    //         视口高度是300像素
    //             滚动位置是100像素
    //         那么：
    //             起始索引 = 100 / 50 = 2 (从第3项开始)
    //             可见项数 = 300 / 50 + 1 = 7 项
    //             结束索引 = 2 + 7 = 9
    //  容器管理：创建新容器或重用已有容器
    //         容器管理主要通过 ItemContainerGenerator 实现
    //         容器池机制
    //         ┌─────────────────────────┐
    //         │       容器池           │
    //         ├─────────────────────────┤
    //         │ Container1 (可重用)     │
    //         │ Container2 (可重用)     │
    //         │ Container3 (可重用)     │
    //         └─────────────────────────┘
    //         生命周期管理
    //         数据项 → 检查容器池 → 存在可重用容器？
    //         │               ├── 是 → 重用容器
    //         │               └── 否 → 创建新容器
    //         ↓
    //         绑定数据到容器
    //         ↓
    //         项滚出可视区域
    //         ↓
    //         容器回收到容器池
    //         回收策略
    //         当前可见：    [4] [5] [6] [7] [8]
    //         向下滚动后：  [6] [7] [8] [9] [10]
    //         ↑
    //         [4][5] 的容器被回收并重用于 [9][10]
    //  布局计算：确定每个可见项的位置
    //         坐标计算
    //         ┌─────────────────┐ ← Y = 0
    //         │     项目1       │ 
    //         │   (高度50px)    │
    //         ├─────────────────┤ ← Y = 50
    //         │     项目2       │    
    //         │   (高度50px)    │
    //         ├─────────────────┤ ← Y = 100
    //         │     项目3       │
    //         │   (高度50px)    │
    //         └─────────────────┘ ← Y = 150
    //         位置映射
    //             实际位置 = 索引 × 项目高度 - 滚动偏移量
    //             例如：
    //             项目3的位置 = 2(索引) × 50(高度) - 20(滚动) = 80px
    //         布局类型 常见的 线性布局 网格布局
    //         布局步骤 (WPF布局系统主要阶段 Measure 阶段（测量阶段） Arrange 阶段（排列阶段） Render 阶段（渲染阶段）)
    //         1. Measure阶段：测量每项所需空间
    //         2. Arrange阶段：确定每项的最终位置
    //         3. 应用偏移：根据滚动位置调整
    //         4. 渲染：将项绘制到计算好的位置
    //  回收机制：移除不可见项的UI元素，但保留数据
    //         回收机制主要通过 ItemContainerGenerator 和 VirtualizingPanel 协同工作来实现
    //             容器池设计
    //             ┌─────────────────────────┐
    //             │       活动容器          │  当前显示的项容器
    //             ├─────────────────────────┤
    //             │ Item3 -> Container1    │
    //             │ Item4 -> Container2    │
    //             │ Item5 -> Container3    │
    //             ├─────────────────────────┤
    //             │       回收池            │  等待重用的容器
    //             ├─────────────────────────┤
    //             │ Container4 (空闲)      │
    //             │ Container5 (空闲)      │
    //             └─────────────────────────┘
    //          回收过程
    //             滚动前：[3] [4] [5]  // 显示项3,4,5
    //             ↓ 向下滚动
    //             滚动后：[4] [5] [6]  // 项3的容器被回收并重用于项6
    //          回收策略
    //             当项滚出可视区域时：
    //             1. 解除数据绑定
    //             2. 清理状态
    //             3. 放入回收池
    //             4. 等待下次重用
    //          重用流程
    //             需要新容器时：
    //             1. 检查回收池
    //             2. 如果有空闲容器：
    //              取出容器
    //              重置状态
    //              绑定新数据
    //             3. 如果没有空闲容器：
    //              创建新容器
    //
    //  性能优化
    //  容器重用：避免频繁创建和销毁UI元素
    //  延迟滚动：滚动时延迟更新内容
    //     滚动时不立即渲染所有内容
    //     使用占位符替代实际内容
    //     滚动过程：
    //           快速滚动 → 显示占位符 → 滚动减速/停止 → 渲染实际内容
    //              显示占位符原理：
    //              监测滚动速度
    //              快速滚动时显示简单占位符
    //              滚动停止后再加载实际内容
    //              实现思路：
    //              ┌─────────────────────┐
    //              │ 正常滚动           │ → 显示完整内容
    //              ├─────────────────────┤
    //              │ 快速滚动(>阈值)    │ → 切换为占位符
    //              │  ┌───────────┐     │
    //              │  │ 占位符    │     │
    //              │  └───────────┘     │
    //              ├─────────────────────┤
    //              │ 滚动停止          │ → 恢复完整内容
    //              └─────────────────────┘
    //              占位符策略
    //              轻量级占位符：
    //                只显示基本框架
    //                不加载图片
    //                使用纯色块
    //                简化布局
    //              重点区域：
    //                保持标题等关键信息
    //                次要信息使用占位符
    //                图片区域显示loading
    //              优点：
    //                减少滚动时的渲染负担
    //                提高滚动流畅度
    //                降低内存压力              
    //              缺点：
    //                可能出现闪烁
    //                需要额外的状态管理
    //                用户体验需要权衡
    //  缓存机制：预加载部分不可见项
    //     在可视区域前后保留一定数量的已实例化项
    //     缓存区域布局：
    //          [缓存区域-上方]
    //          [可视区域]
    //          [缓存区域-下方]
    //     缓存策略
    //          预加载：提前创建即将显示的项
    //          延迟清理：不立即销毁刚滚出的项
    //          动态调整：根据滚动速度调整缓存量
    //

    // 缓存控制
    // VirtualizingPanel.CacheLength="2" - 缓存长度（可以是"2,2"表示前后各2页）
    // VirtualizingPanel.CacheLengthUnit= "Page" - 缓存单位（Page或Item）
    // 滚动优化
    // ScrollViewer.CanContentScroll= "True" - 启用内容级滚动
    // ScrollViewer.IsDeferredScrollingEnabled= "True" - 延迟滚动更新
    // VirtualizingPanel.ScrollUnit= "Pixel" - 像素级滚动


    // ┌─────────────────┐
    // │    数据层       │ 100,000 条数据
    // ├─────────────────┤
    // │    UI层         │ 仅 20-30 个UI元素
    // └─────────────────┘





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