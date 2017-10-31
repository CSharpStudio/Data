using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Css.Wpf.UI.Models
{
    public class BindingEvaluator : FrameworkElement
    {
        public object Value
        {
            get
            {
                Value = DependencyProperty.UnsetValue;
                SetBinding(BindingEvaluator.ValueProperty, _binding);
                return (object)GetValue(ValueProperty);
            }
            private set { SetValue(ValueProperty, value); }
        }
        static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(BindingEvaluator), null);

        BindingBase _binding;
        public BindingEvaluator(BindingBase binding)
        {
            _binding = binding;
        }
    }
}
