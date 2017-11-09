//  _____                     _ _   _                   _     
// /__   \_ __ __ _ _ __  ___(_) |_(_) ___  _ __   __ _| |___ 
//   / /\/ '__/ _` | '_ \/ __| | __| |/ _ \| '_ \ / _` | / __|
//  / /  | | | (_| | | | \__ \ | |_| | (_) | | | | (_| | \__ \
//  \/   |_|  \__,_|_| |_|___/_|\__|_|\___/|_| |_|\__,_|_|___/
//                                                            
// Module   : Transitionals/Transitionals/HorizontalBlindsTransition.xaml.cs
// Name     : Adrian Hum - adrianhum 
// Created  : 2017-09-23-11:00 AM
// Modified : 2017-11-10-7:45 AM

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Animation;

namespace Transitionals.Transitions
{
    /// <summary>
    ///     Stores the XAML that defines the HorizontalBlindsTransition
    /// </summary>
    [ComVisible(false)]
    public partial class HorizontalBlindsTransitionFrameworkElement : FrameworkElement
    {
        public HorizontalBlindsTransitionFrameworkElement()
        {
            InitializeComponent();
        }
    }

    /// <summary>
    ///     Represents the HorizontalBlindsTransition
    /// </summary>
    [ComVisible(false)]
    public class HorizontalBlindsTransition : StoryboardTransition
    {
        private static readonly HorizontalBlindsTransitionFrameworkElement frameworkElement =
            new HorizontalBlindsTransitionFrameworkElement();

        public HorizontalBlindsTransition()
        {
            NewContentStyle = (Style) frameworkElement.FindResource("HorizontalBlindsTransitionNewContentStyle");
            NewContentStoryboard =
                (Storyboard) frameworkElement.FindResource("HorizontalBlindsTransitionNewContentStoryboard");
        }

        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId =
            "System.NotSupportedException.#ctor(System.String)")]
        protected override void OnDurationChanged(Duration oldDuration, Duration newDuration)
        {
            throw new NotSupportedException("CTP1 does not support changing the duration of this transition");
        }
    }
}