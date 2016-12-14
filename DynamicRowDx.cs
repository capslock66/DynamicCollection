using System;
using DevExpress.XtraEditors.DXErrorProvider;
using Eortc.Buddy.Mvvm.DynamicCollection;

namespace Eortc.Buddy.Views.Extensions.DevExpressExtensions
{
    // implement devexpress IDXDataErrorInfo validation interface.
    // This interface is not placed in the DynamicRow, since the MVVM project is NOT dependant of devexpress.

    /*
     namespace DevExpress.XtraEditors.DXErrorProvider
     IDXDataErrorInfo : GetPropertyError(string propertyName, ErrorInfo info) , GetError(ErrorInfo info)
     ErrorInfo : string ErrorText, ErrorType ErrorType
     ErrorType : None, Default, Information, Warning, Critical, User1...User9   
    */

    public class DynamicRowDx : DynamicRow, IDXDataErrorInfo
    {

        public Action<string,ErrorInfo> OnPropertyError { get; set; }
        public Action<ErrorInfo> OnRowError { get; set; }

        public void /*IDXDataErrorInfo*/ GetPropertyError(string propertyName, ErrorInfo info)
        {
            PropertyDescriptor propertyDescriptor;
            DynamicProperties.TryGetValue(propertyName, out propertyDescriptor);
            if (propertyDescriptor?.CheckError != null)
            {
                CellError cellError = new CellError();
                propertyDescriptor.CheckError.Invoke (propertyDescriptor,cellError) ;

                info.ErrorText = cellError.CellErrorText;

                if (cellError.CellErrorType == CellErrorType.None       ) info.ErrorType = ErrorType.None;
                if (cellError.CellErrorType == CellErrorType.Default    ) info.ErrorType = ErrorType.Default;
                if (cellError.CellErrorType == CellErrorType.Information) info.ErrorType = ErrorType.Information;
                if (cellError.CellErrorType == CellErrorType.Warning    ) info.ErrorType = ErrorType.Warning;
                if (cellError.CellErrorType == CellErrorType.Critical   ) info.ErrorType = ErrorType.Critical;
                if (cellError.CellErrorType == CellErrorType.User1      ) info.ErrorType = ErrorType.User1;
                if (cellError.CellErrorType == CellErrorType.User2      ) info.ErrorType = ErrorType.User2;
                if (cellError.CellErrorType == CellErrorType.User3      ) info.ErrorType = ErrorType.User3;
                if (cellError.CellErrorType == CellErrorType.User4      ) info.ErrorType = ErrorType.User4;
                if (cellError.CellErrorType == CellErrorType.User5      ) info.ErrorType = ErrorType.User5;
                if (cellError.CellErrorType == CellErrorType.User6      ) info.ErrorType = ErrorType.User6;
                if (cellError.CellErrorType == CellErrorType.User7      ) info.ErrorType = ErrorType.User7;
                if (cellError.CellErrorType == CellErrorType.User8      ) info.ErrorType = ErrorType.User8;
                if (cellError.CellErrorType == CellErrorType.User9      ) info.ErrorType = ErrorType.User9;
            } 

            // row Error can override info content
            OnPropertyError?.Invoke (propertyName, info) ;
        }

        public void /*IDXDataErrorInfo*/ GetError(ErrorInfo info)
        {
            OnRowError?.Invoke (info) ;
        }
    }

    public class DynamicRowDx<TEntity> : DynamicRow<TEntity>, IDXDataErrorInfo where TEntity : class
    {
        public DynamicRowDx() 
        {
            // Constructor needed for DynamicNodeDx 
        }

        public DynamicRowDx(TEntity entity) : base(entity) {}

        // todo : add other base constructor if needed

        public override string ToString()
        {
            return "DynamicRowDx <" +  Entity?.GetType() + "> " + Entity ;           
        }


        // IDXDataErrorInfo : GetPropertyError , GetError
        public Action<string,ErrorInfo> OnPropertyError { get; set; }
        public Action<ErrorInfo> OnRowError { get; set; }

        public void /*IDXDataErrorInfo*/ GetPropertyError(string propertyName, ErrorInfo info)
        {
            PropertyDescriptor propertyDescriptor;
            DynamicProperties.TryGetValue(propertyName, out propertyDescriptor);
            if (propertyDescriptor?.CheckError != null)
            {
                CellError cellError = new CellError();
                propertyDescriptor.CheckError.Invoke (propertyDescriptor, cellError) ;

                info.ErrorText = cellError.CellErrorText;

                if (cellError.CellErrorType == CellErrorType.None       ) info.ErrorType = ErrorType.None;
                if (cellError.CellErrorType == CellErrorType.Default    ) info.ErrorType = ErrorType.Default;
                if (cellError.CellErrorType == CellErrorType.Information) info.ErrorType = ErrorType.Information;
                if (cellError.CellErrorType == CellErrorType.Warning    ) info.ErrorType = ErrorType.Warning;
                if (cellError.CellErrorType == CellErrorType.Critical   ) info.ErrorType = ErrorType.Critical;
                if (cellError.CellErrorType == CellErrorType.User1      ) info.ErrorType = ErrorType.User1;
                if (cellError.CellErrorType == CellErrorType.User2      ) info.ErrorType = ErrorType.User2;
                if (cellError.CellErrorType == CellErrorType.User3      ) info.ErrorType = ErrorType.User3;
                if (cellError.CellErrorType == CellErrorType.User4      ) info.ErrorType = ErrorType.User4;
                if (cellError.CellErrorType == CellErrorType.User5      ) info.ErrorType = ErrorType.User5;
                if (cellError.CellErrorType == CellErrorType.User6      ) info.ErrorType = ErrorType.User6;
                if (cellError.CellErrorType == CellErrorType.User7      ) info.ErrorType = ErrorType.User7;
                if (cellError.CellErrorType == CellErrorType.User8      ) info.ErrorType = ErrorType.User8;
                if (cellError.CellErrorType == CellErrorType.User9      ) info.ErrorType = ErrorType.User9;
            } 

            // row Error can override info content
            OnPropertyError?.Invoke (propertyName, info) ;
        }

        public void /*IDXDataErrorInfo*/ GetError(ErrorInfo info)
        {
            OnRowError?.Invoke (info) ;
        }
    }

    public class DynamicNodeDx: DynamicRowDx<Object> 
    {
        public DynamicNodeDx(object entity) : base(entity){}
        public DynamicNodeDx Parent {get;set;}
        public DynamicCollection<Object> Children {get;set;}    // DynamicCollection<Object> : ObservableCollection<DynamicRow<Object>> => public T this[int index] where T = DynamicRow<Object>

        public override string ToString()
        {
            return "DynamicNodeDx <" +  Entity?.GetType() + "> " + Entity ;           
        }

    }
}