using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using TraceTool;
using Binder = Microsoft.CSharp.RuntimeBinder.Binder;
// ReSharper disable ExplicitCallerInfoArgument
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable RedundantNameQualifier

namespace Eortc.Buddy.Mvvm.DynamicCollection
{
    // copy of DevExpress.XtraEditors.DXErrorProvider.ErrorType
    public enum CellErrorType {None,Default,Information,Warning,Critical,User1,User2,User3,User4,User5,User6,User7,User8,User9}
    public delegate object PropertyGetter ();                                                           // Func<object>
    public delegate bool   PropertySetter (object newValue);                                            // Func<object,bool>
    public delegate void   PropertyChanged(object oldValue, object newValue);                           // Action<object,object>
    public delegate void   PropertyError  (PropertyDescriptor propertyDescriptor, CellError cellError); // Action<PropertyDescriptor,CellError>

    public class CellError
    {
        public CellErrorType CellErrorType ;
        public string        CellErrorText ;
    }

    // property descriptor on the row
    public class PropertyDescriptor
    {
        // Value and Getter are exclusive !!!
        public object          Value ;      // value if no getter 
        public PropertyGetter  Getter ;     // Func<object>
        public PropertySetter  Setter ;     // Func<object,bool>
        public PropertyChanged OnChange ;   // Action<object,object>
        public PropertyError   CheckError ; // will be used by DynamicRow descendant (DynamicRowDx) to implement IDXDataErrorInfo

        public override string ToString()
        {
            if (Getter != null)
                return Getter.Invoke().ToString() ;
            return Value.ToString() ;
        }
    }

    
    // Don't implement System.ComponentModel.IDataErrorInfo because current "this" implementation return Object and not string
    // For IDXDataErrorInfo => see DynamicRowDx in Eortc.Buddy.Views.Extensions.DevExpressExtensions (use dynamic row callback)
    //namespace System.ComponentModel
    //{
    //    public interface IDataErrorInfo
    //    {
    //        string this[string columnName] { get; }
    //        string Error { get; }
    //    }
    //}

    // DynamicRow used by DynamicCollection
    public class DynamicRow : DynamicObject, INotifyPropertyChanged 
    {
        protected readonly Dictionary<string, PropertyDescriptor> DynamicProperties;
        private bool _propertyChangeCallbackAllowed = true ;
        
        // initialise row with no properties
        public DynamicRow()
        {
            DynamicProperties = new Dictionary<string, PropertyDescriptor>();
        }

        // initialise row using an array of KeyValuePair
        public DynamicRow(params KeyValuePair<string, object>[] propertyNames)
        {
            DynamicProperties = propertyNames.ToDictionary(s => s.Key, s => new PropertyDescriptor {Value = s.Value });
        }

        // initialise row using an IEnumerable of KeyValuePair
        public DynamicRow(IEnumerable<KeyValuePair<string, object>> propertyNames) : this(propertyNames.ToArray())
        {
        }

        // initialise row using a array of tuple key/Value
        public DynamicRow(params Tuple<string, object>[] propertyNamesAndValues)
        {
            DynamicProperties = propertyNamesAndValues.ToDictionary(x => x.Item1, x => new PropertyDescriptor {Value = x.Item2 });
        }

        // initialise row using an IEnumerable of tuple key/Value
        public DynamicRow(IEnumerable<Tuple<string, object>> propertyNames) : this(propertyNames.ToArray())
        {
        }
        public override string ToString()
        {
            return "DynamicRow" ;           
        }

        // Delphi style BeginUpdate. Change to properties will not fire OnPropertyChange 
        public void BeginUpdate()
        {
            _propertyChangeCallbackAllowed = false ;
        }

        // Delphi style EndUpdate. Subsequent properties changes will fire OnPropertyChange event
        public void EndUpdate()
        {
            _propertyChangeCallbackAllowed = true ;
        }

        // Indicate if the row is on update mode (OnPropertyChange not called)
        public bool IsOnUpdate()
        {
            return ! _propertyChangeCallbackAllowed;
        }


        public PropertyDescriptor DefineProperty(string propertyName)
        {
            return DefineProperty(propertyName, null as object);    // specifying as object force to use TryAddProperty with object propertyValue
        }

        // Add a property (if not exist yet) using a value and an optional property change callback. 
        // If property already exist, the current PropertyDescriptor is returned
        public PropertyDescriptor DefineProperty(
            string propertyName, 
            object propertyValue, 
            PropertyChanged onPropertyChanged=null)  // Action<object,string,object,object>
        {
            PropertyDescriptor propertyDescriptor;
            DynamicProperties.TryGetValue(propertyName, out propertyDescriptor);
            if (propertyDescriptor != null)
                return propertyDescriptor;   // row already have a propertyDescriptor, don't allow to replace it

            propertyDescriptor = new PropertyDescriptor {Value  = propertyValue, Getter = null, Setter = null, OnChange = onPropertyChanged} ;
            DynamicProperties.Add(propertyName, propertyDescriptor);  
            return propertyDescriptor;
        }

        // Add a property (if not exist yet) using a value and a CellDescriptor with getter,setter,onchange,checkError. 
        // Comparable to Javascript Object.defineProperty(obj, prop, descriptor) 
        public PropertyDescriptor DefineProperty(
            string propertyName,PropertyDescriptor propertyDescriptorParam) 
        {
            PropertyDescriptor propertyDescriptor;
            DynamicProperties.TryGetValue(propertyName, out propertyDescriptor);
            if (propertyDescriptor != null)
                return propertyDescriptor;   // row already have a propertyDescriptor, don't allow to replace it

            if (propertyDescriptorParam.Getter == null)
                propertyDescriptorParam.Getter = () => null;    // store null as field value

            propertyDescriptor = propertyDescriptorParam;
            DynamicProperties.Add(propertyName, propertyDescriptor);  
            return propertyDescriptor;
        }


        public PropertyDescriptor GetPropertyDescriptor(string propertyName) 
        {
            PropertyDescriptor propertyDescriptor;
            DynamicProperties.TryGetValue(propertyName, out propertyDescriptor);
            return propertyDescriptor;
        }

        // Accessor
        public object this[string propertyName]
        {
           get
           {
                //TTrace.Debug.Send("DynamicRow.this[ " + propertyName + "] getter") ;
                PropertyDescriptor propertyDescriptor;
                DynamicProperties.TryGetValue(propertyName, out propertyDescriptor);
                if (propertyDescriptor == null)
                    return null;
                if (propertyDescriptor.Getter != null)
                    return propertyDescriptor.Getter.Invoke() ;
                return propertyDescriptor.Value ;
           }

           set
           {
                // existing property , same type
                 PropertyDescriptor propertyDescriptor;
                DynamicProperties.TryGetValue(propertyName, out propertyDescriptor);
 
                // Try first to find the prop in dictionnary
                if (propertyDescriptor != null)// && _dynamicProperties[propertyName].GetType() == value.GetType())
                { 
                    // getter but no setter : read only prop
                    if (propertyDescriptor.Getter != null && propertyDescriptor.Setter == null) 
                        return ;

                    var currentProp = propertyDescriptor.Getter != null ? propertyDescriptor.Getter?.Invoke() : propertyDescriptor.Value;

                    if (currentProp == value)      // same as object.Equals(currentProp, value)
                        return ;

                    bool setterInvokeResult = true;
                    if (propertyDescriptor.Setter != null)
                        setterInvokeResult = propertyDescriptor.Setter.Invoke(value);
                    else 
                        propertyDescriptor.Value = value;

                    if (setterInvokeResult && _propertyChangeCallbackAllowed)
                    {
                        // call onPropertyFieldChanged for the field , if defined
                        propertyDescriptor.OnChange?.Invoke(currentProp, value) ;
  
                        // call OnPropertyChanged on the record 
                        OnPropertyChanged(propertyName);
                    }
                    return;
                }   // key exist 
  
                // slow for new property / Other type
                var binder = Binder.SetMember(
                    CSharpBinderFlags.None,
                    propertyName,
                    value.GetType(),
                    new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
  
                if (! base.TrySetMember((SetMemberBinder)binder, value))
                   throw new Exception("DynamicRow : Property " + propertyName + " error");
           }
        }

        // Override DynamicObject.TrySetMember()
        // Provides the implementation of setting a member.  
        // return true if the operation is complete, false if the call site should determine behavior.
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            //TTrace.Debug.Send("dynamicRow TrySetMember " + binder.Name) ;
            PropertyDescriptor propertyDescriptor;
            DynamicProperties.TryGetValue(binder.Name, out propertyDescriptor);
            if (propertyDescriptor != null)
            {
                // getter but no setter : read only prop
                if (propertyDescriptor.Getter != null && propertyDescriptor.Setter == null) 
                    return true; // no error

                object currentProp = propertyDescriptor.Getter != null ? propertyDescriptor.Getter.Invoke() : propertyDescriptor.Value;

                if (currentProp == value)      // same as object.Equals(currentProp, value)
                    return true;  // no error

                // Code differ from this[] "set" accessor : if the old value type is not the same, the new value is converted

                bool setterInvokeResult = true;
                if (propertyDescriptor.Setter != null)
                {
                    setterInvokeResult = propertyDescriptor.Setter.Invoke(value);
                } else if (currentProp == null || currentProp.GetType() == value.GetType()) {
                    // if same type, don't use converter
                    propertyDescriptor.Value = value;
                } else {
                    //TTrace.Debug.Send("dynamicRow setMember convert from " + currentProp + " to " + value) ;
                    try
                    {
                        TypeConverter converter = TypeDescriptor.GetConverter(currentProp.GetType());
                        var converted = converter.ConvertFrom(value);
                        propertyDescriptor.Value = converted;
                    }
                    catch (FormatException e)    
                    {
                        TTrace.Error.Send($"DynamicRow.TrySetMember {binder.Name} : FormatException {e.Message}") ;
                        return false;   // false = Error
                    }
                    catch (Exception e)      
                    {
                        TTrace.Error.Send($"DynamicRow.TrySetMember {binder.Name} : Exception {e.Message}") ;
                        return false;   // false = error
                    }
                }

                if (setterInvokeResult && _propertyChangeCallbackAllowed)
                {
                    // call onPropertyFieldChanged for the field , if defined
                    propertyDescriptor.OnChange?.Invoke(currentProp, value) ;
  
                    // call OnPropertyChanged on the record 
                    OnPropertyChanged(binder.Name);
                }
                return true;  // no error
            }  // key exist

            return base.TrySetMember(binder, value);
        }

        // Override DynamicObject.TryGetMember()
        // Provides the implementation of getting a member. 
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            //TTrace.Debug.Send("dynamicRow TryGetMember " + binder.Name) ;
            PropertyDescriptor propertyDescriptor;
            DynamicProperties.TryGetValue(binder.Name, out propertyDescriptor);
            if (propertyDescriptor != null)
            {
                result = propertyDescriptor.Getter != null ? propertyDescriptor.Getter.Invoke() : propertyDescriptor.Value;
                return true;
            }

            //TTrace.Debug.Send("dynamicRow TryGetMember not found for " + binder.Name) ;
            return base.TryGetMember(binder, out result);
        }

        // override DynamicObject.GetDynamicMemberNames : Returns the enumeration of all dynamic member names.
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return DynamicProperties.Keys.ToArray();
        }
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private /* protected virtual*/ void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
 
        public void RaisePropertyChange(string propertyName)
        {
            if (_propertyChangeCallbackAllowed)
            {
                //PropertyDescriptor propertyDescriptor;
                //DynamicProperties.TryGetValue(propertyName, out propertyDescriptor);
                //if (propertyDescriptor != null)
                //{
                //    // call onPropertyFieldChanged for the field , if defined
                //    propertyDescriptor.OnChange?.Invoke(propertyDescriptor.Value, propertyDescriptor.Value) ;
                //}

                // call OnPropertyChanged on the record, catched by devexpress to refresh data
                OnPropertyChanged(propertyName);
            }
        }
        
        //using System.Linq.Expressions;
        //using System.Reflection;

        //public bool TryGetProperty(string propertyName, out object propertyValue)
        //{
        //    PropertyDescriptor propertyDescriptor;
        //    DynamicProperties.TryGetValue(propertyName, out propertyDescriptor);
        //    if (propertyDescriptor != null)
        //    {
        //        propertyValue = propertyDescriptor.Getter != null ? propertyDescriptor.Getter.Invoke() : propertyDescriptor.Value;
        //        return true;
        //    }
        //    propertyValue = null;
        //    return false;
        //}

        //public IEnumerable<String> GetDynamicValues()
        //{
        //    return DynamicProperties.Values.Select(propDescriptor => propDescriptor.ToString()).ToArray();       // this will call PropertyDescriptor.ToString that check for Getter
        //}

       //private string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        //{
        //    if (propertyExpression == null)
        //        throw new ArgumentNullException("propertyExpression");
        //    MemberExpression body = propertyExpression.Body as MemberExpression;
        //    if (body == null)
        //        throw new ArgumentException(@"Invalid argument", "propertyExpression");
        //    PropertyInfo property = body.Member as PropertyInfo;
        //    if (property == null)
        //        throw new ArgumentException(@"Argument is not a property", "propertyExpression");
        //    return property.Name;
        //}

        //protected bool Set<T>(Expression<Func<T>> selectorExpression, ref T field, T newValue)
        //{
        //    if (EqualityComparer<T>.Default.Equals(field, newValue))
        //        return false;
        //    field = newValue;
        //    string propertyName = GetPropertyName(selectorExpression);
        //    // TO DO : call onPropertyFieldChanged for the field (this, propertyName , currentProp , value)
        //    // ...
        //    // call OnPropertyChanged on the record 
        //    OnPropertyChanged(propertyName);
        //    return true;
        //}

        //protected bool Set<T>(string propertyName, ref T field, T newValue)
        //{
        //    if (EqualityComparer<T>.Default.Equals(field, newValue))
        //        return false;
        //    field = newValue;
        //    // TO DO : call onPropertyFieldChanged for the field (this, propertyName , currentProp , value)
        //    // ...
        //    // call OnPropertyChanged on the record 
        //    OnPropertyChanged(propertyName);
        //    return true;
        //}
    }

    public class DynamicRow<TEntity> : DynamicRow where TEntity : class
    {
        public TEntity Entity {get;}

        public DynamicRow() //: base()
        {
        }

        public DynamicRow(TEntity entity) //: base()
        {
            Entity = entity ;
        }
        
        public DynamicRow(params KeyValuePair<string, object>[] propertyNames) : base(propertyNames) { }
        public DynamicRow(IEnumerable<KeyValuePair<string, object>> propertyNames) : base(propertyNames.ToArray()) { }
        public DynamicRow(params Tuple<string, object>[] propertyNamesAndValues) : base (propertyNamesAndValues) { }
        public DynamicRow(IEnumerable<Tuple<string, object>> propertyNames) : base(propertyNames.ToArray()) { }
        public DynamicRow(TEntity entity, params KeyValuePair<string, object>[] propertyNames) : base(propertyNames) {Entity = entity ;}
        public DynamicRow(TEntity entity, IEnumerable<KeyValuePair<string, object>> propertyNames) : base(propertyNames.ToArray())  {Entity = entity ;}
        public DynamicRow(TEntity entity, params Tuple<string, object>[] propertyNamesAndValues) : base (propertyNamesAndValues) {Entity = entity ;}
        public DynamicRow(TEntity entity, IEnumerable<Tuple<string, object>> propertyNames) : base(propertyNames.ToArray()) {Entity = entity ;}
        public override string ToString()
        {
            return "DynamicRow <" +  Entity?.GetType() + "> " + Entity ;           
        }
    }

    public class DynamicNode: DynamicRow<Object> 
    {
        public DynamicNode(object entity) : base(entity){}
        public DynamicRow<Object> Parent {get;set;}
        public DynamicCollection<Object> Children {get;set;}
        public override string ToString()
        {
            return "DynamicNode <" +  Entity?.GetType() + "> " + Entity ;           
        }

    }

}
