// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="">
//   
// </copyright>
// <summary>
//   The extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace Common.Logging.Sitecore
{
    using System;
    using System.Reflection;

    /// <summary>
    /// The extensions.
    /// </summary>
    public static class Extensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Returns a _private_ Property Value from a given Object. Uses Reflection.
        ///     Throws a ArgumentOutOfRangeException if the Property is not found.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the Property
        /// </typeparam>
        /// <param name="obj">
        /// Object from where the Property Value is returned
        /// </param>
        /// <param name="propName">
        /// Propertyname as string.
        /// </param>
        /// <returns>
        /// PropertyValue
        /// </returns>
        public static T GetPrivatePropertyValue<T>(this object obj, string propName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var pi = obj.GetType()
                .GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (pi == null)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(propName), 
                    $"Property {propName} was not found in Type {obj.GetType().FullName}");
            }

            return (T)pi.GetValue(obj, null);
        }

        #endregion
    }
}