using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;

namespace Css.Wpf.UI.Controls.MultiComboBox
{
    [DefaultProperty("Columns")]
    [ContentProperty("Columns")]
    public class MultiComboBox : ComboBox
    {
        const string partPopupDataGrid = "PART_PopupDataGrid";
        //Columns of DataGrid
        private ObservableCollection<DataGridTextColumn> columns;
        //Attached DataGrid control
        private DataGrid popupDataGrid;

        static MultiComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiComboBox), new FrameworkPropertyMetadata(typeof(MultiComboBox)));
        }

        //The property is default and Content property for CustComboBox
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<DataGridTextColumn> Columns
        {
            get
            {
                if (this.columns == null)
                {
                    this.columns = new ObservableCollection<DataGridTextColumn>();
                }
                return this.columns;
            }
        }

        //Apply theme and attach columns to DataGrid popupo control
        public override void OnApplyTemplate()
        {
            if (popupDataGrid == null)
            {
                
                popupDataGrid = this.Template.FindName(partPopupDataGrid, this) as DataGrid;
                if (popupDataGrid != null && columns != null)
                {
                    //Add columns to DataGrid columns
                    for (int i = 0; i < columns.Count; i++)
                        popupDataGrid.Columns.Add(columns[i]);
                    
                    //Add event handler for DataGrid popup
                    popupDataGrid.MouseDown += new MouseButtonEventHandler(popupDataGrid_MouseDown);
                    popupDataGrid.SelectionChanged += new SelectionChangedEventHandler(popupDataGrid_SelectionChanged);
                }
            }
            //Call base class method
            base.OnApplyTemplate();
        }

        //Synchronize selection between Combo and DataGrid popup
        void popupDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //When open in Blend prevent raising exception 
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                DataGrid dg = sender as DataGrid;
                if (dg != null)
                {
                    this.SelectedItem = dg.SelectedItem;
                    this.SelectedValue = dg.SelectedValue;
                    this.SelectedIndex = dg.SelectedIndex;
                    this.SelectedValuePath = dg.SelectedValuePath;
                }
            }
        }

        //Event for DataGrid popup MouseDown
        void popupDataGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            if (dg != null)
            {
                DependencyObject dep = (DependencyObject)e.OriginalSource;

                // iteratively traverse the visual tree and stop when dep is one of ..
                while ((dep != null) &&
                        !(dep is DataGridCell) &&
                        !(dep is DataGridColumnHeader))
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }

                if (dep == null)
                    return;

                if (dep is DataGridColumnHeader)
                {
                   // do something
                }
                //When user clicks to DataGrid cell, popup have to be closed
                if (dep is DataGridCell)
                {
                    this.IsDropDownOpen = false;                   
                }
            }
        }

        //When selection changed in combobox (pressing  arrow key down or up) must be synchronized with opened DataGrid popup
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (popupDataGrid != null)
            {
                if (!DesignerProperties.GetIsInDesignMode(this))
                {
                    if (IsDropDownOpen)
                    {
                        popupDataGrid.SelectedItem = this.SelectedItem;
                        if (popupDataGrid.SelectedItem != null)
                            popupDataGrid.ScrollIntoView(popupDataGrid.SelectedItem);
                        IsDropDownOpen = false;
                    }
                }
            }
            base.OnSelectionChanged(e);
        }

        protected override void OnDropDownOpened(EventArgs e)
        { 
            popupDataGrid.SelectedItem = this.SelectedItem;
            
            base.OnDropDownOpened(e);

            if (popupDataGrid.SelectedItem != null)
                popupDataGrid.ScrollIntoView(popupDataGrid.SelectedItem);
        }         
    }
}

