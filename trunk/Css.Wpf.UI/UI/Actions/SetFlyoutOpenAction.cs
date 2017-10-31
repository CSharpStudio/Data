using System;
using System.Windows;
using System.Windows.Interactivity;
using Css.Wpf.UI.Controls;

namespace Css.Wpf.UI.Actions
{
    [Obsolete(@"This TargetedTriggerAction will be deleted in the next release.")]
    public class SetFlyoutOpenAction : TargetedTriggerAction<FrameworkElement>
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(bool), typeof(SetFlyoutOpenAction), new PropertyMetadata(default(bool)));

        public bool Value
        {
            get { return (bool)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        protected override void Invoke(object parameter)
        {
            ((Flyout) TargetObject).IsOpen = Value;
        }
    }
}
