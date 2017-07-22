using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Eortc.Buddy.MvvmBase
{
    /// <summary>
    /// ObservableCollection with AddRange, ReplaceRange, RemoveRange
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableCollectionEx<T> : ObservableCollection<T>
    {

        public ObservableCollectionEx() //: base()
        {
        }

        public ObservableCollectionEx(List<T> list) : base(list) { }

        public ObservableCollectionEx(IEnumerable<T> collection) : base (collection) { }


        //public void SetSource(IList<T> source)
        //{
        //    this.Items = source ;    // don't work : Items don't have setter, and backing field is private
        //}

        /// <summary> 
        /// Add Range then fire collection changed event
        /// </summary> 
        public void AddRange(IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException("items");
            CheckReentrancy();

            foreach (var item in items)
                Items.Add(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary> 
        /// Clear then add Range then fire collection changed event
        /// </summary> 
        public void ReplaceRange(IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException("items");
            CheckReentrancy();
            Items.Clear();
            foreach (var item in items)
                Items.Add(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }


        /// <summary> 
        /// Removes the first occurence of each item in the specified collection from ObservableCollection(Of T). 
        /// </summary> 
        public void RemoveRange(IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException("items");

            foreach (var i in items)
                Items.Remove(i);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }


    }
}
