#region License Revision: 0 Last Revised: 3/29/2006 8:21 AM
/******************************************************************************
Copyright (c) Microsoft Corporation.  All rights reserved.


This file is licensed under the Microsoft Public License (Ms-PL). A copy of the Ms-PL should accompany this file. 
If it does not, you can obtain a copy from: 

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx
******************************************************************************/
#endregion // License
using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Markup;
using System.Xml;
using System.Reflection;
using Transitionals;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Win32;

namespace TransitionTester
{
	public partial class TransitionWindow
	{
        #region Constants
        /************************************************
		 * Constants
		 ***********************************************/
        private const string CellA = "CellA";
        private const string CellB = "CellB";
        #endregion // Constants

        #region Member Variables
        /************************************************
		 * Member Variables
		 ***********************************************/
        private ICollectionView view;
        #endregion // Member Variables

        #region Constructors
        /************************************************
		 * Constructors
		 ***********************************************/
        /// <summary>
        /// Initializes a new <see cref="TransitionWindow"/>.
        /// </summary>
        public TransitionWindow()
        {
            this.InitializeComponent();

            // Get the default view for the transition types
            view = CollectionViewSource.GetDefaultView(App.CurrentApp.TransitionTypes);
            
            // Set the default sort
            view.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            
            // Handle changes in currency
            view.CurrentChanged += new EventHandler(view_CurrentChanged);

            // Bind
            TransitionDS.ObjectType = null;
            TransitionTypesDS.ObjectType = null;
            TransitionTypesDS.ObjectInstance = App.CurrentApp.TransitionTypes;

            // Navigate to first item
            view.MoveCurrentToFirst();
        }
        #endregion // Constructors

        #region Internal Methods
        /************************************************
		 * Internal Methods
		 ***********************************************/
        /// <summary>
        /// Activates a transition and displays it.
        /// </summary>
        /// <param name="transitionType">
        /// The type of transition to activate.
        /// </param>
        private void ActivateTransition(Type transitionType)
        {
            // If no type, ignore
            if (transitionType == null) return;

            // Create the instance
            Transition transition = (Transition)Activator.CreateInstance(transitionType);

            // Bind
            TransitionDS.ObjectInstance = transition;
            App.CurrentApp.PropertyWindow.SelectedObject = transition;

            // Swap cells to show transition
            SwapCell();
        }

        /// <summary>
        /// Loads transitions by allowing the user to browse for a transition assembly.
        /// </summary>
        private void BrowseLoadTransitions()
        {
            // Create the browser
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Assemblies (*.dll, *.exe)|*.dll;*.exe|All files (*.*)|*.*";
            ofd.Multiselect = true;

            // Show the browse and if successful, try to load.
            if (ofd.ShowDialog(this) == true)
            {
                // Try to load each selected file
                foreach (string path in ofd.FileNames)
                {
                    try
                    {
                        App.CurrentApp.LoadTransitions(path);
                    }
                    catch (Exception ex)
                    {
                        // Build the message
                        string msg = string.Format("Error loading transitions:\r\n\r\n{0}\r\n\r\nContinue?", ex.Message);

                        // Show message and ask to continue
                        MessageBoxResult result = MessageBox.Show(msg, "Error", MessageBoxButton.YesNo, MessageBoxImage.Error);

                        // If we shouldn't continue, break out of loop
                        if (result != MessageBoxResult.Yes) { break; }
                    }
                }
            }
        }

        /// <summary>
        /// Creates an animation cell for demonstrating a transition.
        /// </summary>
        /// <param name="style">
        /// The style used to create the cell.
        /// </param>
        /// <returns>
        /// A <see cref="ContentControl"/> that represents the cell.
        /// </returns>
        private ContentControl CreateCell(Style style)
        {
            ContentControl c = new ContentControl();
            c.Style = style;
            return c;
        }

        /// <summary>
        /// Displays the About dialog.
        /// </summary>
        private void ShowAbout()
        {
            AboutWindow about = new AboutWindow();
            about.Owner = this;
            about.ShowDialog();
        }

        /// <summary>
        /// Swaps the current cell, from A to B or from B to A.
        /// </summary>
        private void SwapCell()
        {
            ContentControl currentCell = (ContentControl)TransitionBox.Content;
            if ((currentCell == null) || (currentCell.Style == Resources[CellB]))
            {
                TransitionBox.Content = CreateCell((Style)Resources[CellA]);
            }
            else
            {
                TransitionBox.Content = CreateCell((Style)Resources[CellB]);
            }
        }
        #endregion // Internal Methods

        #region Overrides / Event Handlers
        /************************************************
		 * Overrides / Event Handlers
		 ***********************************************/
        private void About_Executed(object sender, RoutedEventArgs e)
        {
            ShowAbout();    
        }

        private void ABButton_Click(object sender, RoutedEventArgs e)
        {
            SwapCell();
        }

        private void AButton_Click(object sender, RoutedEventArgs e)
        {
            TransitionBox.Content = CreateCell((Style)Resources[CellA]);
        }

        private void BButton_Click(object sender, RoutedEventArgs e)
        {
            TransitionBox.Content = CreateCell((Style)Resources[CellB]);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            TransitionBox.Content = null;
        }

        private void Exit_Executed(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Open_Executed(object sender, RoutedEventArgs e)
        {
            BrowseLoadTransitions();
        }

        private void PropertyButton_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentApp.TogglePropertyWindow();
        }

        private void view_CurrentChanged(object sender, EventArgs e)
        {
            ActivateTransition((Type)view.CurrentItem);
        }
        #endregion // Overrides / Event Handlers

        #region Internal Properties
        /************************************************
		 * Internal Properties
		 ***********************************************/
        private ObjectDataProvider TransitionDS
        {
            get
            {
                return (ObjectDataProvider)Resources["TransitionDS"];
            }
        }

        private ObjectDataProvider TransitionTypesDS
        {
            get
            {
                return (ObjectDataProvider)Resources["TransitionTypesDS"];
            }
        }
        #endregion // Internal Properties
    }
}