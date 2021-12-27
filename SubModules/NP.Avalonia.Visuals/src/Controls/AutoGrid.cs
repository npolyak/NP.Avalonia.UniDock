using Avalonia;
using Avalonia.Controls;
using NP.Concepts.Behaviors;
using System;
using NP.Utilities;
using NP.Avalonia.Visuals.Behaviors;
using Avalonia.Collections;
using Avalonia.Metadata;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Avalonia.Data;
using System.Reactive.Linq;
using NP.Avalonia.Visuals.Converters;

namespace NP.Avalonia.Visuals.Controls
{
    public class AutoGrid : Control
    {
        private Grid _grid = new Grid();
        private KeyedDisposables<IControl> _keyedDisposables = new KeyedDisposables<IControl>();

        private IDisposable _behaviorSubscription;
        private IBinding _minRowBinding;
        private IBinding _minColumnBinding;


        #region RowsHeights shared Styled Avalonia Property
        public Dictionary<int, GridLength> RowsHeights
        {
            get { return GetValue(RowsHeightsProperty); }
            set { SetValue(RowsHeightsProperty, value); }
        }

        public static readonly StyledProperty<Dictionary<int, GridLength>> RowsHeightsProperty =
            AttachedProperties.RowsHeightsProperty.AddOwner<AutoGrid>();
        #endregion RowsHeights shared Styled Avalonia Property



        #region ColumnsWidths shared Styled Avalonia Property
        public Dictionary<int, GridLength> ColumnsWidths
        {
            get { return GetValue(ColumnsWidthsProperty); }
            set { SetValue(ColumnsWidthsProperty, value); }
        }

        public static readonly StyledProperty<Dictionary<int, GridLength>> ColumnsWidthsProperty =
            AttachedProperties.ColumnsWidthsProperty.AddOwner<AutoGrid>();
        #endregion ColumnsWidths shared Styled Avalonia Property


        [Content]
        public global::Avalonia.Controls.Controls Children => _grid.Children;


        #region MinRow Direct Avalonia Property
        private int _MinRow = default;

        public static readonly DirectProperty<AutoGrid, int> MinRowProperty =
            AvaloniaProperty.RegisterDirect<AutoGrid, int>
            (
                nameof(MinRow),
                o => o.MinRow,
                (o, v) => o.MinRow = v
            );

        public int MinRow
        {
            get => _MinRow;
            private set
            {
                SetAndRaise(MinRowProperty, ref _MinRow, value);
            }
        }

        #endregion MinRow Direct Avalonia Property

        private IDisposable _minRowSubscription;


        #region MinColumn Direct Avalonia Property
        private int _MinColumn = default;

        public static readonly DirectProperty<AutoGrid, int> MinColumnProperty =
            AvaloniaProperty.RegisterDirect<AutoGrid, int>
            (
                nameof(MinColumn),
                o => o.MinColumn,
                (o, v) => o.MinColumn = v
            );

        public int MinColumn
        {
            get => _MinColumn;
            private set
            {
                SetAndRaise(MinColumnProperty, ref _MinColumn, value);
            }
        }

        #endregion MinColumn Direct Avalonia Property

        private IDisposable _minColumnSubscription;

        public RowDefinitions RowDefinitions => _grid.RowDefinitions;

        public ColumnDefinitions ColumnDefinitions => _grid.ColumnDefinitions;

        public AutoGrid()
        {
            this.VisualChildren.Add(_grid);
            this.LogicalChildren.Add(_grid);
            _behaviorSubscription = this.Children.AddBehavior(OnChildAdded, OnChildRemoved);

            var minRowObservable = this.GetObservable(MinRowProperty);
            _minRowSubscription = minRowObservable.Subscribe(SetRowsHeights);
            _minRowBinding = minRowObservable.Select(r => -r).ToBinding();


            var minColObservable = this.GetObservable(MinColumnProperty);

            _minColumnSubscription = minColObservable.Subscribe(SetColumnsWidths);
            _minColumnBinding = minColObservable.Select(c => -c).ToBinding();

            this.RowsHeights = new Dictionary<int, GridLength>();
            this.ColumnsWidths = new Dictionary<int, GridLength>();

            this.Bind(ShowGridLinesProperty, new Binding("ShowGridLines") { Source = _grid, Mode = BindingMode.TwoWay });

            this.GetObservable(AttachedProperties.RowsHeightsProperty).Subscribe(OnRowsHeightsChanged);
            this.GetObservable(AttachedProperties.ColumnsWidthsProperty).Subscribe(OnColumnsWidthsChanged);
        }

        private void OnRowsHeightsChanged(Dictionary<int, GridLength> rowHeights)
        {
            SetRowsHeights(MinRow);
        }
        private void OnColumnsWidthsChanged(Dictionary<int, GridLength> obj)
        {
            SetColumnsWidths(MinColumn);
        }

        private void SetRowsHeights(int minRow, int startRange)
        {
            for(int i = startRange; i < RowDefinitions.Count; i++)
            {
                int row = minRow + i;
                RowDefinitions[i].Height = GetRowHeight(row);
            }
        }


        private GridLength GetRowHeight(int row)
        {
            GridLength height = GridLength.Auto;
            if (RowsHeights?.TryGetValue(row, out height) == true)
            {
                return height;
            }
            else
            {
                return GridLength.Auto;
            }
        }

        private void SetRowsHeights(int minRow)
        {
            SetRowsHeights(minRow, 0);
        }


        private void SetColumnsWidths(int minCol, int startRange)
        {
            for (int i = startRange; i < ColumnDefinitions.Count; i++)
            {
                int col = minCol + i;
                ColumnDefinitions[i].Width = GetColumnWidth(col);
            }
        }

        private void SetColumnsWidths(int minCol)
        {
            SetColumnsWidths(minCol, 0);
        }

        private GridLength GetColumnWidth(int col)
        {
            GridLength width = GridLength.Auto;
            if (ColumnsWidths?.TryGetValue(col, out width) == true)
            {
                return width;
            }
            else
            {
                return GridLength.Auto;
            }
        }

        private void OnChildRemoved(IControl child)
        {
            PropertiesChangeObserver.SetPropChangeObserver(child, null);
            _keyedDisposables.Remove(child);
        }

        private void OnChildAdded(IControl child)
        {
            PropertiesChangeObserver propertiesChangeObserver = new PropertiesChangeObserver();

            propertiesChangeObserver.Props =
                new AvaloniaProperty[]
                {
                    RowProperty,
                    Grid.RowSpanProperty,
                    ColumnProperty,
                    Grid.ColumnSpanProperty
                };

            PropertiesChangeObserver.SetPropChangeObserver(child, propertiesChangeObserver);

            if (propertiesChangeObserver.ResultObservable != null)
            {
                _keyedDisposables.Add(child, propertiesChangeObserver.ResultObservable.Subscribe(OnChildChanged));
            }

            MultiBinding rowNumberBinding = 
                new MultiBinding { Converter = IntSumConverter.Instance };

            rowNumberBinding.Bindings.Add(_minRowBinding);
            rowNumberBinding.Bindings.Add(child.GetObservable(RowProperty).ToBinding());
            child.Bind(Grid.RowProperty, rowNumberBinding);

            MultiBinding columnNumberBinding =
                new MultiBinding { Converter = IntSumConverter.Instance };
            columnNumberBinding.Bindings.Add(_minColumnBinding);
            columnNumberBinding.Bindings.Add(child.GetObservable(ColumnProperty).ToBinding());
            child.Bind(Grid.ColumnProperty, columnNumberBinding);
        }

        private void OnChildChanged(IAvaloniaObject child)
        {
            Control childControl = (Control)child;

            int currentMinRow = Children.Cast<Control>().Min(c => GetRow(c));

            int currentMaxRow = Children.Cast<Control>().Max(c => GetRow(c) + Grid.GetRowSpan(c));

            int totalRows = currentMaxRow - currentMinRow;
            int extraRows = totalRows - _grid.RowDefinitions.Count;

            AddRows(extraRows);

            MinRow = currentMinRow;

            int currentMinColumn = Children.Cast<Control>().Min(c => GetColumn(c));
            int currentMaxColumn = Children.Cast<Control>().Max(c => GetColumn(c) + Grid.GetColumnSpan(c));

            int totalColumns = currentMaxColumn - currentMinColumn;
            int extraColumns = totalColumns - _grid.ColumnDefinitions.Count;

            AddColumns(extraColumns);
            MinColumn = currentMinColumn;
        }

        private void AddGridDefs<T>(int numberExtraDefs, IList<T> defs, int minItem, Func<int, T> defCreator)
            where T : DefinitionBase
        {
            int absNumberExtraItems = Math.Abs(numberExtraDefs);

            if (absNumberExtraItems == 0)
                return;

            int currentNumberItems = defs.Count;
            for (int i = 0; i < absNumberExtraItems; i++)
            {
                if (numberExtraDefs > 0)
                {
                    int itemNumber = minItem + currentNumberItems + i;
                    defs.Add(defCreator(itemNumber));
                }
                else
                {
                    defs.RemoveAt(0);
                }
            }
        }

        private void AddRows(int numberExtraRows) =>
            AddGridDefs(numberExtraRows, _grid.RowDefinitions, MinRow, (row) => new RowDefinition(GetRowHeight(row)));

        private void AddColumns(int numberExtraCols) =>
            AddGridDefs(numberExtraCols, _grid.ColumnDefinitions, MinColumn, (col) => new ColumnDefinition(GetColumnWidth(col)));

        #region Row Attached Avalonia Property
        public static int GetRow(Control obj)
        {
            return obj.GetValue(RowProperty);
        }

        public static void SetRow(Control obj, int value)
        {
            obj.SetValue(RowProperty, value);
        }

        public static readonly AttachedProperty<int> RowProperty =
            AvaloniaProperty.RegisterAttached<object, Control, int>
            (
                "Row"
            );
        #endregion Row Attached Avalonia Property


        #region Column Attached Avalonia Property
        public static int GetColumn(Control obj)
        {
            return obj.GetValue(ColumnProperty);
        }

        public static void SetColumn(Control obj, int value)
        {
            obj.SetValue(ColumnProperty, value);
        }

        public static readonly AttachedProperty<int> ColumnProperty =
            AvaloniaProperty.RegisterAttached<object, Control, int>
            (
                "Column"
            );
        #endregion Column Attached Avalonia Property


        #region ShowGridLines Styled Avalonia Property
        public bool ShowGridLines
        {
            get { return GetValue(ShowGridLinesProperty); }
            set { SetValue(ShowGridLinesProperty, value); }
        }

        public static readonly StyledProperty<bool> ShowGridLinesProperty =
            Grid.ShowGridLinesProperty.AddOwner<AutoGrid>();
        #endregion ShowGridLines Styled Avalonia Property

    }
}
