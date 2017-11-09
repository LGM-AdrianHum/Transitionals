//  _____                     _ _   _                   _     
// /__   \_ __ __ _ _ __  ___(_) |_(_) ___  _ __   __ _| |___ 
//   / /\/ '__/ _` | '_ \/ __| | __| |/ _ \| '_ \ / _` | / __|
//  / /  | | | (_| | | | \__ \ | |_| | (_) | | | | (_| | \__ \
//  \/   |_|  \__,_|_| |_|___/_|\__|_|\___/|_| |_|\__,_|_|___/
//                                                            
// Module   : Transitionals/Transitionals/TransitionSelector.cs
// Name     : Adrian Hum - adrianhum 
// Created  : 2017-09-23-11:00 AM
// Modified : 2017-11-10-7:25 AM

using System.Runtime.InteropServices;
using System.Windows;

namespace Transitionals
{
    /// <inheritdoc />
    /// <summary>
    ///     Allows different transitions to run based on the old and new contents.
    /// </summary>
    [ComVisible(false)]
    public abstract class TransitionSelector : DependencyObject
    {
        /// <summary>
        ///     When overridden in a derived class, returns a <see cref="Transition" /> based on custom logic.
        /// </summary>
        /// <param name="oldContent">
        ///     The old content that is currently displayed.
        /// </param>
        /// <param name="newContent">
        ///     The new content that is to be displayed.
        /// </param>
        /// <returns>
        ///     The transition used to display the content or <see langword="null" /> if a
        ///     transition should not be used.
        /// </returns>
        public virtual Transition SelectTransition(object oldContent, object newContent)
        {
            return null;
        }
    }
}