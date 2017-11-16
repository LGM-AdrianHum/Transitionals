#region License Revision: 0 Last Revised: 3/29/2006 8:21 AM
/******************************************************************************
Copyright (c) Microsoft Corporation.  All rights reserved.


This file is licensed under the Microsoft Public License (Ms-PL). A copy of the Ms-PL should accompany this file. 
If it does not, you can obtain a copy from: 

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx
******************************************************************************/
#endregion // License

using System.Windows.Input;

namespace TransitionTester
{
    /// <summary>
    /// Commands for the TransitionTester application not provided by the framework.
    /// </summary>
    public static class TransitionTesterCommands
    {
        private static RoutedUICommand _about;
        private static RoutedUICommand _exit;

        /// <summary>
        /// Provides a routed command for application about.
        /// </summary>
        public static RoutedUICommand About => _about ?? (_about = new RoutedUICommand("About", "About", typeof(TransitionTesterCommands)));

        /// <summary>
        /// Provides a routed command for application exit.
        /// </summary>
        public static RoutedUICommand Exit
        {
            get
            {
                if (_exit != null) return _exit;
                _exit = new RoutedUICommand("Exit", "Exit", typeof(TransitionTesterCommands));
                _exit.InputGestures.Add(new KeyGesture(Key.F4, ModifierKeys.Alt));
                return _exit;
            }
        }
    }
}
