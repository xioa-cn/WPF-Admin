using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace Xioa.Admin.Core.Views.XUserControls.Components.QQComboBox;

 [TemplatePart(Name = "PART_ItemsControl", Type = typeof(ListBox))]
    [TemplatePart(Name = "PART_DropDownButton", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]

    public class QQComboBox : ComboBox
    {
        static QQComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(QQComboBox),
                new FrameworkPropertyMetadata(typeof(QQComboBox)));
        }

        #region 依赖属性
        private ItemsControl _itemsControl;
        private ToggleButton _togglrButton;
        private Popup _popup;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _togglrButton = GetTemplateChild("PART_DropDownButton") as ToggleButton;
            if (_togglrButton != null)
                _togglrButton.Click += _togglrButton_Click;

            _itemsControl = GetTemplateChild("PART_ItemsControl") as ItemsControl;

            if (_popup != null)
            {
                _popup.Opened -= OnPopupOpened;
            }
            _popup = GetTemplateChild("PART_Popup") as Popup;
            if (_popup != null)
            {

                _popup.Opened += OnPopupOpened;
            }
            //_itemsControl.SelectionChanged += _itemsControl_SelectionChanged;

        }
        private void OnPopupOpened(object sender, EventArgs e)
        {
            // 添加事件处理
            Window window = Window.GetWindow(this);
            if (window != null)
            {
                window.PreviewMouseDown += OnWindowPreviewMouseDown;
            }
        }

        private void OnWindowPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_popup != null && _popup.IsOpen)
            {
                // 检查点击是否在Popup外部
                Point mousePosition = e.GetPosition(_popup);
                FrameworkElement popupChild = _popup.Child as FrameworkElement;

                if (popupChild != null)
                {
                    bool isInside = mousePosition.X >= 0 &&
                                   mousePosition.X <= popupChild.ActualWidth &&
                                   mousePosition.Y >= 0 &&
                                   mousePosition.Y <= popupChild.ActualHeight + 20;

                    if (!isInside)
                    {
                        _popup.IsOpen = false;
                        IsDropDownOpen = false;
                        _togglrButton.IsChecked = _popup.IsOpen;
                        // 移除事件处理
                        Window window = Window.GetWindow(this);
                        if (window != null)
                        {
                            window.PreviewMouseDown -= OnWindowPreviewMouseDown;
                        }
                    }
                }
            }
        }
        private void _togglrButton_Click(object sender, RoutedEventArgs e)
        {
            if (_popup == null) return;
            _popup.IsOpen = !_popup.IsOpen;
            _togglrButton.IsChecked = _popup.IsOpen;
        }

        private void Popup_Opened(object sender, EventArgs e)
        {
            Mouse.Capture(_popup, CaptureMode.SubTree);
            Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(_popup, OnMouseDownOutsidePopup);
        }

        private void Popup_Closed(object sender, EventArgs e)
        {
            Mouse.RemovePreviewMouseDownOutsideCapturedElementHandler(_popup, OnMouseDownOutsidePopup);
            Mouse.Capture(null);
        }

        private void OnMouseDownOutsidePopup(object sender, MouseButtonEventArgs e)
        {
            _popup.IsOpen = false;
        }
        private void _itemsControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var res = _itemsControl.SelectedValue;
            //if (res != null)
            //    InputText = res.ToString();
            //_togglrButton.IsChecked = false;
        }

        public ICommand SelectCommand
        {
            get { return (ICommand)GetValue(SelectCommandProperty); }
            set { SetValue(SelectCommandProperty, value); }
        }

        public QQComboBox()
        {
            SelectCommand = new RelayCommand<string>(Select);
            //DeleteCommand = new RelayCommand<object>(Delete);
        }

        private void Delete(object? obj)
        {
            Items.Remove(obj);
        }

        private void Select(string value)
        {
            InputText = value;
        }

        public string InputText
        {
            get { return (string)GetValue(InputTextProperty); }
            set { SetValue(InputTextProperty, value); }
        }

        public static readonly DependencyProperty InputTextProperty =
            DependencyProperty.Register("InputText",
                typeof(string),
                typeof(QQComboBox),
                new PropertyMetadata(string.Empty));

        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }

        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register("DisplayMemberPath",
                typeof(string),
                typeof(QQComboBox),
                new PropertyMetadata(string.Empty));

        #endregion

        #region 命令



        #endregion


        // 在自定义控件类中定义命令
        public static readonly DependencyProperty SelectCommandProperty =
            DependencyProperty.Register("SelectCommand", typeof(ICommand), typeof(QQComboBox));



        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register("DeleteCommand", typeof(ICommand), typeof(QQComboBox));

        public ICommand DeleteCommand
        {
            get { return (ICommand)GetValue(DeleteCommandProperty); }
            set { SetValue(DeleteCommandProperty, value); }
        }
    }