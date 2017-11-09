//  _____                     _ _   _                   _     
// /__   \_ __ __ _ _ __  ___(_) |_(_) ___  _ __   __ _| |___ 
//   / /\/ '__/ _` | '_ \/ __| | __| |/ _ \| '_ \ / _` | / __|
//  / /  | | | (_| | | | \__ \ | |_| | (_) | | | | (_| | \__ \
//  \/   |_|  \__,_|_| |_|___/_|\__|_|\___/|_| |_|\__,_|_|___/
//                                                            
// Module   : Transitionals/Transitionals/TransitionConverter.cs
// Name     : Adrian Hum - adrianhum 
// Created  : 2017-09-23-11:00 AM
// Modified : 2017-11-10-7:25 AM

using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace Transitionals
{
    /// <inheritdoc />
    /// <summary>
    ///     TypeConverter to convert Transition to/from other types.
    ///     Currently only <see cref="T:System.String" /> is supported.
    /// </summary>
    public class TransitionConverter : TypeConverter
    {
/*
        /// <summary>
        /// Cached value for GetStandardValues
        /// </summary>
        private static StandardValuesCollection _standardValues;
*/

        /// <inheritdoc />
        /// <summary>
        ///     TypeConverter method override.
        /// </summary>
        /// <param name="context">ITypeDescriptorContext</param>
        /// <param name="sourceType">Type to convert from</param>
        /// <returns>true if conversion is possible</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == null)
                throw new ArgumentNullException("sourceType");
            // We can only handle strings
            return sourceType == typeof(string);
        }

        /// <inheritdoc />
        /// <summary>
        ///     TypeConverter method override.
        /// </summary>
        /// <param name="context">ITypeDescriptorContext</param>
        /// <param name="destinationType">Type to convert to</param>
        /// <returns>true if conversion is possible</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == null)
                throw new ArgumentNullException("destinationType");
            // We can convert to an InstanceDescriptor or to a string.
            return destinationType == typeof(InstanceDescriptor) ||
                   destinationType == typeof(string);
        }

        /// <inheritdoc />
        /// <summary>
        ///     TypeConverter method implementation.
        /// </summary>
        /// <param name="context">ITypeDescriptorContext</param>
        /// <param name="culture">Current culture (see CLR specs)</param>
        /// <param name="value">value to convert from</param>
        /// <returns>value that is result of conversion</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            // Try to get the value as a string
            var strValue = value as string;

            // Only continue if valid string
            if (string.IsNullOrEmpty(strValue)) return base.ConvertFrom(context, culture, value);
            // Try to get the type
            var transType = Type.GetType(strValue);

            // Only continue if we got a valid type
            if (transType != null && typeof(Transition).IsAssignableFrom(transType))
                return Activator.CreateInstance(transType);

            // Not found. Try default base conversion.
            return base.ConvertFrom(context, culture, value);
        }
    }
}