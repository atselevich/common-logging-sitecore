// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommonLoggingAppender.cs" company="">
//   
// </copyright>
// <summary>
//   Routes log events to Common.Logging infrastructure.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace Common.Logging.Sitecore
{
    using System;
    using System.Collections.Generic;
    
    using Common.Logging;
    using Common.Logging.Configuration;

    using log4net.Appender;
    using log4net.Layout;
    using log4net.spi;

    /// <summary>
    ///     Routes log events to Common.Logging infrastructure.
    /// </summary>
    /// <example>
    ///     To route all events logged using log4net to Common.Logging, you need to configure this appender as shown below:
    ///     <code>
    /// &lt;log4net&gt;
    ///                 &lt;appender name="CommonLoggingAppender"
    ///                           type="Common.Logging.Log4Net.CommonLoggingAppender, Common.Logging.Log4Net129"&gt;
    ///                     &lt;layout type="log4net.Layout.PatternLayout, log4net"&gt;
    ///                         &lt;param name="ConversionPattern" value="%level - %class.%method: %message" /&gt;
    ///                     &lt;/layout&gt;
    ///                 &lt;/appender&gt;
    /// 
    ///                 &lt;root&gt;
    ///                     &lt;level value="ALL" /&gt;
    ///                     &lt;appender-ref ref="CommonLoggingAppender" /&gt;
    ///                 &lt;/root&gt;
    ///             &lt;/log4net&gt;
    /// 
    /// </code>
    /// </example>
    /// <author>Erich Eichinger</author>
    public class CommonLoggingAppender : AppenderSkeleton
    {
        #region Static Fields

        /// <summary>
        ///     The log methods.
        /// </summary>
        private static readonly Dictionary<Level, LogMethod> logMethods = new Dictionary<Level, LogMethod>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="CommonLoggingAppender" /> class.
        /// </summary>
        static CommonLoggingAppender()
        {
            string str1;
            logMethods[Level.TRACE] = (log, msg, ex) => log.Trace(m => str1 = m(msg()), ex);
            string str2;
            logMethods[Level.DEBUG] = (log, msg, ex) => log.Debug(m => str2 = m(msg()), ex);
            string str3;
            logMethods[Level.INFO] = (log, msg, ex) => log.Info(m => str3 = m(msg()), ex);
            string str4;
            logMethods[Level.WARN] = (log, msg, ex) => log.Warn(m => str4 = m(msg()), ex);
            string str5;
            logMethods[Level.ERROR] = (log, msg, ex) => log.Error(m => str5 = m(msg()), ex);
            string str6;
            logMethods[Level.FATAL] = (log, msg, ex) => log.Fatal(m => str6 = m(msg()), ex);
            logMethods[Level.ALL] = logMethods[Level.TRACE];
            logMethods[Level.OFF] = (log, msg, ex) => { };
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     The log method.
        /// </summary>
        /// <param name="logger">
        ///     The logger.
        /// </param>
        /// <param name="fmtr">
        ///     The fmtr.
        /// </param>
        /// <param name="exception">
        ///     The exception.
        /// </param>
        private delegate void LogMethod(ILog logger, MessageFormatter fmtr, Exception exception);

        /// <summary>
        ///     The message formatter.
        /// </summary>
        private delegate string MessageFormatter();

        #endregion

        #region Public Properties

        /// <summary>
        ///     Get or set the layout for this appender
        /// </summary>
        public override ILayout Layout
        {
            get
            {
                if (!(base.Layout is ExceptionAwareLayout))
                {
                    return base.Layout;
                }

                return ((ExceptionAwareLayout)base.Layout).InnerLayout;
            }

            set
            {
                ArgUtils.AssertNotNull("Layout", value);
                if (!(value is ExceptionAwareLayout))
                {
                    value = new ExceptionAwareLayout(value);
                }

                base.Layout = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the closest level supported by Common.Logging of the given log4net level
        /// </summary>
        /// <param name="currentLevel">
        /// The current Level.
        /// </param>
        /// <returns>
        /// The <see cref="Level"/>.
        /// </returns>
        protected static Level GetClosestLevel(Level currentLevel)
        {
            if (currentLevel.Equals(Level.OFF))
            {
                return Level.OFF;
            }

            if (currentLevel.Equals(Level.ALL))
            {
                return Level.ALL;
            }

            if (currentLevel >= Level.FATAL)
            {
                return Level.FATAL;
            }

            if (currentLevel >= Level.ERROR)
            {
                return Level.ERROR;
            }

            if (currentLevel >= Level.WARN)
            {
                return Level.WARN;
            }

            if (currentLevel >= Level.INFO)
            {
                return Level.INFO;
            }

            if (currentLevel >= Level.DEBUG)
            {
                return Level.DEBUG;
            }

            if (currentLevel >= Level.TRACE)
            {
                return Level.TRACE;
            }

            return Level.ALL;
        }

        /// <summary>
        /// Sends the given log event to Common.Logging
        /// </summary>
        /// <param name="loggingEvent">
        /// The logging Event.
        /// </param>
        protected override void Append(LoggingEvent loggingEvent)
        {
            var logger = LogManager.GetLogger(loggingEvent.LoggerName);
            var closestLevel = GetClosestLevel(loggingEvent.Level);
            var logMethod = logMethods[closestLevel];
            loggingEvent.Fix = FixFlags.LocationInfo;

            // Using reflection to get the exception for this logging event for old version of log4net that is wrapped by Sitecore.Logging
            // New versions of log4net expose the exception
            var exception = loggingEvent.GetPrivatePropertyValue<Exception>("m_thrownException");
            logMethod(logger, () => this.RenderLoggingEvent(loggingEvent), exception);
        }

        #endregion

        /// <summary>
        ///     Wrapper class that prevents exceptions from being rendered in the message
        /// </summary>
        private class ExceptionAwareLayout : ILayout
        {
            #region Fields

            /// <summary>
            ///     The inner layout.
            /// </summary>
            public readonly ILayout InnerLayout;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="ExceptionAwareLayout"/> class.
            /// </summary>
            /// <param name="inner">
            /// The inner.
            /// </param>
            public ExceptionAwareLayout(ILayout inner)
            {
                this.InnerLayout = inner;
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets the content type.
            /// </summary>
            public string ContentType
            {
                get
                {
                    return this.InnerLayout.ContentType;
                }
            }

            /// <summary>
            ///     Gets the footer.
            /// </summary>
            public string Footer
            {
                get
                {
                    return this.InnerLayout.Footer;
                }
            }

            /// <summary>
            ///     Gets the header.
            /// </summary>
            public string Header
            {
                get
                {
                    return this.InnerLayout.Header;
                }
            }

            /// <summary>
            ///     Gets a value indicating whether ignores exception.
            /// </summary>
            public bool IgnoresException
            {
                get
                {
                    return false;
                }
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            /// The format.
            /// </summary>
            /// <param name="loggingEvent">
            /// The logging event.
            /// </param>
            /// <returns>
            /// The <see cref="string"/>.
            /// </returns>
            public string Format(LoggingEvent loggingEvent)
            {
                return this.InnerLayout.Format(loggingEvent);
            }

            #endregion
        }
    }
}