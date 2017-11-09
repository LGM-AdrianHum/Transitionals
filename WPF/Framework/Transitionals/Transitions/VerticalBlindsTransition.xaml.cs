//  _____                     _ _   _                   _     
// /__   \_ __ __ _ _ __  ___(_) |_(_) ___  _ __   __ _| |___ 
//   / /\/ '__/ _` | '_ \/ __| | __| |/ _ \| '_ \ / _` | / __|
//  / /  | | | (_| | | | \__ \ | |_| | (_) | | | | (_| | \__ \
//  \/   |_|  \__,_|_| |_|___/_|\__|_|\___/|_| |_|\__,_|_|___/
//                                                            
// Module   : Transitionals/Transitionals/VerticalBlindsTransition.xaml.cs
// Name     : Adrian Hum - adrianhum 
// Created  : 2017-09-23-11:00 AM
// Modified : 2017-11-10-7:46 AM

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Animation;

namespace Transitionals.Transitions
{
    /// <inheritdoc cref="FrameworkElement" />
    /// <summary>
    ///     Stores the XAML that defines the VerticalBlindsTransition
    /// </summary>
    [ComVisible(false)]
    public partial class VerticalBlindsTransitionFrameworkElement
    {
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public VerticalBlindsTransitionFrameworkElement()
        {
            InitializeComponent();
        }
    }

    /// <summary>
    ///     Represents the VerticalBlindsTransition
    /// </summary>
    [ComVisible(false)]
    public class VerticalBlindsTransition : StoryboardTransition
    {
        private static readonly VerticalBlindsTransitionFrameworkElement FrameworkElement =
            new VerticalBlindsTransitionFrameworkElement();

        /// <summary>
        /// </summary>
        public VerticalBlindsTransition()
        {
            NewContentStyle = (Style) FrameworkElement.FindResource("VerticalBlindsTransitionNewContentStyle");
            NewContentStoryboard =
                (Storyboard) FrameworkElement.FindResource("VerticalBlindsTransitionNewContentStoryboard");
        }

        /// <summary>
        /// </summary>
        /// <param name="oldDuration"></param>
        /// <param name="newDuration"></param>
        /// <exception cref="NotSupportedException"></exception>
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId =
            "System.NotSupportedException.#ctor(System.String)")]
        protected override void OnDurationChanged(Duration oldDuration, Duration newDuration)
        {
            throw new NotSupportedException("CTP1 does not support changing the duration of this transition");
        }
    }
}