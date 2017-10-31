using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using Css.Wpf.UI.Models;

namespace Css.Wpf.UI.Controls
{
    /// <summary>
    /// 表单布局面板
    /// </summary>
    [ContentProperty("Children")]
    public class FormPanel : HeaderedContentControl
    {
        Grid InnerGrid { get; set; }
        /// <summary>
        /// 子控件集
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<FrameworkElement> Children { get; private set; }

        static readonly DependencyProperty AttachedLabelProperty;
        /// <summary>
        /// 标题
        /// </summary>
        public static readonly DependencyProperty LabelProperty;
        /// <summary>
        /// 是否显示标题
        /// </summary>
        public static readonly DependencyProperty ShowLabelProperty;
        /// <summary>
        /// 显示顺序
        /// </summary>
        public static readonly DependencyProperty DisplayIndexProperty;
        /// <summary>
        /// 列跨度
        /// </summary>
        public static readonly DependencyProperty ColumnSpanProperty;
        /// <summary>
        /// 行跨度
        /// </summary>
        public static readonly DependencyProperty RowSpanProperty;
        /// <summary>
        /// 行高
        /// </summary>
        public static readonly DependencyProperty RowHeightProperty;
        /// <summary>
        /// 最小行高
        /// </summary>
        public static readonly DependencyProperty RowMinHeightProperty;
        /// <summary>
        /// 最大行高
        /// </summary>
        public static readonly DependencyProperty RowMaxHeightProperty;
        /// <summary>
        /// 列数
        /// </summary>
        public static readonly DependencyProperty ColumnsProperty;
        /// <summary>
        /// 默认行高
        /// </summary>
        public static readonly DependencyProperty DefaultRowHeightProperty;
        /// <summary>
        /// 标题水平对齐
        /// </summary>
        public static readonly DependencyProperty LabelHorizontalAlignmentProperty;
        /// <summary>
        /// 标题垂直对齐
        /// </summary>
        public static readonly DependencyProperty LabelVerticalAlignmentProperty;
        /// <summary>
        /// 列最小宽度
        /// </summary>
        public static readonly DependencyProperty ColumnMinWidthProperty;
        /// <summary>
        /// 列最大宽度
        /// </summary>
        public static readonly DependencyProperty ColumnMaxWidthProperty;
        /// <summary>
        /// 列间距
        /// </summary>
        public static readonly DependencyProperty ColumnSpacingProperty;
        /// <summary>
        /// 行间距
        /// </summary>
        public static readonly DependencyProperty RowSpacingProperty;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetLabel(DependencyObject obj, object value)
        {
            obj.SetValue(LabelProperty, value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object GetLabel(DependencyObject obj)
        {
            return obj.GetValue(LabelProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetDisplayIndex(DependencyObject obj, int value)
        {
            obj.SetValue(DisplayIndexProperty, value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int GetDisplayIndex(DependencyObject obj)
        {
            return (int)obj.GetValue(DisplayIndexProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetRowSpan(DependencyObject obj, int value)
        {
            obj.SetValue(RowSpanProperty, value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int GetRowSpan(DependencyObject obj)
        {
            return (int)obj.GetValue(RowSpanProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetShowLabel(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowLabelProperty, value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetShowLabel(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowLabelProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetColumnSpan(DependencyObject obj, int value)
        {
            obj.SetValue(ColumnSpanProperty, value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int GetColumnSpan(DependencyObject obj)
        {
            return (int)obj.GetValue(ColumnSpanProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetRowHeight(DependencyObject obj, GridLength value)
        {
            obj.SetValue(RowHeightProperty, value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static GridLength GetRowHeight(DependencyObject obj)
        {
            return (GridLength)obj.GetValue(RowHeightProperty);
        }

        static void SetAttachedLabel(DependencyObject obj, Label value)
        {
            obj.SetValue(AttachedLabelProperty, value);
        }

        static Label GetAttachedLabel(DependencyObject obj)
        {
            return (Label)obj.GetValue(AttachedLabelProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public double ColumnMinWidth
        {
            get { return (double)GetValue(ColumnMinWidthProperty); }
            set { SetValue(ColumnMinWidthProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public double ColumnMaxWidth
        {
            get { return (double)GetValue(ColumnMaxWidthProperty); }
            set { SetValue(ColumnMaxWidthProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public double DefaultRowHeight
        {
            get { return (double)GetValue(DefaultRowHeightProperty); }
            set { SetValue(DefaultRowHeightProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public double ColumnSpacing
        {
            get { return (double)GetValue(ColumnSpacingProperty); }
            set { SetValue(ColumnSpacingProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public double RowSpacing
        {
            get { return (double)GetValue(RowSpacingProperty); }
            set { SetValue(RowSpacingProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public HorizontalAlignment LabelHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(LabelHorizontalAlignmentProperty); }
            set { SetValue(LabelHorizontalAlignmentProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public VerticalAlignment LabelVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(LabelVerticalAlignmentProperty); }
            set { SetValue(LabelVerticalAlignmentProperty, value); }
        }

        static FormPanel()
        {
            Type thisType = typeof(FormPanel);

            DefaultStyleKeyProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(thisType));

            AttachedLabelProperty = DependencyProperty.RegisterAttached("AttachedLabel", typeof(Label), thisType, new FrameworkPropertyMetadata(null));

            LabelProperty = DependencyProperty.RegisterAttached("Label", typeof(object), thisType, new FrameworkPropertyMetadata(null, OnLabelChanged));
            ShowLabelProperty = DependencyProperty.RegisterAttached("ShowLabel", typeof(bool), thisType, new FrameworkPropertyMetadata(true));
            DisplayIndexProperty = DependencyProperty.RegisterAttached("DisplayIndex", typeof(int), thisType, new FrameworkPropertyMetadata(-1));
            RowSpanProperty = DependencyProperty.RegisterAttached("RowSpan", typeof(int), thisType, new FrameworkPropertyMetadata(1, null, OnCoerceValue));
            ColumnSpanProperty = DependencyProperty.RegisterAttached("ColumnSpan", typeof(int), thisType, new FrameworkPropertyMetadata(1, null, OnCoerceValue));
            RowHeightProperty = DependencyProperty.RegisterAttached("RowHeight", typeof(GridLength), thisType, new FrameworkPropertyMetadata(new GridLength(-1)));

            ColumnsProperty = DependencyProperty.Register("Columns", typeof(int), thisType, new FrameworkPropertyMetadata(1, OnColumnChanged, OnCoerceValue));
            DefaultRowHeightProperty = DependencyProperty.Register("DefaultRowHeight", typeof(double), thisType, new FrameworkPropertyMetadata(26d, FrameworkPropertyMetadataOptions.AffectsMeasure));
            LabelHorizontalAlignmentProperty = DependencyProperty.Register("LabelHorizontalAlignment", typeof(HorizontalAlignment), thisType, new FrameworkPropertyMetadata(HorizontalAlignment.Right, FrameworkPropertyMetadataOptions.AffectsMeasure));
            LabelVerticalAlignmentProperty = DependencyProperty.Register("LabelVerticalAlignment", typeof(VerticalAlignment), thisType, new FrameworkPropertyMetadata(VerticalAlignment.Center, FrameworkPropertyMetadataOptions.AffectsMeasure));
            ColumnMinWidthProperty = DependencyProperty.Register("ColumnMinWidth", typeof(double), thisType, new FrameworkPropertyMetadata(50d, OnColumnChanged));
            ColumnMaxWidthProperty = DependencyProperty.Register("ColumnMaxWidth", typeof(double), thisType, new FrameworkPropertyMetadata(300d, OnColumnChanged));
            RowSpacingProperty = DependencyProperty.Register("RowSpacing", typeof(double), thisType, new FrameworkPropertyMetadata(6.0, FrameworkPropertyMetadataOptions.AffectsMeasure, null, OnCoerceSpacingValue));
            ColumnSpacingProperty = DependencyProperty.Register("ColumnSpacing", typeof(double), thisType, new FrameworkPropertyMetadata(15.0, OnColumnChanged, OnCoerceSpacingValue));
        }

        static object OnCoerceSpacingValue(DependencyObject d, object baseValue)
        {
            var value = (double)baseValue;
            return Math.Max(0, value);
        }

        static object OnCoerceValue(DependencyObject d, object baseValue)
        {
            var value = (int)baseValue;
            return Math.Max(1, value);
        }

        static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var label = GetAttachedLabel(d);
            if (label != null)
                label.Content = e.NewValue;
        }

        static void OnColumnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var form = d as FormPanel;
            form.GenerateColumns();
            form.InvalidateMeasure();
        }
        /// <summary>
        /// 表单布局
        /// </summary>
        public FormPanel()
        {
            InnerGrid = new Grid();
            Content = InnerGrid;
            //Grid.ShowGridLines = true;
            Children = new ObservableCollection<FrameworkElement>();
            Children.CollectionChanged += new NotifyCollectionChangedEventHandler(Children_CollectionChanged);
        }


        Dictionary<UIElement, Dictionary<string, PropertyChangeNotifier>> notifiers = new Dictionary<UIElement, Dictionary<string, PropertyChangeNotifier>>();
        void AddValueChanged(UIElement el, DependencyProperty property, EventHandler handler)
        {
            Dictionary<string, PropertyChangeNotifier> values;
            if (!notifiers.TryGetValue(el, out values))
            {
                values = new Dictionary<string, PropertyChangeNotifier>();
                notifiers.Add(el, values);
            }
            var notifier = new PropertyChangeNotifier(el, property);
            notifier.ValueChanged += handler;
            values.Add(property.Name, notifier);
        }

        void RemoveValueChanged(UIElement el, DependencyProperty property, EventHandler handler)
        {
            Dictionary<string, PropertyChangeNotifier> values;
            if (notifiers.TryGetValue(el, out values))
            {
                PropertyChangeNotifier notifier;
                if (values.TryGetValue(property.Name, out notifier))
                {
                    notifier.ValueChanged -= handler;
                    notifier.Dispose();
                    values.Remove(property.Name);
                }
                if (!values.Any())
                {
                    notifiers.Remove(el);
                }
            }
        }

        void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add
                || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (UIElement el in e.NewItems)
                {
                    AddValueChanged(el, VisibilityProperty, OnChildVisibleChanged);
                    AddValueChanged(el, RowSpanProperty, OnChildChanged);
                    AddValueChanged(el, ColumnSpanProperty, OnChildChanged);
                    AddValueChanged(el, DisplayIndexProperty, OnChildChanged);
                    AddValueChanged(el, RowHeightProperty, OnChildChanged);
                    AddValueChanged(el, ShowLabelProperty, OnChildChanged);

                    InnerGrid.Children.Add(el);
                    InnerGrid.Children.Add(CreateLabel(el));
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove
                || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (UIElement el in e.OldItems)
                {
                    RemoveValueChanged(el, VisibilityProperty, OnChildVisibleChanged);
                    RemoveValueChanged(el, RowSpanProperty, OnChildChanged);
                    RemoveValueChanged(el, ColumnSpanProperty, OnChildChanged);
                    RemoveValueChanged(el, DisplayIndexProperty, OnChildChanged);
                    RemoveValueChanged(el, RowHeightProperty, OnChildChanged);
                    RemoveValueChanged(el, ShowLabelProperty, OnChildChanged);

                    InnerGrid.Children.Remove(el);
                    InnerGrid.Children.Remove(GetAttachedLabel(el));
                }
            }
            InvalidateMeasure();
        }

        void OnChildVisibleChanged(object s, EventArgs e)
        {
            var el = (s as PropertyChangeNotifier).PropertySource as UIElement;
            GetAttachedLabel(el).Visibility = el.Visibility;
            InvalidateMeasure();
        }

        void OnChildChanged(object s, EventArgs e)
        {
            InvalidateMeasure();
        }

        void GenerateRows(int rowCount)
        {
            InnerGrid.RowDefinitions.Clear();
            while (rowCount * 2 > InnerGrid.RowDefinitions.Count - 1)
            {
                InnerGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(DefaultRowHeight) });
                if (rowCount * 2 > InnerGrid.RowDefinitions.Count)
                    InnerGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(RowSpacing) });
            }
        }

        void GenerateColumns()
        {
            InnerGrid.ColumnDefinitions.Clear();
            for (int i = 0; i < Columns; i++)
            {
                InnerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                InnerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(ColumnMinWidth), MaxWidth = ColumnMaxWidth, MinWidth = ColumnMinWidth });
                if (i < Columns - 1)
                    InnerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(ColumnSpacing) });
            }
            InnerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        }

        Label CreateLabel(UIElement element)
        {
            var content = element.GetValue(LabelProperty);
            Label label = new Label();
            SetAttachedLabel(element, label);
            label.Content = content;
            label.SetBinding(Label.VerticalAlignmentProperty, new Binding("LabelVerticalAlignment") { Source = this });
            label.SetBinding(Label.HorizontalAlignmentProperty, new Binding("LabelHorizontalAlignment") { Source = this });
            label.Padding = new Thickness(0);
            label.Margin = new Thickness(0, 0, 5, 0);
            return label;
        }

        void UpdateGridLayout()
        {
            var children = Children
                .Where(p => p.Visibility != System.Windows.Visibility.Collapsed)
                .OrderBy(p => GetDisplayIndex(p)).ToList();
            int rowCapacity = children.Sum(p => Math.Max(1, GetRowSpan(p)));
            bool[,] matrix = new bool[rowCapacity, Columns];
            int rowIndex = 0;
            int columnIndex = 0;
            int rowTotal = 0;
            foreach (FrameworkElement el in children)
            {
                var rowSpan = Math.Max(1, GetRowSpan(el));
                var columnSpan = Math.Max(1, Math.Min(Columns, GetColumnSpan(el)));
                while (true)
                {
                    if (Columns < columnIndex + columnSpan)
                    {
                        rowIndex++;
                        columnIndex = 0;
                    }
                    bool exist = false;
                    for (int j = 0; j < columnSpan; j++)
                        exist |= matrix[rowIndex, columnIndex + j];
                    if (!exist)
                        break;
                    columnIndex++;
                }
                rowTotal = Math.Max(rowTotal, rowIndex + rowSpan - 1);
                for (int i = 0; i < rowSpan; i++)
                    for (int j = 0; j < columnSpan; j++)
                        if (rowIndex + i < rowCapacity && columnIndex + j < Columns)
                            matrix[rowIndex + i, columnIndex + j] = true;

                var label = GetAttachedLabel(el);
                if (GetShowLabel(el))
                {
                    label.Visibility = el.Visibility;
                    Grid.SetColumn(label, columnIndex * 3);
                    Grid.SetRow(label, rowIndex * 2);
                    Grid.SetRowSpan(label, rowSpan * 2 - 1);
                    Grid.SetColumn(el, columnIndex * 3 + 1);
                    Grid.SetColumnSpan(el, columnSpan * 3 - 2);
                }
                else
                {
                    label.Visibility = Visibility.Collapsed;
                    Grid.SetColumn(el, columnIndex * 3);
                    Grid.SetColumnSpan(el, columnSpan * 3 - 1);
                }
                el.Width = ColumnMinWidth;
                Grid.SetRow(el, rowIndex * 2);
                Grid.SetRowSpan(el, rowSpan * 2 - 1);
            }

            GenerateRows(rowTotal);
            foreach (var el in children)
            {
                var height = GetRowHeight(el);
                if (!height.IsAbsolute || height.Value >= 0)
                    InnerGrid.RowDefinitions[Grid.GetRow(el)].Height = height;
            }
            if (InnerGrid.ColumnDefinitions.Count == 0)
                GenerateColumns();
            for (int i = 1; i < InnerGrid.ColumnDefinitions.Count; i += 3)
                InnerGrid.ColumnDefinitions[i].Width = new GridLength(ColumnMinWidth);
        }

        /// <summary>
        /// 测量时计算好控件的位置,控件宽度按最小宽度计
        /// </summary>
        protected override Size MeasureOverride(Size constraint)
        {
            UpdateGridLayout();
            return base.MeasureOverride(constraint);
        }

        /// <summary>
        /// 布局时计算控件准确宽度
        /// </summary>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var children = Children.Where(p => p.Visibility != System.Windows.Visibility.Collapsed);

            var maxLabelWidths = new double[Columns];
            foreach (var item in children)
            {
                int columIndex = Grid.GetColumn(item);
                if (columIndex > 0)
                {
                    var label = GetAttachedLabel(item);
                    maxLabelWidths[(columIndex - 1) / 3] = Math.Max(maxLabelWidths[(columIndex - 1) / 3], label.DesiredSize.Width);
                }
            }

            var labelWidths = maxLabelWidths.Sum();
            var totalWidth = arrangeBounds.Width - labelWidths - ColumnSpacing * (Columns - 1) - Padding.Left - Padding.Right;
            var width = Math.Max(1, Math.Floor(totalWidth / Columns) - 1);
            foreach (var item in children)
            {
                var columnSpan = Math.Max(1, Math.Min(Columns, GetColumnSpan(item)));
                int columIndex = 0;
                double extraWidth = 0.0;
                if (!GetShowLabel(item))
                {
                    columIndex = Grid.GetColumn(item) / 3;
                    extraWidth += maxLabelWidths[columIndex];
                }
                else
                    columIndex = (Grid.GetColumn(item) - 1) / 3;
                for (int i = 1; i < columnSpan; i++)
                {
                    extraWidth += maxLabelWidths[columIndex + i];
                    extraWidth += ColumnSpacing;
                }

                item.Width = (width * columnSpan) + extraWidth;
                item.MinWidth = (ColumnMinWidth * columnSpan) + extraWidth;
                item.MaxWidth = (ColumnMaxWidth * columnSpan) + extraWidth;
            }
            for (int i = 1; i < InnerGrid.ColumnDefinitions.Count; i += 3)
                InnerGrid.ColumnDefinitions[i].Width = new GridLength(width, GridUnitType.Pixel);

            return base.ArrangeOverride(arrangeBounds);
        }
    }
}
