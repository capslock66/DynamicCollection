using System;

namespace Eortc.Buddy.Mvvm.DynamicCollection
{
    public class DynamicPropertyDescriptor : System.ComponentModel.PropertyDescriptor
    {
        private readonly string _displayName;
        private readonly Type _dynamicType;
        private readonly bool _isReadOnly;

        public DynamicPropertyDescriptor(string name, string displayName, Type type, bool isReadOnly = false, Attribute[] attributes = null)
            : base(name, attributes)
        {
            //TTrace.Debug.Send( "DynamicPropertyDescriptor " + name + " attr : " + attributes?.Length) ;
            _displayName = displayName;
            _dynamicType = type;
            _isReadOnly = isReadOnly;
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override object GetValue(object component)
        {
            return GetDynamicMember(component, Name);
        }

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value)
        {
            SetDynamicMember(component, Name, value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override Type ComponentType
        {
            get { return typeof (object); }
        }

        public override bool IsReadOnly
        {
            get { return _isReadOnly; }
        }

        public override Type PropertyType
        {
            get { return _dynamicType; }
        }

        public override string DisplayName
        {
            get { return _displayName; }
        }

        private static void SetDynamicMember(object obj, string memberName, object value)
        {
           if (obj == null)
              throw new Exception($"DynamicPropertyDescriptor '{memberName}': SetDynamicMember obj is null" );
           if (!(obj is DynamicRow))
              throw new Exception($"DynamicPropertyDescriptor '{memberName}': SetDynamicMember obj param must be a DynamicRow. Actual is {obj.GetType()}" );
            DynamicRow dynamicRow = (DynamicRow)obj;
            dynamicRow[memberName] = value;

        }

        private static object GetDynamicMember(object obj, string memberName)
        {
           if (obj == null)
              throw new Exception($"DynamicPropertyDescriptor '{memberName}': GetDynamicMember obj is null" );
           if (!(obj is DynamicRow))
              throw new Exception($"DynamicPropertyDescriptor '{memberName}': GetDynamicMember obj param must be a DynamicRow. Actual is {obj.GetType()}" );

            DynamicRow dynamicRow = (DynamicRow)obj;
            return dynamicRow[memberName];

            // slow ...
            //var binder = Binder.GetMember(
            //    CSharpBinderFlags.None,
            //    memberName,
            //    obj.GetType(),
            //    new[]
            //    {
            //        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
            //    });
            //var callsite = CallSite<Func<CallSite, object, object>>.Create(binder);
            //return callsite.Target(callsite, obj);
        }
    }
}
