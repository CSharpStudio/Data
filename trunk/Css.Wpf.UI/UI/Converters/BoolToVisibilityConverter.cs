/*************************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up the Plus Edition at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like http://facebook.com/datagrids

  ***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Globalization;

namespace Css.Wpf.UI.Converters
{
    /// <summary>
    /// An implementation of <see cref="IValueConverter"/> that converts boolean values to <see cref="Visibility"/> values.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <c>BooleanToVisibilityConverter</c> class can be used to convert boolean values (or values that can be converted to boolean values) to
    /// <see cref="Visibility"/> values. By default, <see langword="true"/> is converted to <see cref="Visibility.Visible"/> and <see langword="false"/>
    /// is converted to <see cref="Visibility.Collapsed"/>. However, the <see cref="UseHidden"/> property can be set to <see langword="true"/> in order
    /// to return <see cref="Visibility.Hidden"/> instead of <see cref="Visibility.Collapsed"/>. In addition, the <see cref="IsReversed"/> property
    /// can be set to <see langword="true"/> to reverse the returned values.
    /// </para>
    /// </remarks>
    /// <example>
    /// The following example shows how a <c>BooleanToVisibilityConverter</c> can be used to display a <c>TextBox</c> only when a property is <c>true</c>:
    /// <code lang="xml">
    /// <![CDATA[
    /// <TextBox Visibility="{Binding ShowTheTextBox, Converter={BooleanToVisibilityConverter}}"/>
    /// ]]>
    /// </code>
    /// </example>
    /// <example>
    /// The following example shows how a <c>BooleanToVisibilityConverter</c> can be used to display a <c>TextBox</c> only when a property is <c>true</c>.
    /// Rather than collapsing the <c>TextBox</c>, it is hidden:
    /// <code lang="xml">
    /// <![CDATA[
    /// <TextBox Visibility="{Binding ShowTheTextBox, Converter={BooleanToVisibilityConverter UseHidden=true}}"/>
    /// ]]>
    /// </code>
    /// </example>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        private bool isReversed;
        private bool useHidden;

        /// <summary>
        /// Initializes a new instance of the BooleanToVisibilityConverter class.
        /// </summary>
        public BoolToVisibilityConverter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the BooleanToVisibilityConverter class.
        /// </summary>
        /// <param name="isReversed">
        /// Whether the return values should be reversed.
        /// </param>
        /// <param name="useHidden">
        /// Whether <see cref="Visibility.Hidden"/> should be used instead of <see cref="Visibility.Collapsed"/>.
        /// </param>
        public BoolToVisibilityConverter(bool isReversed, bool useHidden)
        {
            this.isReversed = isReversed;
            this.useHidden = useHidden;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the return values should be reversed.
        /// </summary>
        public bool IsReversed
        {
            get { return this.isReversed; }
            set { this.isReversed = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="Visibility.Hidden"/> should be returned instead of <see cref="Visibility.Collapsed"/>.
        /// </summary>
        public bool UseHidden
        {
            get { return this.useHidden; }
            set { this.useHidden = value; }
        }

        /// <summary>
        /// Attempts to convert the specified value.
        /// </summary>
        /// <param name="value">
        /// The value to convert.
        /// </param>
        /// <param name="targetType">
        /// The type of the binding target property.
        /// </param>
        /// <param name="parameter">
        /// The converter parameter to use.
        /// </param>
        /// <param name="culture">
        /// The culture to use in the converter.
        /// </param>
        /// <returns>
        /// A converted value.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (value is bool)
            //{
            //    bool val = (bool)value;
            //    if (this.IsReversed)
            //        val = !val;
            //    if (val)
            //        return Visibility.Visible;
            //    return this.UseHidden ? Visibility.Hidden : Visibility.Collapsed;
            //}
            if (value != null)
            {
                var b = (bool)System.Convert.ChangeType(value, TypeCode.Boolean);
                if (this.IsReversed)
                    b = !b;
                if (b)
                    return Visibility.Visible;
                return this.UseHidden ? Visibility.Hidden : Visibility.Collapsed;
            }
            if (value == null)
            {
                if (this.IsReversed)
                    return Visibility.Visible;
                else
                    return this.UseHidden ? Visibility.Hidden : Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        /// <summary>
        /// Attempts to convert the specified value back.
        /// </summary>
        /// <param name="value">
        /// The value to convert.
        /// </param>
        /// <param name="targetType">
        /// The type of the binding target property.
        /// </param>
        /// <param name="parameter">
        /// The converter parameter to use.
        /// </param>
        /// <param name="culture">
        /// The culture to use in the converter.
        /// </param>
        /// <returns>
        /// A converted value.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Visibility))
            {
                return DependencyProperty.UnsetValue;
            }

            var visibility = (Visibility)value;
            var result = visibility == Visibility.Visible;

            if (this.IsReversed)
            {
                result = !result;
            }

            return result;
        }
    }
}