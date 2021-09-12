# Thoughts

1. I decided to use MediatR to publish/consume commands & request/response queries (CQRS).
1. I added 3 endspoints in Swagger, so users can start a job, get the result and get status of all jobs.
1. I used EntityFramework inMemory to persist data, and I realise this allows me to unittest my repository, but I usually integrationTest my repositories and did so here, to demonstrate how I would do it.
1. I added CorrelationId Middleware to logs, so full traceability for each API call is present.
1. I also used Immutable collections and Records everywhere where values shouldn't be modified.
1. I added a project for Shared Test Dependencies. I prepared it to be published as a nuget package (csproj), but for simplicity I just refer to the project from my tests.
1. I realise full test coverage is not delivered, but that wasn't my goal. I just wanted to share enough, so I can be evaluted.
1. I spent too much time on this task. Probably about 10+ effective hours. However I had to stay up during the night, as I didn't have time during the day. To hide my shame (commiting code in the middle of the night), I published my master branch instead of cloning my repository. :)

# Run Instructions

In order to run this solution, please download and install the latest .NET 5.0.X SDK from [dotnet](microsoft	https://dotnet.microsoft.com/download/dotnet/5.0)

Furthermore, you must either have Rider, Visual Studio or another IDE that support .NET development.

If you would like to create a self-contained executable for a specific runtime, please follow the official [docs](https://docs.microsoft.com/en-us/dotnet/core/deploying/#publish-self-contained) or enter one of the following commands, from the below examples.

 *Examples:*

    dotnet publish -r win-x64
    dotnet publish -r linux-x64

By creating a self-contained exectuable, it is no longer required for the person who runs the executable, to have .NET SDK or Runtime installed.