// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Log4NetLogger.cs" company="">
//   
// </copyright>
// <summary>
//   Concrete implementation of <see cref="T:Common.Logging.ILog" /> interface specific to log4net 1.2.9-1.2.11.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Common.Logging.Sitecore
{
    using System;
    using System.Diagnostics;

    using Common.Logging;
    using Common.Logging.Factory;

    using log4net.spi;

    /// <summary>
    ///     Concrete implementation of <see cref="T:Common.Logging.ILog" /> interface specific to log4net 1.2.9-1.2.11.
    /// </summary>
    /// <remarks>
    ///     Log4net is capable of outputting extended debug information about where the current
    ///     message was generated: class name, method name, file, line, etc. Log4net assumes that the location
    ///     information should be gathered relative to where Debug() was called.
    ///     When using Common.Logging, Debug() is called in Common.Logging.Log4Net.Log4NetLogger. This means that
    ///     the location information will indicate that Common.Logging.Log4Net.Log4NetLogger always made
    ///     the call to Debug(). We need to know where Common.Logging.ILog.Debug()
    ///     was called. To do this we need to use the log4net.ILog.Logger.Log method and pass in a Type telling
    ///     log4net where in the stack to begin looking for location information.
    /// </remarks>
    /// <author>Gilles Bayon</author>
    /// <author>Erich Eichinger</author>
    [Serializable]
    public class SitecoreLogger : AbstractLogger
    {
        #region Static Fields

        /// <summary>
        ///     The caller stack boundary type.
        /// </summary>
        private static Type callerStackBoundaryType;

        #endregion

        #region Fields

        /// <summary>
        ///     The _logger.
        /// </summary>
        private readonly ILogger _logger;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SitecoreLogger"/> class.
        ///     Constructor
        /// </summary>
        /// <param name="log">
        /// </param>
        protected internal SitecoreLogger(ILoggerWrapper log)
        {
            this._logger = log.Logger;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Returns the global context for variables
        /// </summary>
        public override IVariablesContext GlobalVariablesContext
        {
            get
            {
                return new SitecoreGlobalVariablesContext();
            }
        }

        /// <summary />
        public override bool IsDebugEnabled
        {
            get
            {
                return this._logger.IsEnabledFor(Level.DEBUG);
            }
        }

        /// <summary />
        public override bool IsErrorEnabled
        {
            get
            {
                return this._logger.IsEnabledFor(Level.ERROR);
            }
        }

        /// <summary />
        public override bool IsFatalEnabled
        {
            get
            {
                return this._logger.IsEnabledFor(Level.FATAL);
            }
        }

        /// <summary />
        public override bool IsInfoEnabled
        {
            get
            {
                return this._logger.IsEnabledFor(Level.INFO);
            }
        }

        /// <summary />
        public override bool IsTraceEnabled
        {
            get
            {
                return this._logger.IsEnabledFor(Level.TRACE);
            }
        }

        /// <summary />
        public override bool IsWarnEnabled
        {
            get
            {
                return this._logger.IsEnabledFor(Level.WARN);
            }
        }

        /// <summary>
        ///     Returns the thread-specific context for variables
        /// </summary>
        public override IVariablesContext ThreadVariablesContext
        {
            get
            {
                return new SitecoreThreadVariablesContext();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Maps <see cref="T:Common.Logging.LogLevel"/> to log4net's <see cref="T:log4net.Core.Level"/>
        /// </summary>
        /// <param name="logLevel">
        /// </param>
        /// <returns>
        /// The <see cref="Level"/>.
        /// </returns>
        public static Level GetLevel(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.All:
                    return Level.ALL;
                case LogLevel.Trace:
                    return Level.TRACE;
                case LogLevel.Debug:
                    return Level.DEBUG;
                case LogLevel.Info:
                    return Level.INFO;
                case LogLevel.Warn:
                    return Level.WARN;
                case LogLevel.Error:
                    return Level.ERROR;
                case LogLevel.Fatal:
                    return Level.FATAL;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, "unknown log level");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Actually sends the message to the underlying log system.
        /// </summary>
        /// <param name="logLevel">
        /// the level of this log event.
        /// </param>
        /// <param name="message">
        /// the message to log
        /// </param>
        /// <param name="exception">
        /// the exception to log (may be null)
        /// </param>
        protected override void WriteInternal(LogLevel logLevel, object message, Exception exception)
        {
            if (callerStackBoundaryType == null)
            {
                lock (this.GetType())
                {
                    var local0 = new StackTrace();
                    var local1 = this.GetType();
                    callerStackBoundaryType = typeof(AbstractLogger);
                    for (var local2 = 1; local2 < local0.FrameCount; ++local2)
                    {
                        if (!this.IsInTypeHierarchy(local1, local0.GetFrame(local2).GetMethod().DeclaringType))
                        {
                            callerStackBoundaryType = local0.GetFrame(local2 - 1).GetMethod().DeclaringType;
                            break;
                        }
                    }
                }
            }

            var level = GetLevel(logLevel);
            this._logger.Log(callerStackBoundaryType.FullName, level, message, exception);
        }

        /// <summary>
        /// The is in type hierarchy.
        /// </summary>
        /// <param name="currentType">
        /// The current type.
        /// </param>
        /// <param name="checkType">
        /// The check type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsInTypeHierarchy(Type currentType, Type checkType)
        {
            for (; currentType != (Type)null && currentType != typeof(object); currentType = currentType.BaseType)
            {
                if (currentType == checkType)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}