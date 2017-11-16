#region License Revision: 0 Last Revised: 3/29/2006 8:21 AM
/******************************************************************************
Copyright (c) Microsoft Corporation.  All rights reserved.


This file is licensed under the Microsoft Public License (Ms-PL). A copy of the Ms-PL should accompany this file. 
If it does not, you can obtain a copy from: 

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx
******************************************************************************/
#endregion // License
using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Threading;

namespace ImageViewer
{
    /// <summary>
    /// Interaction logic for Viewer.xaml
    /// </summary>
    public partial class Viewer : Window
    {
        private bool initialBrowseComplete;

        public Viewer()
        {
            InitializeComponent();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            // After window is shown, wait for idle then load images
            if (!initialBrowseComplete)
            {
                initialBrowseComplete = true;
                Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new Func<bool>(BrowsForImages));
            }
        }

        public bool BrowsForImages()
        {
            try
            {
                // Create the browser
                System.Windows.Forms.FolderBrowserDialog fb =
                    new System.Windows.Forms.FolderBrowserDialog
                    {
                        RootFolder = @"",
                        Description =
                            "Please select a folder that contains images to show.\r\nNote that very large images curently do not transition well on some machines."
                    };

                // Start in 'My Pictures'

                // Set a nice title

                // Show
                if (fb.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // Get all the images
                    SelectImages(fb.SelectedPath);

                    // Success
                    return true;
                }
                else
                {
                    // Failure
                    return false;
                }
            }
            catch (Exception ex)
            {
                string msg = "Error loading images:\r\n\r\n{0}";
                MessageBox.Show(string.Format(msg, ex.Message));
                return false;
            }
        }

        public void SelectImages(string path)
        {
            // Validate
            if (string.IsNullOrEmpty(path)) throw new ArgumentException("path");

            // Place for results
            ObservableCollection<FileInfo> results = new ObservableCollection<FileInfo>();

            // Get directory info for specified path
            DirectoryInfo di = new DirectoryInfo(path);

            // Image mask
            string[] extensions = new string[] { "*.jpg", "*.png", "*.gif" };

            // Search for all
            foreach (string extension in extensions)
            {
                foreach (FileInfo fi in di.GetFiles(extension))
                {
                    results.Add(fi);
                }
            }

            // Sort by date taken (last modified)
            results.OrderBy(f => f.LastWriteTime);

            // Store the results
            ImagesSource.ObjectType = null;
            ImagesSource.ObjectInstance = results;
        }

        /// <summary>
        /// The data provider used to provide images.
        /// </summary>
        public ObjectDataProvider ImagesSource
        {
            get
            {
                return (ObjectDataProvider)Resources["ImagesSource"];
            }
        }
    }
}
