//  _____                     _ _   _                   _     
// /__   \_ __ __ _ _ __  ___(_) |_(_) ___  _ __   __ _| |___ 
//   / /\/ '__/ _` | '_ \/ __| | __| |/ _ \| '_ \ / _` | / __|
//  / /  | | | (_| | | | \__ \ | |_| | (_) | | | | (_| | \__ \
//  \/   |_|  \__,_|_| |_|___/_|\__|_|\___/|_| |_|\__,_|_|___/
//                                                            
// Module   : Transitionals/Transitionals/TransitionEventArgs.cs
// Name     : Adrian Hum - adrianhum 
// Created  : 2017-09-23-11:00 AM
// Modified : 2017-11-10-7:46 AM

using System;
using System.Windows;

namespace Transitionals.Controls
{
    /// <summary>
    ///     Provides data for events involving a transition.
    /// </summary>
    public class TransitionEventArgs : RoutedEventArgs
    {
        /// <summary>
        ///     Initializes a new <see cref="TransitionEventArgs" /> instance.
        /// </summary>
        /// <param name="routedEvent">
        ///     The routed event this data represents.
        /// </param>
        /// <param name="source">
        ///     The source of the event.
        /// </param>
        /// <param name="transition">
        ///     The transition involved in the event.
        /// </param>
        /// <param name="oldContent">
        ///     The old content involved in the event.
        /// </param>
        /// <param name="newContent">
        ///     The new content involved in the event.
        /// </param>
        public TransitionEventArgs(RoutedEvent routedEvent, object source, Transition transition, object oldContent,
            object newContent) : base(routedEvent, source)
        {
            // Validate
            if (transition == null) throw new ArgumentNullException("transition");

            // Store
            Transition = transition;
            OldContent = oldContent;
            NewContent = newContent;
        }

        /// <summary>
        ///     Gets the new content involved in the event.
        /// </summary>
        /// <value>
        ///     The new content involved in the event.
        /// </value>
        public object NewContent { get; }

        /// <summary>
        ///     Gets the old content involved in the event.
        /// </summary>
        /// <value>
        ///     The old content involved in the event.
        /// </value>
        public object OldContent { get; }

        /// <summary>
        ///     Gets the transition involved in the event.
        /// </summary>
        /// <value>
        ///     The transition involved in the event.
        /// </value>
        public Transition Transition { get; }
    }

    /// <summary>
    ///     The signature for a handler of events involving transitions.
    /// </summary>
    /// <param name="sender">
    ///     The sender of the event.
    /// </param>
    /// <param name="e">
    ///     A <see cref="TransitionEventArgs" /> containing the event data.
    /// </param>
    public delegate void TransitionEventHandler(object sender, TransitionEventArgs e);
}