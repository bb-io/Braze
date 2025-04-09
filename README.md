# Blackbird.io Braze

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

<!-- begin docs -->

Braze is a customer engagement platform that helps brands deliver personalized, real-time messaging across channels like email, mobile, SMS, and web.

## Before setting up

Before you can connect you need to make sure that:

- You have access to a Braze instance and the right permissions to create API keys.
- Create an API key under Settings > APIs and identifiers > API Keys > Create API Key.
  - Under permissions grant all permissions for _Campaigns_ and _Canvas_

## Connecting

1. Navigate to apps and search for Braze.
2. Click _Add Connection_.
3. Name your connection for future reference e.g. 'My Braze'.
4. Fill in the Braze instance your account is on. You can find a table of instances [here](https://www.braze.com/docs/api/basics/#braze-rest-api-collection).
5. Fill in the _API Key_ obtained earlier.
6. Click _Connect_

## Actions

- **Search campaigns** returns a list of campaigns. Searchable by last edited date.
- **Get campaign** returns campaign metadata.
- **Download campaign message** downloads the campaign message in both JSON and HTML formats. Use in conjunction with **Upload campaign message** after translation.
- **Upload campaign message** uploads the campaign message content from a translated file. use in conjunction with **Download campaign message**.

More actions will be added soon.

## Feedback

Do you want to use this app or do you have feedback on our implementation? Reach out to us using the [established channels](https://www.blackbird.io/) or create an issue.

<!-- end docs -->
