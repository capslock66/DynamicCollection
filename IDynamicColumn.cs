using System;

namespace Eortc.Buddy.Mvvm.DynamicCollection
{
    public interface IDynamicColumn
    {
        string Name { get; }
        string DisplayName { get; }
        Type Type { get; }
        bool IsReadOnly { get; }
        Attribute[] Attributes { get; set; }
    }
}
