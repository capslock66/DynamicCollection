using System;
using DynamicCollection;
//using TraceTool;

//<UserControl.Resources>
//  <dynamicCollection:DynamicColumnTemplateSelector DefaultColumnTemplate = "{StaticResource DefaultColumnTemplate}" x:Key="DynamicColumnTemplateSelector"/>

//<dxg:GridControl        
//    ColumnGeneratorTemplateSelector="{StaticResource DynamicColumnTemplateSelector}"
//</dxg:GridControl>



namespace Eortc.Buddy.Mvvm.DynamicCollection
{
    public class DynamicColumnTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultColumnTemplate { get; set; }
        public string DefaultColumnTemplateName { get; set; }

        public override DataTemplate SelectTemplate(
            object item,                    // The data object for which to select the template.
            DependencyObject container)     // The data-bound object
        {

            // item      : Eortc.Buddy.Mvvm.DynamicCollection.DynamicColumn
            // container : DevExpress.Xpf.Grid.GridControl or descendant like Eortc.Buddy.Views.Extensions.DevExpressExtensions.GridControlBandEnhanced

            // TTrace.Debug.Send("SelectTemplate , item : " + item?.GetType()) ;  // => DynamicColumn 

            DynamicColumn dynCol = item as DynamicColumn;      // Eortc.Buddy.Mvvm.DynamicCollection.DynamicColumn
            if (dynCol == null)
                throw new Exception("DynamicColumnTemplateSelector.SelectTemplate : item parameter is not a DynamicColumn : " + item?.GetType());
            if (container == null )
                throw new Exception("DynamicColumnTemplateSelector.SelectTemplate : container parameter is Null");

            string resourceName = "Before determining template name";   // needed for exception tracking
            try
            {
                object resource;

                if (!string.IsNullOrEmpty(dynCol.TemplateName)) {                                   // template name is specified in Dynamic column
                    resourceName = "dynCol.TemplateName="+dynCol.TemplateName ;
                    resource = ((Control) container).FindResource(dynCol.TemplateName);
                    if (resource == null)
                    {
                        TTrace.Error.SendStack($"Unable to load template {resourceName} for {dynCol.Name} " ) 
                                    .Send("When", resourceName) 
                                    .SendObject("container", container) ;
                        return null ;
                    }
                    // resource can be DependencyProperty.UnsetValue

                } else if (DefaultColumnTemplate != null) {                                        // DefaultDataTemplate is giving DefaultColumnTemplate="xxx"
                    resourceName = "property DefaultColumnTemplate != null" ;
                    resource = DefaultColumnTemplate;

                    if (resource == null)
                    {
                        TTrace.Error.SendStack("DynamicColumnTemplateSelector : resource null.  Resource way : " + resourceName) 
                                    .Send("When", resourceName) 
                                    .SendObject("container", container) ;

                        return null ;
                    }

                } else if (DefaultColumnTemplateName != null) { 
                    resourceName = "property DefaultColumnTemplateName != null" ;
                    resource = ((Control)container).FindResource(DefaultColumnTemplateName);    // DefaultDataTemplateName is giving DefaultColumnTemplateName="xxx"
                    if (resource == null)
                    {
                        TTrace.Error.SendStack($"Unable to load template {DefaultColumnTemplateName} for {dynCol.Name} ")
                                    .Send("When", resourceName) 
                                    .SendObject("container", container) ;
                        return null ;
                    }
                } else {
                    resourceName = "DEFAULT : Try find 'DefaultColumnTemplate'" ;
                    resource = ((Control)container).FindResource("DefaultColumnTemplate");      // nothing giving. Try to find a "DefaultColumnTemplate"
                    if (resource == null)
                    {
                        TTrace.Error.SendStack("DefaultColumnTemplate not found" ) 
                                    .Send("When", resourceName) 
                                    .SendObject("container", container) ;
                        return null ;
                    }
                }

                if (resource == DependencyProperty.UnsetValue)
                {
                    TTrace.Debug.SendStack($"DynamicColumnTemplateSelector : resource '{resourceName}' for column '{dynCol.Name}' is UnsetValue") 
                                .Send("When", resourceName) 
                                .SendObject("container", container) ;
                    return null ;
                }

                resourceName = "Cast resource to DataTemplate" ;

                DataTemplate result = resource as DataTemplate ;
                if (result == null)
                {
                    var node = TTrace.Debug.Send("DynamicColumnTemplateSelector : resource is not DataTemplate ") ;
                    
                    node.Send("resource way : " + resourceName) ;
                    node.Send($"column : {dynCol.Name} / {dynCol.DisplayName}") ;
                    node.SendObject($"resource type : {resource.GetType()} / {resource}", resource);
                }
                return result ;
            }
            catch (Exception e)
            {
                var trace = TTrace.Error.Send($"DynamicColumnTemplateSelector : Unable to find resource on column {dynCol.Name}, Error : {e.Message}") ;
                trace.Send("When", resourceName) ;
                var traceContainer = trace.Send("container", container.ToString()) ;
                dynamic gridControl = container ;   // use dynamic to don't reference DevExpress.Xpf.Grid.GridControl in this DLL
                traceContainer.Send("DataContext",gridControl.DataContext.ToString() ) ;
                traceContainer.Send("ItemsSource",gridControl.ItemsSource.ToString() ) ;
                traceContainer.Send("SelectedItem",gridControl.SelectedItem.ToString() ) ;

                return null ;                
            }
        }

    }
}