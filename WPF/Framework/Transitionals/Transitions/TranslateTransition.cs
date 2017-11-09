//  _____                     _ _   _                   _     
// /__   \_ __ __ _ _ __  ___(_) |_(_) ___  _ __   __ _| |___ 
//   / /\/ '__/ _` | '_ \/ __| | __| |/ _ \| '_ \ / _` | / __|
//  / /  | | | (_| | | | \__ \ | |_| | (_) | | | | (_| | \__ \
//  \/   |_|  \__,_|_| |_|___/_|\__|_|\___/|_| |_|\__,_|_|___/
//                                                            
// Module   : Transitionals/Transitionals/TranslateTransition.cs
// Name     : Adrian Hum - adrianhum 
// Created  : 2017-09-23-11:00 AM
// Modified : 2017-11-10-7:46 AM

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Transitionals.Controls;

namespace Transitionals.Transitions
{
    // Applies a Translation to the content.  You can specify the starting point of the new 
    // content or the ending point of the old content using relative coordinates.
    // Set start point to (-1,0) to have the content slide from the left 
    /// <summary>
    /// </summary>
    [ComVisible(false)]
    public class TranslateTransition : Transition
    {
        /// <summary>
        /// </summary>
        public static readonly DependencyProperty StartPointProperty =
            DependencyProperty.Register("StartPoint", typeof(Point), typeof(TranslateTransition),
                new UIPropertyMetadata(new Point()));

        /// <summary>
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "EndPoint")]
        public static readonly DependencyProperty EndPointProperty =
            DependencyProperty.Register("EndPoint", typeof(Point), typeof(TranslateTransition),
                new UIPropertyMetadata(new Point()));

        static TranslateTransition()
        {
            ClipToBoundsProperty.OverrideMetadata(typeof(TranslateTransition), new FrameworkPropertyMetadata(true));
        }

        /// <summary>
        /// </summary>
        public TranslateTransition()
        {
            Duration = new Duration(TimeSpan.FromSeconds(0.5));
            StartPoint = new Point(-1, 0);
        }

        /// <summary>
        /// </summary>
        public Point StartPoint
        {
            get { return (Point) GetValue(StartPointProperty); }
            set { SetValue(StartPointProperty, value); }
        }

        /// <summary>
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "EndPoint")]
        public Point EndPoint
        {
            get { return (Point) GetValue(EndPointProperty); }
            set { SetValue(EndPointProperty, value); }
        }

        /// <summary>
        /// </summary>
        /// <param name="transitionElement"></param>
        /// <param name="oldContent"></param>
        /// <param name="newContent"></param>
        protected internal override void BeginTransition(TransitionElement transitionElement,
            ContentPresenter oldContent, ContentPresenter newContent)
        {
            var tt = new TranslateTransform(StartPoint.X * transitionElement.ActualWidth,
                StartPoint.Y * transitionElement.ActualHeight);

            if (IsNewContentTopmost)
                newContent.RenderTransform = tt;
            else
                oldContent.RenderTransform = tt;

            var da = new DoubleAnimation(EndPoint.X * transitionElement.ActualWidth, Duration);
            tt.BeginAnimation(TranslateTransform.XProperty, da);

            da.To = EndPoint.Y * transitionElement.ActualHeight;
            da.Completed += delegate { EndTransition(transitionElement, oldContent, newContent); };
            tt.BeginAnimation(TranslateTransform.YProperty, da);
        }

        /// <summary>
        /// </summary>
        /// <param name="transitionElement"></param>
        /// <param name="oldContent"></param>
        /// <param name="newContent"></param>
        protected override void OnTransitionEnded(TransitionElement transitionElement, ContentPresenter oldContent,
            ContentPresenter newContent)
        {
            newContent.ClearValue(UIElement.RenderTransformProperty);
            oldContent.ClearValue(UIElement.RenderTransformProperty);
        }
    }
}