using System;

namespace WellboreProfileView.Interfaces
{
    public interface ISmartObservableCollection
    {
        bool IsDirty { get; }

        bool IsAddedItems { get; }

        bool IsRemoveItems { get; }

        bool IsChangedItems { get; }

        void AcceptChanges();

        event Action AnyCollectionChanged;
    }
}
