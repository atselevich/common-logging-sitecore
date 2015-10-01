// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Log4NetThreadVariablesContext.cs" company="">
//   
// </copyright>
// <summary>
//   A global context for logger variables
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Common.Logging.Sitecore
{
    using log4net;

    /// <summary>
    ///     A global context for logger variables
    /// </summary>
    public class SitecoreThreadVariablesContext : IVariablesContext
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Clears the global context variables
        /// </summary>
        public void Clear()
        {
            MDC.Clear();
        }

        /// <summary>
        /// Checks if a variable is set within the global context
        /// </summary>
        /// <param name="key">
        /// The key of the variable to check for
        /// </param>
        /// <returns>
        /// True if the variable is set
        /// </returns>
        public bool Contains(string key)
        {
            return MDC.Get(key) != null;
        }

        /// <summary>
        /// Gets the value of a variable within the global context
        /// </summary>
        /// <param name="key">
        /// The key of the variable to get
        /// </param>
        /// <returns>
        /// The value or null if not found
        /// </returns>
        public object Get(string key)
        {
            return MDC.Get(key);
        }

        /// <summary>
        /// Removes a variable from the global context by key
        /// </summary>
        /// <param name="key">
        /// The key of the variable to remove
        /// </param>
        public void Remove(string key)
        {
            MDC.Remove(key);
        }

        /// <summary>
        /// Sets the value of a new or existing variable within the global context
        /// </summary>
        /// <param name="key">
        /// The key of the variable that is to be added
        /// </param>
        /// <param name="value">
        /// The value to add
        /// </param>
        public void Set(string key, object value)
        {
            MDC.Set(key, (string)value);
        }

        #endregion
    }
}