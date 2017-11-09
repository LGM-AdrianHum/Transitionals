//  _____                     _ _   _                   _     
// /__   \_ __ __ _ _ __  ___(_) |_(_) ___  _ __   __ _| |___ 
//   / /\/ '__/ _` | '_ \/ __| | __| |/ _ \| '_ \ / _` | / __|
//  / /  | | | (_| | | | \__ \ | |_| | (_) | | | | (_| | \__ \
//  \/   |_|  \__,_|_| |_|___/_|\__|_|\___/|_| |_|\__,_|_|___/
//                                                            
// Module   : Transitionals/Transitionals/DotsTransition.xaml.cs
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
    ///     Stores the XAML that defines the DotsTransition
    /// </summary>
    [ComVisible(false)]
    public partial class DotsTransitionFrameworkElement : FrameworkElement
    {
        public DotsTransitionFrameworkElement()
        {
            InitializeComponent();
        }
    }

    /// <summary>
    ///     Represents the DotsTransition
    /// </summary>
    [ComVisible(false)]
    public class DotsTransition : StoryboardTransition
    {
        private static readonly DotsTransitionFrameworkElement frameworkElement = new DotsTransitionFrameworkElement();

        public DotsTransition()
        {
            NewContentStyle = (Style) frameworkElement.FindResource("DotsTransitionNewContentStyle");
            NewContentStoryboard = (Storyboard) frameworkElement.FindResource("DotsTransitionNewContentStoryboard");
            Duration = new Duration(TimeSpan.FromSeconds(0.5));
        }

        protected override void OnDurationChanged(Duration oldDuration, Duration newDuration)
        {
            if (NewContentStoryboard != null && NewContentStoryboard.Children.Count > 0)
                NewContentStoryboard.Children[0].Duration = newDuration;
        }
    }
}