---
layout: default
title: Promitor - Bringing Azure Monitor metrics where you need them
---

[![License](https://img.shields.io/github/license/mashape/apistatus.svg?style=flat-square)](https://github.com/tomkerkhove/promitor/blob/master/LICENSE)
[![Build Status](https://img.shields.io/azure-devops/build/tomkerkhove/promitor/50/master.svg?label=Scraper%20Agent%20-%20CI&style=flat-square)](https://dev.azure.com/tomkerkhove/Promitor/_build/latest?definitionId=50&branchName=master)
[![Docker Pulls](https://img.shields.io/docker/pulls/tomkerkhove/promitor-agent-scraper.svg?style=flat-square)](https://hub.docker.com/r/tomkerkhove/promitor-agent-scraper/)
[![Docker Stars](https://img.shields.io/docker/stars/tomkerkhove/promitor-agent-scraper.svg?style=flat-square)](https://hub.docker.com/r/tomkerkhove/promitor-agent-scraper/)
[![Donate](https://img.shields.io/badge/Donate%20via-GitHub-blue.svg?style=flat-square)](https://github.com/users/tomkerkhove/sponsorship)

**Promitor** is an Azure Monitor scraper which makes the metrics available
for metric systems such as Atlassian Statuspage, Prometheus and StatsD.

{:refdef: style="text-align: center;"}
![Promitor](./media/logos/promitor.png)
{: refdef}

## Running Promitor Scraper

Running Promitor Scraper is super easy:

```shell
docker run -d -p 8999:80 --name promitor-agent-scraper \
                         --env PROMITOR_AUTH_APPID='<azure-ad-app-id>'   \
                         --env-file C:/Promitor/az-mon-auth.creds \
                         --volume C:/Promitor/metrics-declaration.yaml:/config/metrics-declaration.yaml \
                         --volume C:/Promitor/runtime.yaml:/config/runtime.yaml \
                         tomkerkhove/promitor-agent-scraper:1.6.1
```

Docker image is available on [Docker Hub](https://hub.docker.com/r/tomkerkhove/promitor-agent-scraper/).

## Features

- Automatically pushes metrics to systems such as Prometheus & StatsD
- Automatically scrapes Azure Monitor metrics (single and multi-dimensional) across various subscription & resource groups
- Built-in support for a variety of Azure services ([overview](configuration/v1.x/metrics#supported-azure-services))
- Easy to declare metrics to scrape via YAML & APIs
- Easily deployable via Docker & Kubernetes
- Sends telemetry to container logs & Azure Application Insights
- Available for Linux & Windows runtimes
- Support for all Azure clouds

And there is more on the way - Check our [backlog](https://github.com/tomkerkhove/promitor/issues)
and vote for features!

## Documentation

- **[How It Works](concepts/how-it-works)**
- **Metrics**
  - [General Declaration](configuration/v1.x/metrics)
  - [Supported Providers](configuration/v1.x/metrics#supported-azure-services)
  - [What labels do we provide?](metrics/labels)
- **Deployment**
  - [Overview](deployment)
  - Scraper
    - [Docker](deployment/scraper/#docker)
    - [Kubernetes](deployment/scraper/#kubernetes)
  - Resource Discovery
    - [Docker](deployment/resource-discovery/#docker)
  - [Image Tagging Strategy](deployment#image-tagging-strategy)
- **Configuration**
  - Scraper
    - [Overview of metric sinks](configuration/v1.x/runtime#metric-sinks)
      - [Atlassian Statuspage](configuration/v2.x/runtime#atlassian-statuspage)
      - [Prometheus Scraping Endpoint](configuration/v1.x/runtime#prometheus-scraping-endpoint)
      - [StatsD](configuration/v1.x/runtime#statsd)
    - [Using resource discovery](configuration/v2.x/runtime#using-resource-discovery)
    - [Authentication with Azure Monitor](configuration/v1.x/azure-monitor)
    - [Logging & External Providers](configuration/v1.x/runtime#telemetry)
    - [Runtime](configuration/v1.x/runtime)
  - Resource Discovery
    - [Declaring resource discovery groups](configuration/v2.x/resource-discovery)
- **Operations**
  - [Azure Resource Manager API - Consumption & Throttling](operations#azure-resource-manager-api---consumption--throttling)
  - [Azure Monitor Integration](operations#azure-monitor-integration)
  - [Configuration REST APIs](operations#configuration-rest-apis)
  - [Health](operations#health)
- **Walkthroughs**
  - [Deploying Promitor, Prometheus, and Grafana on an AKS Cluster](/walkthrough/scrape-promitor-with-prometheus-on-azure-kubernetes-service)
  - [Migrate from Promitor Scraper 1.x to 2.x](/walkthrough/migrate-from-1.x-to-2.x)
- [**Frequently asked questions (FAQs)**](/faq)

## Support

Promitor is actively maintained and developed with best-effort support.

We do welcome PRs that implement features from our backlog and are always happy
to help you incorporate Promitor in your infrastructure, but do not provide 24/7
support. Are you having issues or feature requests?

Feel free to [let us know](https://github.com/tomkerkhove/promitor/issues/new/choose)!

## Customers

We are proud to have the following customer(s) running Promitor in production:

![Walmart Labs](./media/logos/customers/walmart-labs.jpg)
![ResDiary](./media/logos/customers/resdiary.png)

Learn more about how they are using Promitor:

- ["Monitor Azure Resources using Promitor"](https://medium.com/resdiary-product-team/monitor-azure-resources-using-promitor-b3d8384867c1)
 by ResDiary

## Thank you

We'd like to thank the following service(s) for supporting our open-source initiative!

- **Netlify** allows us to provide previews of our documentation changes in our
  pull requests that make it easier to review them.

<!-- markdownlint-disable MD033 -->
  <a href="https://www.netlify.com">
    <img src="https://www.netlify.com/img/global/badges/netlify-color-bg.svg" alt="Deploys by Netlify" />
  </a>
<!-- markdownlint-enable -->

But they are not the only one we'd like to thank!

For a full list of services, tooling & NuGet packages that support us -
 Have a look at our [Thank you](thank-you) page!

## License Information

This is licensed under The MIT License (MIT). Which means that you can use, copy,
modify, merge, publish, distribute, sublicense, and/or sell copies of the web application.
But you always need to state that Tom Kerkhove is the original author of this web
application.
