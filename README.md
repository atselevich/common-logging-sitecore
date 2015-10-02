# common-logging-sitecore
A Common.Logging adapter for Sitecore.Logging.dll (log4net v1.2.0.30714 aka Beta8) see https://github.com/net-commons/common-logging for the base project.

Sitecore.Logging.dll hasn't changed since Sitecore 6.6.0 rev 120918, it wraps a very old version of log4net. This adapter allows you to use Common.Logging with Sitecore.Logging. You may have a Sitecore agnostic library that is used in a Sitecore solution. If that library implements the Common.Logging ILog for all logging functionality, then you will be able to patch into Sitecore's logging using this adapter. That means that logging from your Sitecore agnostic assembly will go to the  Sitecore log as configured in the web.congfig.

In order for the adapter to function the following configuration needs to be added to the web.config:

```<configuration>
  <configSections>
	<sectionGroup name="common">
	  <section name="logging" type="Common.Logging.ConfigurationSectionHandler, Common.Logging" />
	</sectionGroup>
  </configSections>
  <common>
	<logging>
	  <factoryAdapter type="Common.Logging.Sitecore.SitecoreLoggerFactoryAdapter, Common.Logging.Sitecore">
		<arg key="configType" value="INLINE" />
	  </factoryAdapter>
	</logging>
  </common>
 </configuration>
