# CodeTest

> **Background**
>
> This was a code challenge Maersk gave me as part of a hiring process. I was offered the job but turned it down.
> Since Maersk has moved on to new challenges, I decided to publish my solution.
>
> **A note on MediatR:** I wouldn't use MediatR as a library today. I still like the mediator pattern, but starting with
> v13 (July 2025) MediatR is no longer free for everyone: it moved to Lucky Penny Software under a dual
> commercial / Reciprocal Public License (RPL-1.5) model. A free Community edition exists only for companies and individuals
> under USD 5M in annual revenue, and 12.x is the last Apache 2.0 release. This solution uses MediatR 9.0.0, which is still Apache 2.0.

## Thoughts

1. I used **MediatR** to publish/consume commands and request/response queries (CQRS).
2. I considered running the Command and Query services as hosted services, with a queued background task that uses
   channels to enqueue jobs, but went with MediatR in the end. Its API makes it easy to replace with a message broker,
   and it is simple to implement.
3. There are **3 endpoints** in Swagger: start a job, get the result of a job, and get the status of all jobs.
4. Data is persisted with **Entity Framework InMemory**. That would allow unit testing the repository, but I usually
   integration test my repositories, so I did that here to show how I would do it.
5. A **CorrelationId middleware** adds a correlation id to the logs, giving full traceability for each API call.
6. **Immutable collections and records** are used wherever values shouldn't be modified.
7. A **Shared Test Dependencies** project is prepared to be published as a NuGet package (see its `.csproj`), but for
   simplicity the tests reference the project directly.
8. Full test coverage is not delivered, and that wasn't the goal. I wanted to share enough to be evaluated.
9. I spent too much time on this task, probably 10+ effective hours. I had to work on it at night, as I didn't have time
   during the day. To hide my shame (committing code in the middle of the night), I published my master branch instead of
   cloning my repository. :)

## Run instructions

### Prerequisites

- The latest [.NET 5.0.x SDK](https://dotnet.microsoft.com/download/dotnet/5.0)
- Rider, Visual Studio, or another IDE that supports .NET development

### Self-contained executable

To create a self-contained executable for a specific runtime, follow the official
[docs](https://docs.microsoft.com/en-us/dotnet/core/deploying/#publish-self-contained) or run one of these commands:

```bash
dotnet publish -r win-x64
dotnet publish -r linux-x64
```

A self-contained executable does not require the .NET SDK or runtime to be installed on the machine that runs it.

## License

Released into the public domain under [The Unlicense](LICENSE). Use it for anything, no attribution required.
