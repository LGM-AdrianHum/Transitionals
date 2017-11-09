//  _____                     _ _   _                   _     
// /__   \_ __ __ _ _ __  ___(_) |_(_) ___  _ __   __ _| |___ 
//   / /\/ '__/ _` | '_ \/ __| | __| |/ _ \| '_ \ / _` | / __|
//  / /  | | | (_| | | | \__ \ | |_| | (_) | | | | (_| | \__ \
//  \/   |_|  \__,_|_| |_|___/_|\__|_|\___/|_| |_|\__,_|_|___/
//                                                            
// Module   : Transitionals/Transitionals/RollTransition.xaml.cs
// Name     : Adrian Hum - adrianhum 
// Created  : 2017-09-23-11:00 AM
// Modified : 2017-11-10-7:45 AM

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Animation;

namespace Transitionals.Transitions
{
    /// <summary>
    ///     Stores the XAML that defines the RollTransition
    /// </summary>
    [ComVisible(false)]
    public partial class RollTransitionFrameworkElement : FrameworkElement
    {
        public RollTransitionFrameworkElement()
        {
            InitializeComponent();
        }
    }

    /// <summary>
    ///     Represents the RollTransition
    /// </summary>
    [ComVisible(false)]
    public class RollTransition : StoryboardTransition
    {
        private static readonly RollTransitionFrameworkElement frameworkElement = new RollTransitionFrameworkElement();

        static RollTransition()
        {
            AcceptsNullContentProperty.OverrideMetadata(typeof(RollTransition),
                new FrameworkPropertyMetadata(NullContentSupport.New));
            IsNewContentTopmostProperty.OverrideMetadata(typeof(RollTransition), new FrameworkPropertyMetadata(false));
            ClipToBoundsProperty.OverrideMetadata(typeof(RollTransition), new FrameworkPropertyMetadata(true));
        }

        public RollTransition()
        {
            OldContentStyle = (Style) frameworkElement.FindResource("RollTransitionOldContentStyle");
            OldContentStoryboard = (Storyboard) frameworkElement.FindResource("RollTransitionOldContentStoryboard");
            Duration = new Duration(TimeSpan.FromSeconds(0.5));
        }

        protected override void OnDurationChanged(Duration oldDuration, Duration newDuration)
        {
            if (OldContentStoryboard != null && OldContentStoryboard.Children.Count > 0)
                OldContentStoryboard.Children[0].Duration = newDuration;
        }
    }
}