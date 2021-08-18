using NP.Concepts.Behaviors;
using NP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NP.AvaloniaDock
{
    public class SetItemsBehavior<TGetterObj, TItem, TProp> : IDisposable
        where TGetterObj : class
    {
        #region GetterObj Property
        private TGetterObj? _getterObj;
        public TGetterObj? GetterObj
        {
            get
            {
                return this._getterObj;
            }
            set
            {
                if (this._getterObj.ObjEquals(value))
                {
                    return;
                }

                if (_getterObj is INotifyPropertyChanged releaseNotifiable)
                {
                    releaseNotifiable.PropertyChanged -= Notifiable_PropertyChanged;
                }

                _behavior?.Suspend();
                this._getterObj = value;

                if (_getterObj is INotifyPropertyChanged subscribeNotifiable)
                {
                    subscribeNotifiable.PropertyChanged += Notifiable_PropertyChanged;
                }

                if (_getterObj != null)
                {
                    _behavior?.Reset();
                }
            }
        }

        private void Notifiable_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            _behavior?.Reset();
        }
        #endregion GetterObj Property


        #region ChildItems Property
        private IEnumerable<TItem>? _childItems;
        public IEnumerable<TItem>? ChildItems
        {
            get
            {
                return this._childItems;
            }
            set
            {
                if (this._childItems.ObjEquals(value))
                {
                    return;
                }

                _behavior?.Dispose();

                this._childItems = value;

                _behavior = _childItems?.AddBehavior(OnAdded, OnRemoved);
            }
        }

        private void OnRemoved(TItem obj)
        {
            _itemSetter.Invoke(obj, default(TProp)!);
        }

        private void OnAdded(TItem obj)
        {
            _itemSetter.Invoke(obj, _valGetter.Invoke(GetterObj!));
        }

        public void Dispose()
        {
            GetterObj = null;
            _behavior?.Dispose();
            _behavior = null;
        }
        #endregion ChildItems Property


        Func<TGetterObj, TProp> _valGetter;
        Action<TItem, TProp> _itemSetter;
        ISuspendableDisposable? _behavior;

        public SetItemsBehavior(Func<TGetterObj, TProp> valGetter, Action<TItem, TProp> itemSetter)
        {
            _valGetter = valGetter;
            _itemSetter = itemSetter;
        }
    }
}
