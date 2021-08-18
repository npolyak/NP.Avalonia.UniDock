using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using System.Linq;
using NP.Utilities;
using System.Collections.Generic;
using System;
using Avalonia.Media;
using Avalonia.Metadata;
using NP.Concepts.Behaviors;
using System.Collections;
using System.Collections.ObjectModel;

namespace NP.AvaloniaDock
{
    public class StackGroup<T> : Control
        where T : IControl
    {
        private ObservableCollection<T> _items = new ObservableCollection<T>();
        /// <summary>
        /// Defines the <see cref="Items"/> property.
        /// </summary>
        public static readonly DirectProperty<StackGroup<T>, ObservableCollection<T>> ItemsProperty =
            AvaloniaProperty.RegisterDirect<StackGroup<T>, ObservableCollection<T>>
            (
                nameof(Items),
                o => o.Items,
                (o, v) => o.Items = v);


        /// <summary>
        /// Gets or sets the items to display.
        /// </summary>
        [Content]
        public ObservableCollection<T> Items
        {
            get
            {
                return _items;
            }

            set
            {
                DisposeBehavior();

                SetAndRaise(ItemsProperty, ref _items, value);

                SetBehavior();
            }
        }

        protected virtual void BeforeItemsSet()
        {

        }

        protected virtual void AfterItemsSet()
        {

        }

        public int Count => Items.Count;

        #region SeparatorWidth Styled Avalonia Property
        public double SeparatorWidth
        {
            get { return GetValue(SeparatorWidthProperty); }
            set { SetValue(SeparatorWidthProperty, value); }
        }

        public static readonly StyledProperty<double> SeparatorWidthProperty =
            AvaloniaProperty.Register<StackGroup<T>, double>
            (
                nameof(SeparatorWidth),
                2d
            );
        #endregion SeparatorWidth Styled Avalonia Property


        #region SeparatorBackground Styled Avalonia Property
        public IBrush SeparatorBackground
        {
            get { return GetValue(SeparatorBackgroundProperty); }
            set { SetValue(SeparatorBackgroundProperty, value); }
        }

        public static readonly StyledProperty<IBrush> SeparatorBackgroundProperty =
            AvaloniaProperty.Register<StackGroup<T>, IBrush>
            (
                nameof(SeparatorBackground),
                new SolidColorBrush(Colors.Black)
            );
        #endregion SeparatorBackground Styled Avalonia Property


        #region TheOrientation Styled Avalonia Property
        public Orientation TheOrientation
        {
            get { return GetValue(TheOrientationProperty); }
            set { SetValue(TheOrientationProperty, value); }
        }

        public static readonly StyledProperty<Orientation> TheOrientationProperty =
            AvaloniaProperty.Register<StackGroup<T>, Orientation>
            (
                nameof(TheOrientation)
            );
        #endregion TheOrientation Styled Avalonia Property

        public int NumberChildren =>
            this.GridChildren.Count;

        public void UpdateLayout()
        {
            var children =
                this.Items.ToList();

            this.Items.RemoveAllOneByOne();

            foreach(T child in children)
            {
                AddLastItem(child);
            }
        }

        public T? FirstItem =>
            (T?) this.GridChildren.FirstOrDefault();


        public T? LastItem =>
            (T?) this.GridChildren.LastOrDefault();

        private static int NextIdx(int idx, bool isAfter)
        {
            if (isAfter)
            {
                return idx + 1;
            }
            else
            {
                return idx;
            }
        }

        private void InsertBeforeOrAfterItem(T? currentItem, T itemToInsert, bool isAfter)
        {
            if (currentItem == null)
            {
                AddFirstChild(itemToInsert);
            }
            else
            {
                int currentChildIdx = this.GridChildren.IndexOf(currentItem);

                int insertSeparatorIdx = NextIdx(currentChildIdx, isAfter);
                int insertChildIdx = NextIdx(insertSeparatorIdx, isAfter);

                Control separator = GetSeparator();

                AddDefinition(insertSeparatorIdx, true);
                AddDefinition(insertChildIdx, false);


                this.GridChildren.Insert(insertSeparatorIdx, separator);
                this.GridChildren.Insert(insertChildIdx, itemToInsert);

                ResetChildIndexes();
            }
        }

        private void ResetChildIndexes()
        {
            Action<Control, int> idxSetter =
                TheOrientation == Orientation.Horizontal ? Grid.SetColumn : Grid.SetRow;

            int i = 0;
            foreach(Control child in GridChildren)
            {
                idxSetter(child, i);
                i++;
            }
        }

        private void InsertChildAtIdx(int idx, T itemToInsert)
        {
            if (idx == 0)
            {
                AddChildAtIndexZero(itemToInsert);
            }
            else
            {
                InsertAfterItem(Items[idx - 1], itemToInsert);
            }
        }

        private void AddDefinition(int idx, bool isSeparator)
        {
            if (TheOrientation == Orientation.Horizontal)
            {
                ColumnDefinition definition = new ColumnDefinition(isSeparator ? GridLength.Auto : GridLength.Parse("*"));

                if (!isSeparator)
                {
                    definition.MinWidth = 50;
                }

                this.GridColumnDefinitions.Insert(idx, definition);
            }
            else
            {
                RowDefinition definition = new RowDefinition(isSeparator ? GridLength.Auto : GridLength.Parse("*"));

                if (!isSeparator)
                {
                    definition.MinHeight = 50;
                }


                this.GridRowDefinitions.Insert(idx, definition);
            }
        }

        private void InsertBeforeItem(T? currentItem, T itemToInsert)
        {
            InsertBeforeOrAfterItem(currentItem, itemToInsert, false);
        }

        private void InsertAfterItem(T? currentItem, T itemToInsert)
        {
            InsertBeforeOrAfterItem(currentItem, itemToInsert, true);
        }

        private Control GetSeparator()
        {
            GridSplitter gridSplitter = new GridSplitter();

            gridSplitter.Background = SeparatorBackground;

            if (TheOrientation == Orientation.Horizontal)
            {
                gridSplitter.Width = SeparatorWidth;
                gridSplitter.VerticalAlignment = VerticalAlignment.Stretch;
            }
            else //TheOrientation == Orientation.Vertical
            {
                gridSplitter.Height = SeparatorWidth;
                gridSplitter.HorizontalAlignment = HorizontalAlignment.Stretch;
            }

            return gridSplitter;
        }

        private void AddFirstChild(T itemToInsert)
        {
            if (NumberChildren > 0)
            {
                throw new Exception("Programming errorin StackGroup.AddFirstChild - the item collection is not empty.");
            }

            AddDefinition(0, false);
            this.GridChildren.Insert(0, itemToInsert);
        }

        private void RemoveItem(T item)
        {
            if (NumberChildren == 1)
            {
                this.GridChildren.Remove(item);
            }
            else
            {
                int idx = GridChildren.IndexOf(item);

                RemoveChildAt(idx);

                if (idx == 0)
                {
                    RemoveChildAt(0);
                }
                else
                {
                    RemoveChildAt(idx - 1);
                }

                ResetChildIndexes();
            }
        }

        private void RemoveChildAt(int idx)
        {
            GridChildren.RemoveAt(idx);

            IList definitions =
                TheOrientation == Orientation.Horizontal ? this.GridColumnDefinitions : this.GridRowDefinitions;

            definitions.RemoveAt(idx);
        }

        private void AddChildAtIndexZero(T itemToInsert)
        {
            InsertBeforeItem(FirstItem, itemToInsert);
        }

        private void AddLastItem(T item)
        {
            this.InsertAfterItem(LastItem, item);
        }

        Grid _grid = new Grid();
        IDisposable? _disposableBehavior;

        private Controls GridChildren => _grid.Children;
        private ColumnDefinitions GridColumnDefinitions => _grid.ColumnDefinitions;
        private RowDefinitions GridRowDefinitions => _grid.RowDefinitions;

        public StackGroup()
        {
            this.VisualChildren.Add(_grid);
            this.LogicalChildren.Add(_grid);
            SetBehavior();
            AfterItemsSet();
        }

        private void DisposeBehavior()
        {
            _disposableBehavior?.Dispose();
            _disposableBehavior = null;
        }

        private void SetBehavior()
        {
            _disposableBehavior = Items?.AddDetailedBehavior(OnItemAdded, OnItemRemoved)!;
        }

        private void OnItemAdded(IEnumerable<T> controls, T item, int idx)
        {
            InsertChildAtIdx(idx, item);
        }

        private void OnItemRemoved(IEnumerable<T> controls, T item, int idx)
        {
            RemoveItem(item);
        }
    }
}
