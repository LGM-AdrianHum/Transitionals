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
using System.Windows;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Diagnostics;

namespace Transitionals
{
    /// <inheritdoc />
    /// <summary>
    /// A transition selector that randomly selects from a list of available transitions.
    /// </summary>
    [System.Windows.Markup.ContentProperty("Transitions")]
    public class RandomTransitionSelector : TransitionSelector
    {
        #region Static Version
        #region Constants
        /************************************************
		 * Dependency Properties
		 ***********************************************/
        /// <summary>
        /// Identifies the <see cref="TransitionDurationProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TransitionDurationProperty = DependencyProperty.Register("TransitionDurationProperty", typeof(Duration), typeof(RandomTransitionSelector), new FrameworkPropertyMetadata(Duration.Automatic, OnTransitionDurationChanged));

        /// <summary>
        /// Identifies the <see cref="TransitionAssembliesProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TransitionAssembliesProperty = DependencyProperty.Register("TransitionAssembliesProperty", typeof(ObservableCollection<AssemblyName>), typeof(RandomTransitionSelector), new FrameworkPropertyMetadata(new ObservableCollection<AssemblyName>(), FrameworkPropertyMetadataOptions.AffectsRender, OnTransitionAssembliesChanged));

        /// <summary>
        /// Identifies the <see cref="TransitionsProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TransitionsProperty = DependencyProperty.Register("TransitionsProperty", typeof(ObservableCollection<Transition>), typeof(RandomTransitionSelector), new FrameworkPropertyMetadata(new ObservableCollection<Transition>(), FrameworkPropertyMetadataOptions.AffectsRender, OnTransitionsChanged));
        #endregion // Constants

        #region Overrides / Event Handlers
        /************************************************
		 * Overrides / Event Handlers
		 ***********************************************/
        private static void OnTransitionDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RandomTransitionSelector)d).OnTransitionDurationChanged(e);
        }

        private static void OnTransitionAssembliesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var selector = ((RandomTransitionSelector)d);
            selector.ResetAssembliesResolved();
            selector.OnTransitionAssembliesChanged(e);
        }

        private static void OnTransitionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RandomTransitionSelector)d).OnTransitionsChanged(e);
        }
        #endregion // Overrides / Event Triggers
        #endregion // Static Version

        #region Instance Version
        #region Member Variables
        /************************************************
		 * Member Variables
		 ***********************************************/
        private bool _assembliesResolved;
        private readonly List<Transition> _assemblyTransitions = new List<Transition>();
        private readonly Random _random = new Random();
        #endregion // Member Variables

        #region Internal Methods
        /************************************************
		 * Internal Methods
		 ***********************************************/
        /// <summary>
        /// Sets the assemblies list to unresolved.
        /// </summary>
        private void ResetAssembliesResolved()
        {
            // Clone current list of transitions
            var explicitTransitions = new List<Transition>(Transitions);

            // Remove transitions that were added from assemblies
            if (_assemblyTransitions.Count > 0)
            {
                explicitTransitions.RemoveAll(t => _assemblyTransitions.Contains(t));
            }

            // Clear list of transitions added by assemblies
            _assemblyTransitions.Clear();

            // Set the list of transitions to whatever's left over.
            // This will be the list of all transitions that weren't loaded 
            // automatically from assemblies.
            Transitions = new ObservableCollection<Transition>(explicitTransitions);
            
            // Mark assemblies as unresolved.
            _assembliesResolved = false;
        }

        /// <summary>
        /// Resolves assembly names to transitions.
        /// </summary>
        private void ResolveAssemblies()
        {
            // Get list of existing transitions
            var existingTransitions = Transitions.ToList();

            // Placeholder for final list of transitions (explicit + loaded)
            var finalTransitions = new List<Transition>(existingTransitions);

            // For each assembly name
            foreach (var asmName in TransitionAssemblies)
            {
                // Loading an assembly may fail and we don't want to skip 
                // loading additional assemblies.
                try
                {
                    // Load the assembly from the name
                    var asm = Assembly.Load(asmName);

                    // Examine all types
                    foreach (var transitionType in asm.GetTypes())
                    {
                        // Make sure type can be instantiated as a transition
                        if (!transitionType.IsCreatableAs<Transition>()) continue;
                        
                        // If there's already an existing transition of the same type don't add it again.
                        // This is so that non-default settings can be specified for certain transitions
                        // but other ones can take their default values.
                        if (existingTransitions.ContainsType(transitionType))
                        {
                            continue;
                        }

                        // Creating an instance of a transition may fail. We don't 
                        // want to skip loading other transitions.
                        try
                        {
                            // Create
                            var transition = (Transition)Activator.CreateInstance(transitionType);

                            // Add to the final list
                            finalTransitions.Add(transition);

                            // Add to list that indicates it was loaded from an assembly
                            _assemblyTransitions.Add(transition);
                        }
                        catch (Exception ex)
                        {
                            if (!TraceSwitches.Transitions.TraceVerbose) continue;
                           
                            Debug.WriteLine($"Error loading transition '{transitionType.Name}'.\r\n\r\n{ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (!TraceSwitches.Transitions.TraceVerbose) continue;
                    Debug.WriteLine($"Error loading transition assembly '{asmName.Name}'.\r\n\r\n{ex.Message}");
                }
            }

            // Set final list as current list
            Transitions = new ObservableCollection<Transition>(finalTransitions);

            // Assemblies are resolved
            _assembliesResolved = true;
        }
        #endregion // Internal Methods

        #region Overrides / Event Handlers
        /************************************************
		 * Overrides / Event Handlers
		 ***********************************************/
        /// <inheritdoc />
        /// <summary>
        /// Returns a random <see cref="T:Transitionals.Transition" /> from the list of <see cref="P:Transitionals.RandomTransitionSelector.Transitions" />.
        /// </summary>
        /// <param name="oldContent">
        /// The old content that is currently displayed.
        /// </param>
        /// <param name="newContent">
        /// The new content that is to be displayed.
        /// </param>
        /// <returns>
        /// The transition used to display the content or <see langword="null" /> if a 
        /// transition should not be used.
        /// </returns>
        public override Transition SelectTransition(object oldContent, object newContent)
        {
            // If assemblies haven't been resolved, resolve them
            if (!_assembliesResolved)
            {
                ResolveAssemblies();
            }

            // Get transition collection
            var transitions = Transitions;

            // Placeholder for the transition
            Transition transition = null;

            // Only proceed if there are some to select from
            if ((transitions != null) && (transitions.Count > 0))
            {
                // Select
                transition = transitions[_random.Next(0, transitions.Count)];
            }

            // Get duration
            var transitionDuration = TransitionDuration;

            // Update duration?
            if ((transition == null) || (transitionDuration == Duration.Automatic)) return transition;
            // TODO: Fix transitions that throw an exception here.

            // This catch is because back in Acropolis when the transitions were first 
            // implemented, some transitions did not (and currently still do not) support 
            // variable durations. The current implementation is to throw an exception, 
            // which the block below will ignore.
            try
            {
                transition.Duration = transitionDuration;
            }
            catch
            {
                // ignored
            }

            // Return the transition (or lack thereof)
            return transition;
        }

        /// <summary>
        /// Occurs when the value of the <see cref="TransitionDuration"/> property has changed.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> containing event information.
        /// </param>
        protected virtual void OnTransitionDurationChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// Occurs when the value of the <see cref="TransitionAssemblies"/> property has changed.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> containing event information.
        /// </param>
        protected virtual void OnTransitionAssembliesChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// Occurs when the value of the <see cref="Transitions"/> property has changed.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DependencyPropertyChangedEventArgs"/> containing event information.
        /// </param>
        protected virtual void OnTransitionsChanged(DependencyPropertyChangedEventArgs e)
        {

        }
        #endregion // Overridables / Event Triggers

        #region Public Properties
        /************************************************
		 * Public Properties
		 ***********************************************/
        /// <summary>
        /// Gets or sets a <see cref="Duration"/> that all transitions will last. This is a dependency property.
        /// </summary>
        /// <value>
        /// The <see cref="Duration"/> that all transitions will last, or <see cref="Duration.Automatic"/> 
        /// to allow each transition to have it's own unique duration. The default value is 
        /// <see cref="Duration.Automatic"/>.
        /// </value>
        /// <remarks>
        /// Each transition provides its own default duration even if one isn't specified in markup. 
        /// Setting this property to <see cref="Duration.Automatic"/> (the default) will honor these 
        /// defaults or any durations specified in markup. If a value other than 
        /// <see cref="Duration.Automatic"/> is supplied, the specified duration will 
        /// replace the duration for each <see cref="Transition"/> in the <see cref="Transitions"/> 
        /// list as it is used.
        /// </remarks>
        public Duration TransitionDuration
        {
            get
            {
                return (Duration)GetValue(TransitionDurationProperty);
            }
            set
            {
                SetValue(TransitionDurationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a list of assembly names that define assemblies containing transitions. 
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// A list of assembly names that define assemblies containing transitions.
        /// </value>
        public ObservableCollection<AssemblyName> TransitionAssemblies
        {
            get
            {
                return (ObservableCollection<AssemblyName>)GetValue(TransitionAssembliesProperty);
            }
            set
            {
                SetValue(TransitionAssembliesProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the list of transitions that will be used by the <see cref="RandomTransitionSelector"/>. 
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// The list of transitions that will be used by the <see cref="RandomTransitionSelector"/>.
        /// </value>
        public ObservableCollection<Transition> Transitions
        {
            get
            {
                return (ObservableCollection<Transition>)GetValue(TransitionsProperty);
            }
            set
            {
                SetValue(TransitionsProperty, value);
            }
        }
        #endregion // Public Properties
        #endregion // Instance Version
    }
}
