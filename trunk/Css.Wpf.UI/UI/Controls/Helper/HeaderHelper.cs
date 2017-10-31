using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Css.Wpf.UI.Controls
{
    public class HeaderHelper : DependencyObject
    {
        public static readonly DependencyProperty HeaderCommandsProperty;

        static HeaderHelper()
        {
            HeaderCommandsProperty = DependencyProperty.RegisterAttached("HeaderCommands", typeof(object), typeof(HeaderHelper), new FrameworkPropertyMetadata());
        }

        public static void SetHeaderCommands(DependencyObject obj, object value)
        {
            obj.SetValue(HeaderCommandsProperty, value);
        }

        public static object GetHeaderCommands(DependencyObject obj)
        {
            return obj.GetValue(HeaderCommandsProperty);
        }
    }
}
