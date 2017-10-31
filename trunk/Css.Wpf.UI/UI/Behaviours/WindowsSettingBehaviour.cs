using System.Windows.Interactivity;
using Css.Wpf.UI.Controls;

namespace Css.Wpf.UI.Behaviours
{
    public class WindowsSettingBehaviour : Behavior<MetroWindow>
    {
        protected override void OnAttached()
        {
            if (AssociatedObject != null && AssociatedObject.SaveWindowPosition) {
                // save with custom settings class or use the default way
                var windowPlacementSettings = this.AssociatedObject.GetWindowPlacementSettings();
                WindowSettings.SetSave(AssociatedObject, windowPlacementSettings);
            }
        }
    }
}