# Jira-Tempo Integration

## Jira

[Jira](https://www.atlassian.com/software/jira) is management information system developed by [Atlassian](https://www.atlassian.com/company) to help teams plan, track, and manage software development projects. Originally focused on bug tracking, Jira evolved into a robust platform that supports agile methodologies like [Scrum](https://en.wikipedia.org/wiki/Scrum_(software_development)) and [Kanban](https://en.wikipedia.org/wiki/Kanban_(development). It is especially popular among software development teams for its customizable workflows, integration capabilities, and detailed reporting features. As of 2025, Jira is used by over 180,000 customers, including major organizations like NASA, Cisco, and eBay, making it one of the most popular solutions to manage software product development and track software project issues.

## Tempo

[Tempo](https://www.tempo.io) is a popular plugin for Jira that enables time tracking, resource planning, and cost management within project workflows. It helps teams and organizations log work hours, plan team capacity, and generate reports for billing, payroll, and productivity analysis. Tempo integrates seamlessly with Jira issues, making it easy for users to record time spent on tasks directly within their existing workflows. Used by thousands of companies globally, Tempo provides valuable insights into how time is spent across projects, improving transparency and operational efficiency.

## Problem

Jira and Tempo together form a powerful and flexible platform for managing projects, tracking time, and understanding team performance. With Jira's customizable workflows and Tempo's robust time-tracking and resource planning capabilities, organizations can tailor their setup to fit virtually any process or reporting need. 

However, this level of flexibility comes with some cost: while you can capture almost any kind of data, the search and reporting features built into the user interface sometimes fall short when you need specific, nuanced insights into your data.

## Solution

Fortunately, both Jira and Tempo provide an API to bridge the gap. You can write custom scripts and integrations to surface the exact data you need for decision-making, compliance, or billing.

Both APIs are comprehensive and well-documented, but diving into the deep end can be intimidating. This project contains the source code you can use as a starting point for your own integration.

It is important to note: The source code in this repository is almost certain NOT to work for your specific needs out-of-the-box. You'll need to get your hands a little dirty, with code changes that fit your specific Jira/Tempo implementation and configuration.

### Getting Started

First, you'll need a Jira API key. You can generate a key here:

https://id.atlassian.com/manage-profile/security/api-tokens

Second, you'll need a Tempo API key. The URL for the page where you can generate a new key looks like this:

https://example.atlassian.net/plugins/servlet/ac/io.tempo.jira/tempo-app#!/configuration/api-integration

> Replace "example" with the subdomain for your Jira account on the atlassian.net domain.

Next, update the appsettings.json file with the configuration settings for your environment.

Finally, implement code changes to the JiraClient and TempoClient classes that are necessary to extract the desired information from your account.

Now you can execute the console app on the command line. For example, this command generates a detail report and a summary report of time entries for the month of April 2025:

`dotnet run 2025-04-01 2025-04-30`

## Example

In my case, the end goal is to arrive at a summary of hours worked per Account (where each Jira issue is assigned to a customer Account), and a summary of hours worked by each person on the team.

I want the output in a simple CSV format, so I can copy and paste to an Excel spreadsheet.

For example:

Account,Hours
Alpha Corporation,10.50
Beta, Inc.,19.50
Gamma Ltd.,10.00

Worker,Hours
Alice,20.00
Bob,8.00
Carol,32.00

## Code Highlights

As you can see, the high-level code is relatively simple:

```
public async Task Run()
{
  var jira = new JiraClient(_settings);

  var tempo = new TempoClient(_settings, jira);

  var entries = await tempo.GetTimeEntries(_settings.Since, _settings.Until);

  var list = entries.OrderBy(x => x.Date)
    .ThenBy(x => x.Worker)
    .ThenBy(x => x.Account)
    .ThenBy(x => x.Type)
    .ThenBy(x => x.Issue)
    .ToList();

  BuildReports(list);
}
```

You'll find the grit in the TempoClient and JiraClient classes.

When you send a request to the Tempo API for a list of [Worklogs](https://apidocs.tempo.io/#tag/Worklogs) it responds with a paged data set. You need to extra metadata from the response to determine the URL for requesting subsequent pages.

This bit of magic is handled here:

[image]

If you are extracting a custom field value from a Jira issue, then you'll need to do some experimentation to identify the name assigned by Jira to your custom field.

For example, I have a custom field named "Account", and the name assigned by Jira to this field is "customfield_10033". It is a little like an Easter egg hunt...

Parsing the data you want from the Jira API is where you'll truly earn your keep as a developer. The parsing required for my case is pretty hairy:

[image]

The good news is that it works. After you've identified the data you want, and after you've written the code that's needed to parse it from the API's response, you can run the app again and again for any date range that you need.

This utility has proven to be a big help for our project managers and our billing department!

## Next Steps

Here are a few links so you can explore the Jira and Tempo API documentation for yourself.

* [Jira API Reference](https://developer.atlassian.com/cloud/jira/platform/rest/v3/intro)
* [Tempo API Reference](https://apidocs.tempo.io)