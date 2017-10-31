﻿namespace Css.Wpf.UI.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Animation;

    /// <summary>
    ///     A special animation used to animates the length of a <see cref="Grid" />.
    /// </summary>
    /// <seealso cref="System.Windows.Media.Animation.AnimationTimeline" />
    /// <autogeneratedoc />
    public class GridLengthAnimation : AnimationTimeline
    {
        public static readonly DependencyProperty FromProperty = DependencyProperty.Register(nameof(From), typeof(GridLength), typeof(GridLengthAnimation));

        public GridLength From
        {
            get { return (GridLength)this.GetValue(FromProperty); }
            set { this.SetValue(FromProperty, value); }
        }

        public static readonly DependencyProperty ToProperty = DependencyProperty.Register(nameof(To), typeof(GridLength), typeof(GridLengthAnimation));

        public GridLength To
        {
            get { return (GridLength)this.GetValue(ToProperty); }
            set { this.SetValue(ToProperty, value); }
        }

        public override Type TargetPropertyType
        {
            get { return typeof(GridLength); }
        }

        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
        {
            var from = (GridLength)this.GetValue(FromProperty);
            var to = (GridLength)this.GetValue(ToProperty);
            if (from.GridUnitType != to.GridUnitType) // We can't animate different types, so just skip straight to it
                return to;
            var fromVal = from.Value;
            var toVal = to.Value;

            if (fromVal > toVal)
            {
                return new GridLength((1 - animationClock.CurrentProgress.Value) * (fromVal - toVal) + toVal, GridUnitType.Star);
            }
            return new GridLength(animationClock.CurrentProgress.Value * (toVal - fromVal) + fromVal, GridUnitType.Star);
        }

        protected override Freezable CreateInstanceCore()
        {
            return new GridLengthAnimation();
        }
    }
}