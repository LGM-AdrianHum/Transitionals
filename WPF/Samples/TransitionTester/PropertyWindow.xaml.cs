#region License Revision: 0 Last Revised: 3/29/2006 8:21 AM
/******************************************************************************
Copyright (c) Microsoft Corporation.  All rights reserved.


This file is licensed under the Microsoft Public License (Ms-PL). A copy of the Ms-PL should accompany this file. 
If it does not, you can obtain a copy from: 

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx
******************************************************************************/
#endregion // License

using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Threading;
using System.Threading;

namespace TransitionTester
{
    /// <summary>
    /// Interaction logic for PropertyWindow.xaml
    /// </summary>
    public partial class PropertyWindow : System.Windows.Window
    {
        private object selectedObject;

        public PropertyWindow()
        {
            InitializeComponent();
        }

        private object CreateFilterWrapper(object value)
        {
            // Don't wrap null values
            if (value == null) { return null; }

            // Use the filter
            return new TransitionFilterDescriptor(value, (TransitionFilterLevel)FilterCombo.SelectedIndex);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // Don't let close, hide instead
            e.Cancel = true;
            
            // hideTimer.Start();
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(Hide));
        }

        /// <summary>
        /// Gets or sets the selected object of the <see cref="PropertyWindow"/>.
        /// </summary>
        /// <value>
        /// The selected object of the <c>PropertyWindow</c>.
        /// </value>
        public object SelectedObject
        {
            get
            {
                return selectedObject;
            }
            set
            {
                // Store in local variable
                selectedObject = value;

                // Create the wrapper and assign it to the grid
                propertyGrid.SelectedObject = CreateFilterWrapper(value);
            }
        }

        private void FilterChanged(object sender, SelectionChangedEventArgs e)
        {
            // Recreate the wrapper
            if (propertyGrid != null)
            {
                propertyGrid.SelectedObject = CreateFilterWrapper(selectedObject);
            }
        }
    }
}
