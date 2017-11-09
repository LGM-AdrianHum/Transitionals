//  _____                     _ _   _                   _     
// /__   \_ __ __ _ _ __  ___(_) |_(_) ___  _ __   __ _| |___ 
//   / /\/ '__/ _` | '_ \/ __| | __| |/ _ \| '_ \ / _` | / __|
//  / /  | | | (_| | | | \__ \ | |_| | (_) | | | | (_| | \__ \
//  \/   |_|  \__,_|_| |_|___/_|\__|_|\___/|_| |_|\__,_|_|___/
//                                                            
// Module   : Transitionals/Transitionals/StoryboardTransition.cs
// Name     : Adrian Hum - adrianhum 
// Created  : 2017-09-23-11:00 AM
// Modified : 2017-11-10-7:46 AM

using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Transitionals.Controls;

namespace Transitionals.Transitions
{
    // Transition with storyboards for the old and new content presenters
    /// <summary>
    /// </summary>
    [StyleTypedProperty(Property = "OldContentStyle", StyleTargetType = typeof(ContentPresenter))]
    [StyleTypedProperty(Property = "NewContentStyle", StyleTargetType = typeof(ContentPresenter))]
    [ComVisible(false)]
    public abstract class StoryboardTransition : Transition
    {
        /// <summary>
        /// </summary>
        public static readonly DependencyProperty OldContentStyleProperty =
            DependencyProperty.Register("OldContentStyle",
                typeof(Style),
                typeof(StoryboardTransition),
                new UIPropertyMetadata(null));

        /// <summary>
        /// </summary>
        public static readonly DependencyProperty OldContentStoryboardProperty =
            DependencyProperty.Register("OldContentStoryboard",
                typeof(Storyboard),
                typeof(StoryboardTransition),
                new UIPropertyMetadata(null));

        /// <summary>
        /// </summary>
        public static readonly DependencyProperty NewContentStyleProperty =
            DependencyProperty.Register("NewContentStyle",
                typeof(Style),
                typeof(StoryboardTransition),
                new UIPropertyMetadata(null));

        /// <summary>
        /// </summary>
        public static readonly DependencyProperty NewContentStoryboardProperty =
            DependencyProperty.Register("NewContentStoryboard",
                typeof(Storyboard),
                typeof(StoryboardTransition),
                new UIPropertyMetadata(null));

        /// <summary>
        /// </summary>
        public Style OldContentStyle
        {
            get { return (Style) GetValue(OldContentStyleProperty); }
            set { SetValue(OldContentStyleProperty, value); }
        }


        /// <summary>
        /// </summary>
        public Storyboard OldContentStoryboard
        {
            get { return (Storyboard) GetValue(OldContentStoryboardProperty); }
            set { SetValue(OldContentStoryboardProperty, value); }
        }

        /// <summary>
        /// </summary>
        public Style NewContentStyle
        {
            get { return (Style) GetValue(NewContentStyleProperty); }
            set { SetValue(NewContentStyleProperty, value); }
        }

        /// <summary>
        /// </summary>
        public Storyboard NewContentStoryboard
        {
            get { return (Storyboard) GetValue(NewContentStoryboardProperty); }
            set { SetValue(NewContentStoryboardProperty, value); }
        }

        /// <summary>
        /// </summary>
        /// <param name="transitionElement"></param>
        /// <param name="oldContent"></param>
        /// <param name="newContent"></param>
        protected internal override void BeginTransition(TransitionElement transitionElement,
            ContentPresenter oldContent, ContentPresenter newContent)
        {
            var oldStoryboard = OldContentStoryboard;
            var newStoryboard = NewContentStoryboard;

            if (oldStoryboard != null || newStoryboard != null)
            {
                oldContent.Style = OldContentStyle;
                newContent.Style = NewContentStyle;

                // Flag to determine when both storyboards are done
                var done = oldStoryboard == null || newStoryboard == null;

                if (oldStoryboard != null)
                {
                    oldStoryboard = oldStoryboard.Clone();
                    oldContent.SetValue(OldContentStoryboardProperty, oldStoryboard);
                    oldStoryboard.Completed += delegate
                    {
                        if (done)
                            EndTransition(transitionElement, oldContent, newContent);
                        done = true;
                    };
                    oldStoryboard.Begin(oldContent, true);
                }

                if (newStoryboard == null) return;
                newStoryboard = newStoryboard.Clone();
                newContent.SetValue(NewContentStoryboardProperty, newStoryboard);
                newStoryboard.Completed += delegate
                {
                    if (done)
                        EndTransition(transitionElement, oldContent, newContent);
                    done = true;
                };
                newStoryboard.Begin(newContent, true);
            }
            else
            {
                EndTransition(transitionElement, oldContent, newContent);
            }
        }

        /// <inheritdoc />
        protected override void OnTransitionEnded(TransitionElement transitionElement, ContentPresenter oldContent,
            ContentPresenter newContent)
        {
            var oldStoryboard = (Storyboard) oldContent.GetValue(OldContentStoryboardProperty);
            oldStoryboard?.Stop(oldContent);
            oldContent.ClearValue(FrameworkElement.StyleProperty);

            var newStoryboard = (Storyboard) newContent.GetValue(NewContentStoryboardProperty);
            newStoryboard?.Stop(newContent);
            newContent.ClearValue(FrameworkElement.StyleProperty);
        }
    }
}