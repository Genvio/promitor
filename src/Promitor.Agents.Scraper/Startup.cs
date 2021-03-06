﻿using System;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Promitor.Agents.Core;
using Promitor.Agents.Scraper.Configuration;
using Promitor.Agents.Scraper.Configuration.Sinks;
using Promitor.Agents.Scraper.Discovery;
using Promitor.Agents.Scraper.Extensions;
using Promitor.Core;
using Promitor.Core.Scraping.Configuration.Serialization.v1.Mapping;
using Promitor.Integrations.AzureMonitor.Logging;
using Promitor.Integrations.Sinks.Atlassian.Statuspage;
using Serilog;

namespace Promitor.Agents.Scraper
{
    public class Startup : AgentStartup
    {
        private const string ComponentName = "Promitor Scraper";

        public Startup(IConfiguration configuration)
            : base(configuration)
        {
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string openApiDescription = BuildOpenApiDescription(Configuration);
            services.AddHttpClient<ResourceDiscoveryClient>(client =>
            {
                // Provide Promitor User-Agent
                client.DefaultRequestHeaders.UserAgent.TryParseAdd(Http.Headers.UserAgents.Scraper);
            });
            services.AddHttpClient<IAtlassianStatuspageClient, AtlassianStatuspageClient>(client =>
            {
                // Provide Promitor User-Agent
                client.DefaultRequestHeaders.UserAgent.TryParseAdd(Http.Headers.UserAgents.Scraper);

                // Auth all requests
                var apiKey = Configuration[EnvironmentVariables.Integrations.AtlassianStatuspage.ApiKey];
                client.DefaultRequestHeaders.Add("Authorization", $"OAuth {apiKey}");
            });

            services.UseWebApi()
                .AddHttpCorrelation()
                .AddAutoMapper(typeof(V1MappingProfile).Assembly)
                .DefineDependencies()
                .ConfigureValidation()
                .ConfigureYamlConfiguration(Configuration)
                .UseOpenApiSpecifications("Promitor - Scraper API v1", openApiDescription, 1);

            services.AddHealthChecks()
                   .AddResourceDiscoveryHealthCheck(Configuration);
            
            services.UseMetricSinks(Configuration)
                .ScheduleMetricScraping();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandling()
                .UseRequestTracking()
                .UseHttpCorrelation()
                .UseRouting()
                .UseMetricSinks(Configuration)
                .ExposeOpenApiUi()
                .UseEndpoints(endpoints => endpoints.MapControllers());
            UseSerilog(ComponentName, app.ApplicationServices);
        }

        protected override LoggerConfiguration FilterTelemetry(LoggerConfiguration loggerConfiguration)
        {
            var standardConfiguration = base.FilterTelemetry(loggerConfiguration);

            var azureMonitorConfiguration = Configuration.Get<ScraperRuntimeConfiguration>()?.AzureMonitor?.Logging;
            if (azureMonitorConfiguration == null)
            {
                throw new Exception("Unable to get logging configuration for Azure Monitor");
            }

            standardConfiguration.Filter.With(new AzureMonitorLoggingFilter(azureMonitorConfiguration));
            return standardConfiguration;
        }

        private string BuildOpenApiDescription(IConfiguration configuration)
        {
            var metricSinkConfiguration = configuration.GetSection("metricSinks").Get<MetricSinkConfiguration>();
            var openApiDescriptionBuilder = new StringBuilder();
            openApiDescriptionBuilder.Append("Collection of APIs to manage the Promitor Scraper.\r\n\r\n");
            openApiDescriptionBuilder.AppendLine("Configured metric sinks are:\r\n");

            if (metricSinkConfiguration != null)
            {
                if (metricSinkConfiguration.PrometheusScrapingEndpoint != null)
                {
                    var prometheusScrapingBaseUri = metricSinkConfiguration.PrometheusScrapingEndpoint.BaseUriPath;
                    openApiDescriptionBuilder.AppendLine($"<li>Prometheus scrape endpoint is exposed at <a href=\"./../..{prometheusScrapingBaseUri}\" target=\"_blank\">{prometheusScrapingBaseUri}</a></li>");
                }

                if (metricSinkConfiguration.Statsd != null)
                {
                    openApiDescriptionBuilder.AppendLine($"<li>StatsD server located on {metricSinkConfiguration.Statsd.Host}:{metricSinkConfiguration.Statsd.Port}</li>");
                }
            }

            return openApiDescriptionBuilder.ToString();
        }
    }
}