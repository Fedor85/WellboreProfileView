using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Prism.Mvvm;
using WellboreProfileView.Interfaces;

namespace WellboreProfileView.ViewModels
{
    public class SmartObservableCollection<T> : ObservableCollection<T>, ISmartObservableCollection, IRemoveRangeCollection where T : BindableBase
    {
        private List<PropertyInfo> smartObservableCollectionPropertyInfos;

        private List<T> changedItems { get; }

        public List<T> ChangedItems
        {
            get
            {
                List<T> currentChangedItems = new List<T>(changedItems);
                foreach (T changedDetailItems in GetItemWhereAddAndChangedDetail())
                {
                    if (!currentChangedItems.Contains(changedDetailItems))
                        currentChangedItems.Add(changedDetailItems);
                }
                return currentChangedItems;
            }
        }

        public List<T> AddItems { get; }

        public List<T> RemoveItems { get; }

        public bool IsAddedItems { get {return  AddItems.Count > 0;} }

        public bool IsRemoveItems { get { return RemoveItems.Count > 0; } }

        public bool IsChangedItems { get { return changedItems.Count > 0; } }

        /// <summary>
        /// Все измения в коллекции в тoмчесле и изменения контралируемых свойсв.
        /// Контроль T объектов если они содержать проперти SmartObservableCollection
        /// </summary>
        public event Action AnyCollectionChanged;
        
        public bool IsDirty
        {
            get
            {
                return AddItems.Count > 0 || RemoveItems.Count > 0 || changedItems.Count > 0 || IsDirtyDetail();
            }
        }

        public SmartObservableCollection()
        {
            smartObservableCollectionPropertyInfos = GetISmartObservableCollectionPropertyInfos();
            AddItems = new List<T>();
            RemoveItems = new List<T>();
            changedItems = new List<T>();
            CollectionChanged += ThisCollectionChanged;
        }

        /// <summary>
        /// Добавить сущесвующий элемент когда необходим контроль только измения контролируемых свойств
        /// </summary>
        /// <param name="item"></param>
        public void AddExisting(T item)
        {
            CollectionChanged -= ThisCollectionChanged;
            Add(item);
            item.PropertyChanged += ItemPropertyChanged;
            CollectionChanged += ThisCollectionChanged;
            AttachAnyCollectionChangedToDetailsItem(item);
        }

        public void AddRange(IEnumerable<T> items)
        {
            CollectionChanged -= ThisCollectionChanged;
            foreach (T item in items)
            {
                Add(item);
                AddItems.Add(item);
                AttachAnyCollectionChangedToDetailsItem(item);
                item.PropertyChanged += ItemPropertyChanged;
            }
            CollectionChanged += ThisCollectionChanged;
            OnAnyCollectionChanged();
        }


        public void RemoveRange(List<object> items)
        {
            CollectionChanged -= ThisCollectionChanged;

            foreach (T item in items)
            {
                Remove(item);
                item.PropertyChanged -= ItemPropertyChanged;
                DetachAnyCollectionChangedToDetailsItem(item);
                RemoveItems.Add(item);
            }
            CollectionChanged += ThisCollectionChanged;
            OnAnyCollectionChanged();
        }

        public void AcceptChanges()
        {
            AddItems.Clear();
            RemoveItems.Clear();
            changedItems.Clear();
            AcceptChangesDetail();
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            T item = (T)sender;
            if (!changedItems.Contains(item))
                changedItems.Add(item);

            OnAnyCollectionChanged();
        }

        public new void Clear()
        {
            RemoveItems.AddRange(Items);
            base.Clear();
        }

        private void ThisCollectionChanged(object sender,  NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                foreach (T newItem in e.NewItems)
                {
                    newItem.PropertyChanged += ItemPropertyChanged;
                    AttachAnyCollectionChangedToDetailsItem(newItem);
                    AddItems.Add(newItem);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
            {
                foreach (T oldItem in e.OldItems)
                {
                    oldItem.PropertyChanged -= ItemPropertyChanged;
                    DetachAnyCollectionChangedToDetailsItem(oldItem);
                    RemoveItems.Add(oldItem);
                }
            }
            OnAnyCollectionChanged();
        }

        private bool IsDirtyDetail()
        {
            foreach (PropertyInfo propertyInfo in smartObservableCollectionPropertyInfos)
            {
                foreach (T item in Items)
                {
                    if (((ISmartObservableCollection)propertyInfo.GetValue(item)).IsDirty)
                        return true;
                }
            }
            return false;
        }

        private List<T> GetItemWhereAddAndChangedDetail()
        {
            List<T> items = new List<T>();
            foreach (PropertyInfo propertyInfo in smartObservableCollectionPropertyInfos)
            {
                foreach (T item in Items)
                {
                    ISmartObservableCollection detailColection = (ISmartObservableCollection)propertyInfo.GetValue(item);
                    if (detailColection.IsAddedItems || detailColection.IsChangedItems)
                        items.Add(item);
                }
            }
            return items;
        }

        private void AcceptChangesDetail()
        {
            foreach (PropertyInfo propertyInfo in smartObservableCollectionPropertyInfos)
            {
                foreach (T item in Items)
                    ((ISmartObservableCollection)propertyInfo.GetValue(item)).AcceptChanges();
            }
        }

        private void AttachAnyCollectionChangedToDetailsItem(T item)
        {
            foreach (PropertyInfo propertyInfo in smartObservableCollectionPropertyInfos)
                ((ISmartObservableCollection)propertyInfo.GetValue(item)).AnyCollectionChanged += OnAnyCollectionChanged;
        }

        private void DetachAnyCollectionChangedToDetailsItem(T item)
        {
            foreach (PropertyInfo propertyInfo in smartObservableCollectionPropertyInfos)
                ((ISmartObservableCollection)propertyInfo.GetValue(item)).AnyCollectionChanged -= OnAnyCollectionChanged;
        }

        private List<PropertyInfo> GetISmartObservableCollectionPropertyInfos()
        {
            List<PropertyInfo> propertyInfos = new List<PropertyInfo>();
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                if (propertyInfo.PropertyType.GetInterfaces().ToList().Contains(typeof(ISmartObservableCollection)))
                    propertyInfos.Add(propertyInfo);
            }
            return propertyInfos;
        }

        private void OnAnyCollectionChanged()
        {
            AnyCollectionChanged?.Invoke();
        }
    }
}