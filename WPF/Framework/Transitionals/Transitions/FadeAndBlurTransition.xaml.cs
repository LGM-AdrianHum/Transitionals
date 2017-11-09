//  _____                     _ _   _                   _     
// /__   \_ __ __ _ _ __  ___(_) |_(_) ___  _ __   __ _| |___ 
//   / /\/ '__/ _` | '_ \/ __| | __| |/ _ \| '_ \ / _` | / __|
//  / /  | | | (_| | | | \__ \ | |_| | (_) | | | | (_| | \__ \
//  \/   |_|  \__,_|_| |_|___/_|\__|_|\___/|_| |_|\__,_|_|___/
//                                                            
// Module   : Transitionals/Transitionals/FadeAndBlurTransition.xaml.cs
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
    ///     Stores the XAML that defines the FadeAndBlurTransition
    /// </summary>
    [ComVisible(false)]
    public partial class FadeAndBlurTransitionFrameworkElement : FrameworkElement
    {
        public FadeAndBlurTransitionFrameworkElement()
        {
            InitializeComponent();
        }
    }

    /// <summary>
    ///     Represents the FadeAndBlurTransition
    /// </summary>
    [ComVisible(false)]
    public class FadeAndBlurTransition : StoryboardTransition
    {
        private static readonly FadeAndBlurTransitionFrameworkElement frameworkElement =
            new FadeAndBlurTransitionFrameworkElement();

        static FadeAndBlurTransition()
        {
            AcceptsNullContentProperty.OverrideMetadata(typeof(FadeAndBlurTransition),
                new FrameworkPropertyMetadata(NullContentSupport.New));
            IsNewContentTopmostProperty.OverrideMetadata(typeof(FadeAndBlurTransition),
                new FrameworkPropertyMetadata(false));
        }

        public FadeAndBlurTransition()
        {
            OldContentStyle = (Style) frameworkElement.FindResource("FadeAndBlurTransitionOldContentStyle");
            OldContentStoryboard =
                (Storyboard) frameworkElement.FindResource("FadeAndBlurTransitionOldContentStoryboard");
            NewContentStyle = (Style) frameworkElement.FindResource("FadeAndBlurTransitionNewContentStyle");
            NewContentStoryboard =
                (Storyboard) frameworkElement.FindResource("FadeAndBlurTransitionNewContentStoryboard");
        }

        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId =
            "System.NotSupportedException.#ctor(System.String)")]
        protected override void OnDurationChanged(Duration oldDuration, Duration newDuration)
        {
            throw new NotSupportedException("CTP1 does not support changing the duration of this transition");
        }
    }
}