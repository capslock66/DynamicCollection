using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

// using System.Runtime.CompilerServices;  // for [CallerMemberName]

namespace DynamicCollection
{
    public class ObservableObject : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged ;
        public virtual void OnPropertyChanged(string propertyName = null)  // [CallerMemberName] string propertyName
        {
            // resharper will return a warning if the propertyName is giving. ([CallerMemberName] is in framework 4.5):
            // "explicit argument passed to parameter with caller info attribute"
            // this warning can be ignored

            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) 
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException("propertyExpression");

            var body = propertyExpression.Body as MemberExpression;

            if (body == null)
                throw new ArgumentException(@"Invalid argument", "propertyExpression");

            var property = body.Member as PropertyInfo;

            if (property == null)
                throw new ArgumentException(@"Argument is not a property", "propertyExpression");

            return property.Name;
        }

        // return true if old and new values are different
        protected bool Set<T>(Expression<Func<T>> selectorExpression, ref T field, T newValue)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
                return false;
            field = newValue;
            string propertyName = GetPropertyName(selectorExpression);
            OnPropertyChanged(propertyName);
            return true;
        }

        protected bool Set<T>(string propertyName, ref T field, T newValue)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
                return false;
            field = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
