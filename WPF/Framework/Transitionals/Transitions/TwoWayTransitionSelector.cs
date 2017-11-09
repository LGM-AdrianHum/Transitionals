//  _____                     _ _   _                   _     
// /__   \_ __ __ _ _ __  ___(_) |_(_) ___  _ __   __ _| |___ 
//   / /\/ '__/ _` | '_ \/ __| | __| |/ _ \| '_ \ / _` | / __|
//  / /  | | | (_| | | | \__ \ | |_| | (_) | | | | (_| | \__ \
//  \/   |_|  \__,_|_| |_|___/_|\__|_|\___/|_| |_|\__,_|_|___/
//                                                            
// Module   : Transitionals/Transitionals/TwoWayTransitionSelector.cs
// Name     : Adrian Hum - adrianhum 
// Created  : 2017-09-23-11:00 AM
// Modified : 2017-11-10-7:46 AM

using System.Runtime.InteropServices;
using System.Windows;

namespace Transitionals.Transitions
{
    /// <summary>
    /// </summary>
    public enum TransitionDirection
    {
        /// <summary>
        /// </summary>
        Forward,

        /// <summary>
        /// </summary>
        Backward
    }

    //Choose between a forward and backward transition based on the Direction property
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    [ComVisible(false)]
    public class TwoWayTransitionSelector : TransitionSelector
    {
        /// <summary>
        /// </summary>
        public static readonly DependencyProperty ForwardTransitionProperty =
            DependencyProperty.Register("ForwardTransition", typeof(Transition), typeof(TwoWayTransitionSelector),
                new UIPropertyMetadata(null));

        /// <summary>
        /// </summary>
        public static readonly DependencyProperty BackwardTransitionProperty =
            DependencyProperty.Register("BackwardTransition", typeof(Transition), typeof(TwoWayTransitionSelector),
                new UIPropertyMetadata(null));

        /// <summary>
        /// </summary>
        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(TransitionDirection), typeof(TwoWayTransitionSelector),
                new UIPropertyMetadata(TransitionDirection.Forward));

        /// <summary>
        /// </summary>
        public Transition ForwardTransition
        {
            get { return (Transition) GetValue(ForwardTransitionProperty); }
            set { SetValue(ForwardTransitionProperty, value); }
        }

        /// <summary>
        /// </summary>
        public Transition BackwardTransition
        {
            get { return (Transition) GetValue(BackwardTransitionProperty); }
            set { SetValue(BackwardTransitionProperty, value); }
        }


        /// <summary>
        /// </summary>
        public TransitionDirection Direction
        {
            get { return (TransitionDirection) GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }


        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="oldContent"></param>
        /// <param name="newContent"></param>
        /// <returns></returns>
        public override Transition SelectTransition(object oldContent, object newContent)
        {
            return Direction == TransitionDirection.Forward ? ForwardTransition : BackwardTransition;
        }
    }
}