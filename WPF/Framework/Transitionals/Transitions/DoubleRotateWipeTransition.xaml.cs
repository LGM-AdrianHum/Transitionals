//  _____                     _ _   _                   _     
// /__   \_ __ __ _ _ __  ___(_) |_(_) ___  _ __   __ _| |___ 
//   / /\/ '__/ _` | '_ \/ __| | __| |/ _ \| '_ \ / _` | / __|
//  / /  | | | (_| | | | \__ \ | |_| | (_) | | | | (_| | \__ \
//  \/   |_|  \__,_|_| |_|___/_|\__|_|\___/|_| |_|\__,_|_|___/
//                                                            
// Module   : Transitionals/Transitionals/DoubleRotateWipeTransition.xaml.cs
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
    ///     Stores the XAML that defines the DoubleRotateWipeTransition
    /// </summary>
    [ComVisible(false)]
    public partial class DoubleRotateWipeTransitionFrameworkElement : FrameworkElement
    {
        public DoubleRotateWipeTransitionFrameworkElement()
        {
            InitializeComponent();
        }
    }

    /// <summary>
    ///     Represents the DoubleRotateWipeTransition
    /// </summary>
    [ComVisible(false)]
    public class DoubleRotateWipeTransition : StoryboardTransition
    {
        private static readonly DoubleRotateWipeTransitionFrameworkElement frameworkElement =
            new DoubleRotateWipeTransitionFrameworkElement();

        public DoubleRotateWipeTransition()
        {
            NewContentStyle = (Style) frameworkElement.FindResource("DoubleRotateWipeTransitionNewContentStyle");
            NewContentStoryboard =
                (Storyboard) frameworkElement.FindResource("DoubleRotateWipeTransitionNewContentStoryboard");
        }

        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId =
            "System.NotSupportedException.#ctor(System.String)")]
        protected override void OnDurationChanged(Duration oldDuration, Duration newDuration)
        {
            throw new NotSupportedException("CTP1 does not support changing the duration of this transition");
        }
    }
}