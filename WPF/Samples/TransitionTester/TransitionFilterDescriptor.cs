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
using System.ComponentModel;
using System.Collections.ObjectModel;
using Transitionals;

namespace TransitionTester
{
    /// <summary>
    /// Defines the filter level of properties to be displayed.
    /// </summary>
    public enum TransitionFilterLevel
    {
        /// <summary>
        /// The properties that are defined on the selected transition will be displayed 
        /// along with the properties defined on transition-related base classes (Transition, 
        /// Transition3D and any classes that inherit from them).
        /// </summary>
        TransitionPlusBase,

        /// <summary>
        /// Only properties that are defined on the selected transition will be displayed.
        /// </summary>
        TransitionSpecific,
        
        /// <summary>
        /// All properties will be displayed.
        /// </summary>
        All
    };

    /// <summary>
    /// A custom type descriptor that can be used to show or hide properties in a property grid based 
    /// on a filter level.
    /// </summary>
    public class TransitionFilterDescriptor : CustomTypeDescriptor
    {
        private TransitionFilterLevel filter;
        private object component;

        public TransitionFilterDescriptor(object component, TransitionFilterLevel filter)
        {
            this.component = component;
            this.filter = filter;
        }

        private bool MatchesFilter(PropertyDescriptor property)
        {
            Type componentType = component.GetType();
            DependencyPropertyDescriptor dProperty = DependencyPropertyDescriptor.FromProperty(property);
            
            switch (filter)
            {
                case TransitionFilterLevel.All:
                    return true;
                
                case TransitionFilterLevel.TransitionPlusBase:
                    // If the property isn't defined on a type that inherits from Transition, drop it
                    if (!typeof(Transition).IsAssignableFrom(property.ComponentType)) return false;

                    // If the property is a dependency property, it must be owned by a type that inherits from Transition
                    if ((dProperty != null) && (!typeof(Transition).IsAssignableFrom(dProperty.DependencyProperty.OwnerType))) return false;

                    // All conditions met
                    return true;
                
                case TransitionFilterLevel.TransitionSpecific:
                    // If the property isn't defined exactly on the component, drop it
                    if (property.ComponentType != componentType) return false;

                    // If the property is a dependency property, it must be owned by the component
                    if ((dProperty != null) && (dProperty.DependencyProperty.OwnerType != componentType)) return false;

                    // All conditions met
                    return true;

                default:
                    return false;
            }
        }

        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            // Placeholder
            Collection<PropertyDescriptor> results = new Collection<PropertyDescriptor>();

            // Start with all
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(component, attributes, true);

            // Add filtered
            foreach (PropertyDescriptor property in properties)
            {
                if (MatchesFilter(property)) results.Add(property);
            }
            
            return new PropertyDescriptorCollection(results.ToArray());
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            PropertyDescriptorCollection c = new PropertyDescriptorCollection(new PropertyDescriptor[] { TypeDescriptor.GetProperties(component, true)[0] });
            return c;
        }

        public override AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(component, true);
        }

        public override object GetPropertyOwner(PropertyDescriptor pd)
        {
            return component;
        }

        /// <summary>
        /// Gets or sets the filter of the <see cref="TransitionFilterDescriptor"/>.
        /// </summary>
        /// <value>
        /// The filter of the <c>TransitionFilterDescriptor</c>.
        /// </value>
        public TransitionFilterLevel Filter
        {
            get
            {
                return filter;
            }
            set
            {
                filter = value;
            }
        }

    }
}
