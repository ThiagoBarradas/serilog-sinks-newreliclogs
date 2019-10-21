﻿using System;
using System.Configuration;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.NewRelicLogs;

namespace Serilog
{
    /// <summary>
    /// Extends Serilog configuration to write events to NewRelic Logs
    /// </summary>
    public static class NewRelicLoggerConfigurationExtensions
    {
        /// <summary>
        /// </summary>
        /// <param name="loggerSinkConfiguration">The logger configuration.</param>
        /// <param name="endpointUrl">The NewRelic Logs API endpoint URL. Default is set to https://log-api.newrelic.com/log/v1 located in the US.</param>
        /// <param name="applicationName">Application name in NewRelic. This can be either supplied here or through "NewRelic.AppName" appSettings</param>
        /// <param name="licenseKey">New Relic APM License key. Either "licenseKey" or "insertKey" must be provided.</param>
        /// <param name="insertKey">New Relic Insert API key. Either "licenseKey" or "insertKey" must be provided.</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required 
        ///     in order to write an event to the sink.</param>
        /// <param name="batchPostingLimit">The maximum number of events to include in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <returns></returns>
        public static LoggerConfiguration NewRelicLogs(
            this LoggerSinkConfiguration loggerSinkConfiguration,
            string endpointUrl = "https://log-api.newrelic.com/log/v1",
            string applicationName = null,
            string licenseKey = null,
            string insertKey = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            int batchPostingLimit = NewRelicLogsSink.DefaultBatchPostingLimit,
            TimeSpan? period = null
            )
        {
            if (loggerSinkConfiguration == null)
            {
                throw new ArgumentNullException(nameof(loggerSinkConfiguration));
            }

            if (string.IsNullOrEmpty(applicationName))
            {
                applicationName = ConfigurationManager.AppSettings["NewRelic.AppName"];
                if (string.IsNullOrEmpty(applicationName))
                {
                    throw new ArgumentException("Must supply an application name either as a parameter or an appSetting", nameof(applicationName));
                }
            }

            if (string.IsNullOrWhiteSpace(endpointUrl))
            {
                throw new ArgumentException("NewRelic Logs API endpoint URL must be supplied");
            }

            if (string.IsNullOrWhiteSpace(licenseKey) && string.IsNullOrWhiteSpace(insertKey))
            {
                throw new ArgumentException("Either LicenseKey or InsertKey must be supplied");
            }

            var defaultPeriod = period ?? NewRelicLogsSink.DefaultPeriod;

            ILogEventSink sink = new NewRelicLogsSink(endpointUrl, applicationName, licenseKey, insertKey, batchPostingLimit, defaultPeriod);

            return loggerSinkConfiguration.Sink(sink, restrictedToMinimumLevel);
        }
    }
}