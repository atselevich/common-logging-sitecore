// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Log4NetLoggerFactoryAdapter.cs" company="">
//   
// </copyright>
// <summary>
//   Concrete subclass of ILoggerFactoryAdapter specific to log4net 1.2.9-1.2.11.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System;
using System.IO;

using Common.Logging;
using Common.Logging.Configuration;
using Common.Logging.Factory;

using log4net.Config;

namespace Common.Logging.Sitecore
{
    using log4net;

    using ILog = ILog;

    /// <summary>
    ///     Concrete subclass of ILoggerFactoryAdapter specific to Sitecore.Logging.dll (log4net v1.2.0.30714 aka Beta8).
    /// </summary>
    /// <remarks>
    ///     The following configuration property values may be configured:
    ///     <list type="bullet">
    ///         <item>
    ///             <c>configType</c>: <c>INLINE|FILE|FILE-WATCH|EXTERNAL</c>
    ///         </item>
    ///         <item>
    ///             <c>configFile</c>: log4net configuration file path in case of FILE or FILE-WATCH
    ///         </item>
    ///     </list>
    ///     The configType values have the following implications:
    ///     <list type="bullet">
    ///         <item>
    ///             INLINE: simply calls <c>XmlConfigurator.Configure()</c>
    ///         </item>
    ///         <item>
    ///             FILE: calls <c>XmlConfigurator.Configure(System.IO.FileInfo)</c> using <c>configFile</c>.
    ///         </item>
    ///         <item>
    ///             FILE-WATCH: calls <c>XmlConfigurator.ConfigureAndWatch(System.IO.FileInfo)</c> using <c>configFile</c>.
    ///         </item>
    ///         <item>
    ///             EXTERNAL: does nothing and expects log4net to be configured elsewhere.
    ///         </item>
    ///         <item>
    ///             &lt;any&gt;: calls <c>BasicConfigurator.Configure()</c>
    ///         </item>
    ///     </list>
    /// </remarks>
    /// <example>
    ///     The following snippet shows an example of how to configure Sitecore's logging with Common.Logging. Sitecore's web.config already has the log4net logging configuration done.
    ///     <code>
    ///     &lt;configuration&gt;
    ///           &lt;configSections&gt;
    ///             &lt;sectionGroup name = &quot; common&quot;&gt;
    ///                 &lt;section name = &quot; logging&quot; type=&quot;Common.Logging.ConfigurationSectionHandler, Common.Logging&quot; /&gt;
    ///     &lt;/sectionGroup&gt;
    ///     &lt;/configSections&gt;
    ///     &lt;common&gt;
    ///         &lt;logging&gt;
    ///             &lt;factoryAdapter type = &quot; Common.Logging.Sitecore.SitecoreLoggerFactoryAdapter, Common.Logging.Sitecore&quot;&gt;
    ///                 &lt;arg key = &quot; configType&quot; value=&quot;INLINE&quot; /&gt;
    ///             &lt;/factoryAdapter&gt;
    ///         &lt;/logging&gt;
    ///     &lt;/common&gt;
    ///     &lt;/configuration&gt;
    /// 
    /// </code>
    /// </example>
    /// <author>Gilles Bayon</author>
    /// <author>Erich Eichinger</author>
    /// <author>Alex Tselevich</author>
    public class SitecoreLoggerFactoryAdapter : AbstractCachingLoggerFactoryAdapter
    {
        #region Fields

        /// <summary>
        ///     The runtime.
        /// </summary>
        private readonly ILog4NetRuntime runtime;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SitecoreLoggerFactoryAdapter"/> class.
        ///     Constructor
        /// </summary>
        /// <param name="properties">
        /// configuration properties, see
        ///     <see cref="T:Common.Logging.Log4Net.Log4NetLoggerFactoryAdapter"/> for more.
        /// </param>
        public SitecoreLoggerFactoryAdapter(NameValueCollection properties)
            : this(properties, new Log4NetRuntime())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SitecoreLoggerFactoryAdapter"/> class.
        ///     Constructor for binary backwards compatibility with non-portableversions
        /// </summary>
        /// <param name="properties">
        /// The properties.
        /// </param>
        [Obsolete("Use Constructor taking Common.Logging.Configuration.NameValueCollection instead")]
        public SitecoreLoggerFactoryAdapter(System.Collections.Specialized.NameValueCollection properties)
            : this(NameValueCollectionHelper.ToCommonLoggingCollection(properties))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SitecoreLoggerFactoryAdapter"/> class.
        ///     Constructor accepting configuration properties and an arbitrary
        ///     <see cref="T:Common.Logging.Log4Net.Log4NetLoggerFactoryAdapter.ILog4NetRuntime"/> instance.
        /// </summary>
        /// <param name="properties">
        /// configuration properties, see
        ///     <see cref="T:Common.Logging.Log4Net.Log4NetLoggerFactoryAdapter"/> for more.
        /// </param>
        /// <param name="runtime">
        /// a log4net runtime adapter
        /// </param>
        protected SitecoreLoggerFactoryAdapter(NameValueCollection properties, ILog4NetRuntime runtime)
            : base(true)
        {
            if (runtime == null)
            {
                throw new ArgumentNullException(nameof(runtime));
            }

            this.runtime = runtime;
            var str1 = ArgUtils.GetValue(properties, "configType", string.Empty).ToUpper();
            var str2 = ArgUtils.GetValue(properties, "configFile", string.Empty);
            if (str2.StartsWith("~/") || str2.StartsWith("~\\"))
            {
                str2 = $"{AppDomain.CurrentDomain.BaseDirectory.TrimEnd('/', '\\')}/{str2.Substring(2)}";
            }

            if (str1 == "FILE" || str1 == "FILE-WATCH")
            {
                if (str2 == string.Empty)
                {
                    throw new ConfigurationException(
                        "Configuration property 'configFile' must be set for Sitecore.Logging log4Net configuration of type 'FILE' or 'FILE-WATCH'.");
                }

                if (!File.Exists(str2))
                {
                    throw new ConfigurationException("Sitecore.Logging log4net configuration file '" + str2 + "' does not exists");
                }
            }

            switch (str1)
            {
                case "INLINE":
                    this.runtime.XmlConfiguratorConfigure();
                    break;
                case "FILE":
                    this.runtime.XmlConfiguratorConfigure(str2);
                    break;
                case "FILE-WATCH":
                    this.runtime.XmlConfiguratorConfigureAndWatch(str2);
                    break;
                case "EXTERNAL":
                    break;
                default:
                    this.runtime.BasicConfiguratorConfigure();
                    break;
            }
        }

        #endregion

        #region Interfaces

        /// <summary>
        ///     Abstract interface to the underlying log4net runtime
        /// </summary>
        public interface ILog4NetRuntime
        {
            #region Public Methods and Operators

            /// <summary>
            ///     Calls <see cref="M:log4net.Config.BasicConfigurator.Configure" />
            /// </summary>
            void BasicConfiguratorConfigure();

            /// <summary>
            /// Calls <see cref="M:Common.Logging.LogManager.GetLogger(System.String)"/>
            /// </summary>
            /// <param name="name">
            /// The name.
            /// </param>
            /// <returns>
            /// The <see cref="ILog"/>.
            /// </returns>
            log4net.ILog GetLogger(string name);

            /// <summary>
            ///     Calls <see cref="M:log4net.Config.XmlConfigurator.Configure" />
            /// </summary>
            void XmlConfiguratorConfigure();

            /// <summary>
            /// Calls <see cref="M:log4net.Config.XmlConfigurator.Configure(System.IO.FileInfo)"/>
            /// </summary>
            /// <param name="configFile">
            /// The config File.
            /// </param>
            void XmlConfiguratorConfigure(string configFile);

            /// <summary>
            /// Calls <see cref="M:log4net.Config.XmlConfigurator.ConfigureAndWatch(System.IO.FileInfo)"/>
            /// </summary>
            /// <param name="configFile">
            /// The config File.
            /// </param>
            void XmlConfiguratorConfigureAndWatch(string configFile);

            #endregion
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create a ILog instance by name
        /// </summary>
        /// <param name="name">
        /// </param>
        /// <returns>
        /// The <see cref="ILog"/>.
        /// </returns>
        protected override ILog CreateLogger(string name)
        {
            return new SitecoreLogger(this.runtime.GetLogger(name));
        }

        #endregion

        /// <summary>
        ///     The log 4 net runtime.
        /// </summary>
        private class Log4NetRuntime : ILog4NetRuntime
        {
            #region Public Methods and Operators

            /// <summary>
            ///     The basic configurator configure.
            /// </summary>
            public void BasicConfiguratorConfigure()
            {
                BasicConfigurator.Configure();
            }

            /// <summary>
            /// The get logger.
            /// </summary>
            /// <param name="name">
            /// The name.
            /// </param>
            /// <returns>
            /// The <see cref="ILog"/>.
            /// </returns>
            public log4net.ILog GetLogger(string name)
            {
                return LogManager.GetLogger(name);
            }

            /// <summary>
            ///     The xml configurator configure.
            /// </summary>
            public void XmlConfiguratorConfigure()
            {
                XmlConfigurator.Configure();
            }

            /// <summary>
            /// The xml configurator configure.
            /// </summary>
            /// <param name="configFile">
            /// The config file.
            /// </param>
            public void XmlConfiguratorConfigure(string configFile)
            {
                XmlConfigurator.Configure(new FileInfo(configFile));
            }

            /// <summary>
            /// The xml configurator configure and watch.
            /// </summary>
            /// <param name="configFile">
            /// The config file.
            /// </param>
            public void XmlConfiguratorConfigureAndWatch(string configFile)
            {
                XmlConfigurator.ConfigureAndWatch(new FileInfo(configFile));
            }

            #endregion
        }
    }
}