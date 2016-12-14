using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

//using TraceTool;

// Inspired by Joel Heymbeeck work at https://github.com/SinaC/WPF-Helpers

namespace Eortc.Buddy.Mvvm.DynamicCollection
{

    public interface IDynamicCollection
    {
        IList Bands {get ;}
        IList Columns {get ;}

        // IList Rows ??? => this
    }

    public class DynamicCollection<TEntity> : ObservableCollection<DynamicRow<TEntity>>, ITypedList, IDynamicCollection where TEntity : class
    {
        #region IDynamicCollection
        public IList Bands { get; set; } // ObservableCollection<DynamicBand> 
        public IList Columns { get; set; }  // ObservableCollection<DynamicColumn> 
        #endregion

        public DynamicCollection()
        {
            Bands = new ObservableCollection<DynamicBand>();
            
            // Due to a devexpress/generator _bug_  with dynamic bands , we must have a first invisible band (with no columns)
            DynamicBand band0 = new DynamicBand() { BandHeader = "BUG" , Visible=false, Width = 5 , OverlayHeaderByChildren = true };
            Bands.Add(band0);

            Columns = new ObservableCollection<DynamicColumn>();
        }

        public DynamicCollection(IEnumerable<DynamicRow<TEntity>> rows, IEnumerable<DynamicColumn> columns)
        {
            Columns = new ObservableCollection<DynamicColumn>();
            if (rows != null)
                foreach (DynamicRow<TEntity> row in rows)
                    Add(row);
            if (columns != null)
                foreach (DynamicColumn col in columns)
                    Columns.Add(col);
        }

        #region ITypedList
        // Provides functionality to discover the schema for a bindable list, where the properties available for binding differ from the public properties of the object to bind to.
        // https://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k(System.ComponentModel.ITypedList);k(TargetFrameworkMoniker-.NETFramework,Version%3Dv4.5.2);k(DevLang-csharp)&rd=true

        // Returns the name of the list.
        // This method is only used in the design-time framework and by the obsolete DataGrid control.
        public string GetListName(System.ComponentModel.PropertyDescriptor[] listAccessors)
        {
            return null;
        }

        // Returns the PropertyDescriptorCollection that represents the properties on each item used to bind data.
        public PropertyDescriptorCollection GetItemProperties(System.ComponentModel.PropertyDescriptor[] listAccessors)
        {
            System.ComponentModel.PropertyDescriptor[] dynamicDescriptors;

            //TTrace.Debug.Send( "GetItemProperties ") ;
            //foreach (var column in Columns)
            //    TTrace.Debug.Send( "new DynamicPropertyDescriptor " + column.Name  + "," + column.DisplayName + "," + column.Type + "," + column.IsReadOnly) ;
            //TTrace.Flush();

            var columns = (ObservableCollection<DynamicColumn>) Columns ;
            if (columns.Any())
                dynamicDescriptors = columns.Select(column => new DynamicPropertyDescriptor(column.Name, column.DisplayName, column.Type, column.IsReadOnly, column.Attributes)).Cast<System.ComponentModel.PropertyDescriptor>().ToArray();
            else
                dynamicDescriptors = new System.ComponentModel.PropertyDescriptor[0];

            return new PropertyDescriptorCollection(dynamicDescriptors);
        }

        #endregion
        

    }

}
