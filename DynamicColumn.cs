using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Eortc.Buddy.Mvvm.DynamicCollection
{
    public class DynamicColumn : ObservableObject , IDynamicColumn
    {
        public DynamicColumn()
        {
            IsVisible      = true ;
            IsReadOnly     = false ;
            GroupIndex     = -1 ;
            DisplayIndex   = -1 ;
            FixedAlignment = "Left" ;
            //TemplateName   = "DefaultColumnTemplate";
        }

        //                                                     Dev Express                                                                Telerik
        //                                                     ------------                                                               ------------
        private string               _name ;                // FieldName                                                                  UniqueName
        private Binding              _dataMemberBinding;    //                                                                            DataMemberBinding
        private string               _displayName ;         // Header                                                                     Header
        private Type                 _type ;             
        private bool                 _isVisible ;           // Visible                                                                    IsVisible
        private bool                 _isReadOnly ;          // ReadOnly                                                                   IsReadOnly   
        private int                  _groupIndex ;       
        private string               _fixedAlignment ;      // Fixed
        private string               _templateName ;    
        private IList                _comboSource ;         // <dxe:ComboBoxEditSettings ItemsSource>
        private string               _comboDisplayMember ;       // <dxe:ComboBoxEditSettings DisplayMember>
        private string               _comboNullablePlacement ; // <dxe:ComboBoxEditSettings NullValueButtonPlacement="EditBox" or "None" or "Popup" > 
        private int                  _displayIndex ;        // ?                                                                          DisplayIndex
        private Style                _headerCellStyle;      // ?                                                                          HeaderCellStyle
        private bool                 _isResizable;          // ?                                                                          IsResizable
        private bool                 _isSorteable;          // ?                                                                          IsSorteable
        private bool                 _isGroupable;          // ?                                                                          IsGroupable
        private bool                 _isFilterable;         // ?                                                                          IsFilterable
        private TextAlignment        _headerTextAlignment;  // ?                                                                          HeaderTextAlignment
        private StyleSelector        _cellStyleSelector;    // ?                                                                          CellStyleSelector
        private DataTemplateSelector _cellTemplateSelector; // ?                                                                          CellTemplateSelector
        private double               _maxWidth = 1000;      // ?                                                                          MaxWidth
        private double               _width = -1;           // Width                                                                      ?
        private object               _tag ;                 // ?                                                                          ?

        public string Name                                  { get { return _name; }                 set { Set(() => Name,                   ref _name          , value); }}
        public string DisplayName                           { get { return _displayName; }          set { Set(() => DisplayName,            ref _displayName   , value); }}   
        public Type Type                                    { get { return _type; }                 set { Set(() => Type,                   ref _type          , value); }}   // object type
        public bool IsVisible                               { get { return _isVisible; }            set { Set(() => IsVisible,              ref _isVisible     , value); }}   //
        public bool IsReadOnly                              { get { return _isReadOnly; }           set { Set(() => IsReadOnly,             ref _isReadOnly    , value); }}
        public Attribute[] Attributes                       { get; set; }
        public int GroupIndex                               { get { return _groupIndex; }           set { Set(() => GroupIndex,             ref _groupIndex    , value); }}   // devexpress ?
        public string FixedAlignment                        { get { return _fixedAlignment; }       set { Set(() => FixedAlignment,         ref _fixedAlignment, value); }}
        public string TemplateName                          { get { return _templateName; }         set { Set(() => TemplateName,           ref _templateName  , value); }}   // devexpress ColumnGeneratorTemplateSelector
        public IList ComboSource                            { get { return _comboSource; }          set { Set(() => ComboSource,            ref _comboSource   , value); }}   // devexpress ComboBoxEditSettings.ItemsSource
        public string ComboDisplayMember                    { get { return _comboDisplayMember; }   set { Set(() => ComboDisplayMember,     ref _comboDisplayMember , value); }}   // devexpress ComboBoxEditSettings.DisplayMember 
        public string ComboNullablePlacement                { get { return _comboNullablePlacement; } set { Set(() => ComboNullablePlacement,ref _comboNullablePlacement , value); }}   // devexpress ComboBoxEditSettings.NullValueButtonPlacement 
        public int DisplayIndex                             { get { return _displayIndex; }         set { Set(() => DisplayIndex,           ref _displayIndex , value); }} 
        public Style HeaderCellStyle                        { get { return _headerCellStyle; }      set { Set(() => HeaderCellStyle,        ref _headerCellStyle , value); }}         
        public Binding DataMemberBinding                    { get { return _dataMemberBinding; }    set { Set(() => DataMemberBinding,      ref _dataMemberBinding , value); }}             
        public bool IsResizable                             { get { return _isResizable; }          set { Set(() => IsResizable,            ref _isResizable , value); }} 
        public bool IsSorteable                             { get { return _isSorteable; }          set { Set(() => IsSorteable,            ref _isSorteable , value); }} 
        public bool IsGroupable                             { get { return _isGroupable; }          set { Set(() => IsGroupable,            ref _isGroupable , value); }} 
        public bool IsFilterable                            { get { return _isFilterable; }         set { Set(() => IsFilterable,           ref _isFilterable , value); }}     
        public TextAlignment HeaderTextAlignment            { get { return _headerTextAlignment; }  set { Set(() => HeaderTextAlignment,    ref _headerTextAlignment , value); }}                     
        public StyleSelector CellStyleSelector              { get { return _cellStyleSelector; }    set { Set(() => CellStyleSelector,      ref _cellStyleSelector , value); }}                 
        public DataTemplateSelector CellTemplateSelector    { get { return _cellTemplateSelector; } set { Set(() => CellTemplateSelector,   ref _cellTemplateSelector , value); }}                             
        public double MaxWidth                              { get { return _maxWidth; }             set { Set(() => MaxWidth,               ref _maxWidth , value); }} 
        public double Width                                 { get { return _width; }                set { Set(() => Width,                  ref _width , value); }} 
        public object Tag                                   { get { return _tag; }                  set { Set(() => Tag,                    ref _tag , value); }}
    }
}
