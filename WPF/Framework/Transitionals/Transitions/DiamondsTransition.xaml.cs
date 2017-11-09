#region License Revision: 0 Last Revised: 3/29/2006 8:21 AM
/******************************************************************************
Copyright (c) Microsoft Corporation.  All rights reserved.


This file is licensed under the Microsoft Public License (Ms-PL). A copy of the Ms-PL should accompany this file. 
If it does not, you can obtain a copy from: 

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx
******************************************************************************/
#endregion // License
using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Runtime.InteropServices;

namespace Transitionals.Transitions
{
    /// <summary>
    ///     Stores the XAML that defines the DiamondsTransition
    /// </summary>
    [ComVisible(false)]
    public partial class DiamondsTransitionFrameworkElement : FrameworkElement
    {
        public DiamondsTransitionFrameworkElement()
        {
            InitializeComponent();
        }
    }

    /// <summary>
    ///     Represents the DiamondsTransition
    /// </summary>
    [ComVisible(false)]
    public class DiamondsTransition : StoryboardTransition
    {
        private static DiamondsTransitionFrameworkElement frameworkElement = new DiamondsTransitionFrameworkElement();

        public DiamondsTransition()
        {
            this.NewContentStyle = (Style)frameworkElement.FindResource("DiamondsTransitionNewContentStyle");
            this.NewContentStoryboard = (Storyboard)frameworkElement.FindResource("DiamondsTransitionNewContentStoryboard");
            this.Duration = new Duration(TimeSpan.FromSeconds(0.5));
        }

        protected override void OnDurationChanged(Duration oldDuration, Duration newDuration)
        {
            if (this.NewContentStoryboard != null && this.NewContentStoryboard.Children.Count > 0)
            {
                this.NewContentStoryboard.Children[0].Duration = newDuration;
            }
        }
    }
}
