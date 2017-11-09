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
using System.Collections.ObjectModel;
using System.Reflection;
using Transitionals;

namespace TransitionTester
{
	public partial class App: System.Windows.Application
	{
        #region Static Version
        /// <summary>
        /// Gets the typed instance of the current application.
        /// </summary>
        public static App CurrentApp
        {
            get
            {
                return (App)Current;
            }
        }
        #endregion // Static Version

        #region Instance Version
        #region Member Variables
        /************************************************
		 * Member Variables
		 ***********************************************/
        private PropertyWindow propertyWindow = new PropertyWindow();
        private ObservableCollection<Type> transitionTypes = new ObservableCollection<Type>();
        #endregion // Member Variables

        #region Overrides / Event Handlers
        /************************************************
		 * Overrides / Event Handlers
		 ***********************************************/
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            // Can't assign the owner to the property window unless it has been created.
            if (propertyWindow.Owner == null)
            {
                propertyWindow.Owner = MainWindow;
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Load transitions
            LoadStockTransitions();
        }
        #endregion // Overrides / Event Handlers

        #region Internal Methods
        /************************************************
		 * Internal Methods
		 ***********************************************/
        /// <summary>
        /// Loads the transitions that are part of the framework.
        /// </summary>
        private void LoadStockTransitions()
        {
            LoadTransitions(Assembly.GetAssembly(typeof(Transition)));
        }
        #endregion // Internal Methods

        #region Public Methods
        /************************************************
		 * Public Methods
		 ***********************************************/
        /// <summary>
        /// Loads the transitions found in the specified assembly.
        /// </summary>
        /// <param name="assembly">
        /// The assembly to search for transitions in.
        /// </param>
        public void LoadTransitions(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                // Must not already exist
                if (transitionTypes.Contains(type)) { continue; }

                // Must not be abstract.
                if ((typeof(Transition).IsAssignableFrom(type)) && (!type.IsAbstract))
                {
                    transitionTypes.Add(type);
                }
            }
        }

        /// <summary>
        /// Loads the transitions found in the assembly at the specified path.
        /// </summary>
        /// <param name="assemblyPath">
        /// The path to the assembly to search for transitions in.
        /// </param>
        public void LoadTransitions(string assemblyPath)
        {
            // Load the assembly
            Assembly assembly = Assembly.LoadFrom(assemblyPath);

            // Load transitions from the assembly
            LoadTransitions(assembly);
        }

        /// <summary>
        /// Toggles the visibility of the property window.
        /// </summary>
        public void TogglePropertyWindow()
        {
            if (propertyWindow.IsVisible)
            {
                propertyWindow.Hide();
            }
            else
            {
                propertyWindow.Show();
            }
        }
        #endregion // Public Methods

        #region Public Properties
        /************************************************
		 * Public Properties
		 ***********************************************/
        /// <summary>
        /// Gets the <see cref="PropertyWindow"/> used by the application.
        /// </summary>
        /// <value>
        /// The <see cref="PropertyWindow"/> used by the application.
        /// </value>
        public PropertyWindow PropertyWindow
        {
            get
            {
                return propertyWindow;
            }
        }

        /// <summary>
        /// Gets the list of known transition types.
        /// </summary>
        /// <value>
        /// The list of known transition types.
        /// </value>
        public ObservableCollection<Type> TransitionTypes
        {
            get
            {
                return transitionTypes;
            }
        }
        #endregion // Public Properties
        #endregion // Instance Version

    }
}
