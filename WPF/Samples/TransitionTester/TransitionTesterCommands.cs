#region License Revision: 0 Last Revised: 3/29/2006 8:21 AM
/******************************************************************************
Copyright (c) Microsoft Corporation.  All rights reserved.


This file is licensed under the Microsoft Public License (Ms-PL). A copy of the Ms-PL should accompany this file. 
If it does not, you can obtain a copy from: 

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx
******************************************************************************/
#endregion // License
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace TransitionTester
{
    /// <summary>
    /// Commands for the TransitionTester application not provided by the framework.
    /// </summary>
    static public class TransitionTesterCommands
    {
        private static RoutedUICommand about = null;
        private static RoutedUICommand exit = null;

        /// <summary>
        /// Provides a routed command for application about.
        /// </summary>
        public static RoutedUICommand About
        {
            get
            {
                if (about == null)
                {
                    about = new RoutedUICommand("About", "About", typeof(TransitionTesterCommands));
                }
                return about;
            }
        }

        /// <summary>
        /// Provides a routed command for application exit.
        /// </summary>
        public static RoutedUICommand Exit
        {
            get
            {
                if (exit == null)
                {
                    exit = new RoutedUICommand("Exit", "Exit", typeof(TransitionTesterCommands));
                    exit.InputGestures.Add(new KeyGesture(Key.F4, ModifierKeys.Alt));
                }
                return exit;
            }
        }
    }
}
