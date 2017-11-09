//  _____                     _ _   _                   _     
// /__   \_ __ __ _ _ __  ___(_) |_(_) ___  _ __   __ _| |___ 
//   / /\/ '__/ _` | '_ \/ __| | __| |/ _ \| '_ \ / _` | / __|
//  / /  | | | (_| | | | \__ \ | |_| | (_) | | | | (_| | \__ \
//  \/   |_|  \__,_|_| |_|___/_|\__|_|\___/|_| |_|\__,_|_|___/
//                                                            
// Module   : Transitionals/Transitionals/InternalExtensions.cs
// Name     : Adrian Hum - adrianhum 
// Created  : 2017-09-23-11:00 AM
// Modified : 2017-11-09-2:46 PM

using System;
using System.Collections;

namespace Transitionals
{
	internal static class InternalExtensions
	{
	    /// <summary>
	    ///     Determines if an item of the specified type exists within the collection.
	    /// </summary>
	    /// <param name="list"></param>
	    /// <param name="type">
	    ///     The type to search for.
	    /// </param>
	    /// <returns>
	    ///     <c>true</c> if an item of the specified type is found; otherwise <c>false</c>.
	    /// </returns>
	    public static bool ContainsType(this IList list, Type type)
		{
			// If no items, skip
			if (list == null) return false;

			// Search each item
			foreach (var compare in list)
				if (compare.GetType() == type)
					return true;

			// Not found
			return false;
		}

	    /// <summary>
	    ///     Indicates if the type can be created as a specified type.
	    /// </summary>
	    /// <typeparam name="T">
	    ///     The type to be created as.
	    /// </typeparam>
	    /// <param name="type">
	    ///     The type to try and create.
	    /// </param>
	    /// <returns>
	    ///     <c>true</c> if the type can be created as <typeparamref name="T" />.
	    /// </returns>
	    public static bool IsCreatableAs<T>(this Type type)
		{
			// Validate parameters
			if (type == null) throw new ArgumentNullException(nameof(type));

			// Make sure type matches
			if (!typeof(T).IsAssignableFrom(type)) return false;

			// Make sure it's public
			if (type.IsNotPublic) return false;

			// Make sure it's not a generic type
			if (type.IsGenericType) return false;

			// Make sure it's not an interface
			if (type.IsInterface) return false;

			// Make sure it's not abstract
			return !type.IsAbstract;

			// Valid
		}
	}
}