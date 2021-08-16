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

namespace NP.AvaloniaDock
{
    public class StackGroup : Control
    {
        [Content]
        public Controls PanelChildren { get; } = new Controls();

        #region SeparatorWidth Styled Avalonia Property
        public double SeparatorWidth
        {
            get { return GetValue(SeparatorWidthProperty); }
            set { SetValue(SeparatorWidthProperty, value); }
        }

        public static readonly StyledProperty<double> SeparatorWidthProperty =
            AvaloniaProperty.Register<StackGroup, double>
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
            AvaloniaProperty.Register<StackGroup, IBrush>
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
            AvaloniaProperty.Register<StackGroup, Orientation>
            (
                nameof(TheOrientation)
            );
        #endregion TheOrientation Styled Avalonia Property

        public int NumberChildren =>
            this.Children.Count;

        public void UpdateLayout()
        {
            var children =
                this.PanelChildren.ToList();

            this.PanelChildren.DeleteAllOneByOne();

            foreach(Control child in children)
            {
                AddLastPanelChild(child);
            }
        }

        public Control? FirstPanelChild =>
            this.Children.FirstOrDefault() as Control;


        public Control? LastPanelChild =>
            this.Children.LastOrDefault() as Control;

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

        private void InsertBeforeOrAfterPanelChild(Control? panelChild, Control panelChildToInsert, bool isAfter)
        {
            if (panelChild == null)
            {
                AddFirstChild(panelChildToInsert);
            }
            else
            {
                int currentChildIdx = this.Children.IndexOf(panelChild);

                int insertSeparatorIdx = NextIdx(currentChildIdx, isAfter);
                int insertChildIdx = NextIdx(insertSeparatorIdx, isAfter);

                Control separator = GetSeparator();

                AddDefinition(insertSeparatorIdx, true);
                AddDefinition(insertChildIdx, false);


                this.Children.Insert(insertSeparatorIdx, separator);
                this.Children.Insert(insertChildIdx, panelChildToInsert);

                ResetChildIndexes();
            }
        }

        private void ResetChildIndexes()
        {
            Action<Control, int> idxSetter =
                TheOrientation == Orientation.Horizontal ? Grid.SetColumn : Grid.SetRow;

            int i = 0;
            foreach(Control child in Children)
            {
                idxSetter(child, i);
                i++;
            }
        }

        private void InsertChildAtIdx(int idx, Control panelChildToInsert)
        {
            if (idx == 0)
            {
                AddChildAtIndexZero(panelChildToInsert);
            }
            else
            {
                InsertAfterPanelChild(PanelChildren[idx - 1] as Control, panelChildToInsert);
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

                this.ColumnDefinitions.Insert(idx, definition);
            }
            else
            {
                RowDefinition definition = new RowDefinition(isSeparator ? GridLength.Auto : GridLength.Parse("*"));

                if (!isSeparator)
                {
                    definition.MinHeight = 50;
                }


                this.RowDefinitions.Insert(idx, definition);
            }
        }

        private void InsertBeforePanelChild(Control? panelChild, Control panelChildToInsert)
        {
            InsertBeforeOrAfterPanelChild(panelChild, panelChildToInsert, false);
        }

        private void InsertAfterPanelChild(Control? panelChild, Control panelChildToInsert)
        {
            InsertBeforeOrAfterPanelChild(panelChild, panelChildToInsert, true);
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

        private void AddFirstChild(Control panelChildToInsert)
        {
            if (NumberChildren > 0)
            {
                throw new Exception("Programming errorin StackGroup.AddFirstChild - the panel children collection is not empty.");
            }

            AddDefinition(0, false);
            this.Children.Insert(0, panelChildToInsert);
        }

        private void RemovePanelChild(Control panelChild)
        {
            if (NumberChildren == 1)
            {
                this.Children.Remove(panelChild);
            }
            else
            {
                int idx = Children.IndexOf(panelChild);

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
            Children.RemoveAt(idx);

            IList definitions =
                TheOrientation == Orientation.Horizontal ? this.ColumnDefinitions : this.RowDefinitions;

            definitions.RemoveAt(idx);
        }

        private void AddChildAtIndexZero(Control panelChildToInsert)
        {
            InsertBeforePanelChild(FirstPanelChild, panelChildToInsert);
        }

        private void AddLastPanelChild(Control child)
        {
            this.InsertAfterPanelChild(LastPanelChild, child);
        }

        Grid _grid = new Grid();
        IDisposable _disposableBehavior;

        private Controls Children => _grid.Children;
        private ColumnDefinitions ColumnDefinitions => _grid.ColumnDefinitions;
        private RowDefinitions RowDefinitions => _grid.RowDefinitions;
        public StackGroup()
        {
            this.VisualChildren.Add(_grid);
            this.LogicalChildren.Add(_grid);
            _disposableBehavior = PanelChildren.AddDetailedBehavior(OnPanelChildAdded, OnPanelChildRemoved);
        }

        private void OnPanelChildAdded(IEnumerable<IControl> controls, IControl panelChild, int idx)
        {
            InsertChildAtIdx(idx, (Control) panelChild);
        }

        private void OnPanelChildRemoved(IEnumerable<IControl> controls, IControl panelChild, int idx)
        {
            RemovePanelChild((Control) panelChild);
        }
    }
}
