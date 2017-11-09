//  _____                     _ _   _                   _     
// /__   \_ __ __ _ _ __  ___(_) |_(_) ___  _ __   __ _| |___ 
//   / /\/ '__/ _` | '_ \/ __| | __| |/ _ \| '_ \ / _` | / __|
//  / /  | | | (_| | | | \__ \ | |_| | (_) | | | | (_| | \__ \
//  \/   |_|  \__,_|_| |_|___/_|\__|_|\___/|_| |_|\__,_|_|___/
//                                                            
// Module   : Transitionals/Transitionals/FadeTransition.cs
// Name     : Adrian Hum - adrianhum 
// Created  : 2017-09-23-11:00 AM
// Modified : 2017-11-10-7:45 AM

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Transitionals.Controls;

namespace Transitionals.Transitions
{
    // Simple transition that fades out the old content
    /// <summary>
    /// </summary>
    [ComVisible(false)]
    public class FadeTransition : Transition
    {
        static FadeTransition()
        {
            AcceptsNullContentProperty.OverrideMetadata(typeof(FadeTransition),
                new FrameworkPropertyMetadata(NullContentSupport.New));
            IsNewContentTopmostProperty.OverrideMetadata(typeof(FadeTransition), new FrameworkPropertyMetadata(false));
        }

        /// <summary>
        /// </summary>
        public FadeTransition()
        {
            Duration = new Duration(TimeSpan.FromSeconds(0.5));
        }

        /// <summary>
        /// </summary>
        /// <param name="transitionElement"></param>
        /// <param name="oldContent"></param>
        /// <param name="newContent"></param>
        protected internal override void BeginTransition(TransitionElement transitionElement,
            ContentPresenter oldContent, ContentPresenter newContent)
        {
            var da = new DoubleAnimation(0, Duration);
            da.Completed += delegate { EndTransition(transitionElement, oldContent, newContent); };
            oldContent.BeginAnimation(UIElement.OpacityProperty, da);
        }

        /// <summary>
        /// </summary>
        /// <param name="transitionElement"></param>
        /// <param name="oldContent"></param>
        /// <param name="newContent"></param>
        protected override void OnTransitionEnded(TransitionElement transitionElement, ContentPresenter oldContent,
            ContentPresenter newContent)
        {
            oldContent.BeginAnimation(UIElement.OpacityProperty, null);
        }
    }
}