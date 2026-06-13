# EventHighway Client V2

EventHighway is an event-dispatch library built on top of a SQL database. When you submit an event it is persisted, routed to every registered listener, and each listener's handler is invoked. The result of every handler call is stored so you can query it later.

**Client V2** is the current generation of the EventHighway API. It is accessed through `EventHighwayClient.V2` and provides a clean, fluent way to wire up handlers, publish events, and read results.

---

## Table of Contents

1. [Core Concepts](#1-core-concepts)
2. [Prerequisites](#2-prerequisites)
3. [Step-by-Step Setup](#3-step-by-step-setup)
   - [Step 1 – Create the client](#step-1--create-the-client)
   - [Step 2 – Choose and create your handlers](#step-2--choose-and-create-your-handlers)
   - [Step 3 – Register handlers with the client](#step-3--register-handlers-with-the-client)
   - [Step 4 – Register an event address](#step-4--register-an-event-address)
   - [Step 5 – Register listeners on that address](#step-5--register-listeners-on-that-address)
   - [Step 6 – Submit an event](#step-6--submit-an-event)
   - [Step 7 – Fire pending events](#step-7--fire-pending-events)
   - [Step 8 – Read the results](#step-8--read-the-results)
4. [RestBearerEventHandler](#4-restbeareventhandler)
5. [DelegateEventHandler](#5-delegateeventhandler)
   - [Inline lambda](#51-inline-lambda)
   - [Calling a method on your class](#52-calling-a-method-on-your-class)
   - [Reading what the handler returned](#53-reading-what-the-handler-returned)
6. [Building a Custom Event Handler](#6-building-a-custom-event-handler)
   - [The IEventHandler contract](#61-the-ieventhandler-contract)
   - [Step 1 – Define your exception classes](#62-step-1--define-your-exception-classes)
   - [Step 2 – Implement the handler class](#63-step-2--implement-the-handler-class)
   - [Step 3 – Use handlerParams to pass configuration](#64-step-3--use-handlerparams-to-pass-configuration)
   - [Step 4 – Register and use the handler](#65-step-4--register-and-use-the-handler)
   - [Why the exception interfaces matter](#66-why-the-exception-interfaces-matter)
7. [Future Enhancements](#7-future-enhancements)
   - [Listener filter criteria](#71-listener-filter-criteria)

---

## 1. Core Concepts

| Concept | What it is |
|---|---|
| **Event Address** | A named channel (topic). Events are published to an address; listeners subscribe to it. |
| **Event** | A unit of work — a string payload plus metadata — published to an address. |
| **Event Listener** | A subscription on an address that pairs a *handler* with optional configuration. |
| **Handler** | The code that does the actual work when an event arrives. |
| **Listener Event** | The persisted record of one handler invocation — status, response code, response body, etc. |

The flow is:

```
publish EventV2 ──► EventAddressV2 ──► EventListenerV2 (×N) ──► IEventHandler
                                                                       │
                                                               EventHandlerResult
                                                                       │
                                                               ListenerEventV2 (stored)
```

---

## 2. Prerequisites

- .NET 10 or later
- A SQL Server database (e.g. LocalDB for local development)
- NuGet packages
  - `EventHighway.Core`
  - `EventHighway.EventHandlers`
  - `EventHighway.Abstractions`

Connection string example (LocalDB):

```
Server=(localdb)\MSSQLLocalDB;Database=EventHighwayDB;Trusted_Connection=True;MultipleActiveResultSets=true
```

---

## 3. Step-by-Step Setup

### Step 1 – Create the client

Pass your connection string to `EventHighwayClient`. The V2 API is available through the `.V2` property.

```csharp
var client = new EventHighwayClient(
    "Server=(localdb)\\MSSQLLocalDB;Database=EventHighwayDB;" +
    "Trusted_Connection=True;MultipleActiveResultSets=true");
```

---

### Step 2 – Choose and create your handlers

A handler is any class that implements `IEventHandler`. Two built-in options are provided:

| Handler | When to use it |
|---|---|
| `DelegateEventHandler` | You want to run any custom C# code inline or call a method you already have. |
| `RestBearerEventHandler` | You want to POST the event content to an external HTTP endpoint that is secured with OAuth 2.0 client credentials. |

See [Section 4](#4-restbeareventhandler) and [Section 5](#5-delegateeventhandler) for full details.

Each handler needs a stable `Guid` so that listeners can reference it across restarts:

```csharp
var myHandler = new DelegateEventHandler(Guid.NewGuid(), /* … */);
var restHandler = new RestBearerEventHandler(Guid.NewGuid());
```

> **Tip:** In production, store these GUIDs in configuration so they remain the same across deployments. A new GUID means existing listeners in the database can no longer find the handler.

---

### Step 3 – Register handlers with the client

Call `RegisterEventHandler` once per handler. The calls are fluent so they can be chained.

```csharp
client.V2
    .RegisterEventHandler(myHandler)
    .RegisterEventHandler(restHandler);
```

This tells the runtime which handler implementation to invoke when the handler's `Id` is referenced by a listener.

---

### Step 4 – Register an event address

An event address is the channel your events will flow through. Register it once; reuse the `Id` when registering listeners and submitting events.

```csharp
DateTimeOffset now = DateTimeOffset.UtcNow;

var orderPlacedAddress = new EventAddressV2
{
    Id = Guid.NewGuid(),
    Name = "Order Placed",
    Description = "Raised when a customer places a new order.",
    CreatedDate = now,
    UpdatedDate = now
};

await client.V2.EventAddressV2Client.RegisterEventAddressV2Async(orderPlacedAddress);
```

---

### Step 5 – Register listeners on that address

A listener ties an event address to a specific handler. You can register as many listeners as you like on the same address — each one will be invoked every time an event is published to that address.

```csharp
await client.V2.EventListenerV2Client.RegisterEventListenerV2Async(new EventListenerV2
{
    Id = Guid.NewGuid(),
    Name = "Order Confirmation Listener",
    Description = "Sends an order confirmation to the fulfilment API.",
    HandlerId = restHandler.Id,
    HandlerName = restHandler.Name,
    HandlerConfigurations = CreateRestBearerConfigurations(baseUrl, scope: "orders", now),
    EventAddressId = orderPlacedAddress.Id,
    CreatedDate = now,
    UpdatedDate = now
});
```

`HandlerConfigurations` is a list of key/value pairs passed to the handler at invocation time. For `DelegateEventHandler`, pass an empty list because all logic lives inside the delegate itself.

```csharp
await client.V2.EventListenerV2Client.RegisterEventListenerV2Async(new EventListenerV2
{
    Id = Guid.NewGuid(),
    Name = "Order Logger Listener",
    Description = "Logs the order content to the console.",
    HandlerId = myHandler.Id,
    HandlerName = myHandler.Name,
    HandlerConfigurations = new List<HandlerConfiguration>(),
    EventAddressId = orderPlacedAddress.Id,
    CreatedDate = now,
    UpdatedDate = now
});
```

---

### Step 6 – Submit an event

The `Content` field is a free-form string — JSON, CSV, plain text, whatever your handlers expect.

```csharp
var orderEvent = new EventV2
{
    Id = Guid.NewGuid(),
    Content = "{ \"orderId\": \"ORD-001\", \"amount\": 99.95 }",
    EventAddressId = orderPlacedAddress.Id,
    ScheduledDate = DateTimeOffset.UtcNow.AddSeconds(-1), // in the past = fire immediately
    CreatedDate = now,
    UpdatedDate = now
};

await client.V2.EventV2Client.SubmitEventV2Async(orderEvent);
```

Setting `ScheduledDate` to a time in the past causes the event to be treated as immediately eligible. Setting it to a future time schedules it for later.

---

### Step 7 – Fire pending events

After submission, events sit in the database with a `Pending` status. Call `FireScheduledPendingEventV2sAsync` to dispatch all events that are due.

```csharp
await client.V2.EventV2Client.FireScheduledPendingEventV2sAsync();
```

This is typically run on a timer or background job in a real application. In this sample it is called once after all events have been submitted.

---

### Step 8 – Read the results

After firing, the outcome of every handler invocation is stored as a `ListenerEventV2`. Retrieve them all and filter by the event you care about.

```csharp
IQueryable<ListenerEventV2> all =
    await client.V2.ListenerEventV2Client.RetrieveAllListenerEventV2sAsync();

foreach (ListenerEventV2 result in all.Where(r => r.EventId == orderEvent.Id))
{
    Console.WriteLine($"  Status   : {result.Status}");
    Console.WriteLine($"  Code     : {result.ResponseCode}");
    Console.WriteLine($"  Message  : {result.ResponseMessage}");
    Console.WriteLine($"  Response : {result.Response}");
}
```

`result.Status` is a `ListenerEventStatusV2` enum with three possible values:

| Value | Meaning |
|---|---|
| `Pending` | The event has been submitted but not yet dispatched. |
| `Success` | The handler ran and returned `IsSuccess = true`. |
| `Error` | The handler threw an exception or returned `IsSuccess = false`. |

The `Response`, `ResponseCode`, and `ResponseMessage` fields come directly from the `EventHandlerResult` that the handler returned — see [Section 5.3](#53-reading-what-the-handler-returned) for the full picture.

---

## 4. RestBearerEventHandler

`RestBearerEventHandler` posts the event content to an external HTTP endpoint that is protected by an OAuth 2.0 **client credentials** flow.

**What it does internally:**

1. Reads configuration from the listener's `HandlerConfigurations`.
2. Sends a `POST` to `TokenUrl` with `client_id`, `client_secret`, `scope`, and `grant_type` to obtain a bearer token.
3. Sends a `POST` to `Url` with the event `Content` as the request body, including the bearer token in the `Authorization` header.
4. Returns an `EventHandlerResult` with the HTTP status code, reason phrase, and response body.

### Required configuration keys

When registering a listener that uses `RestBearerEventHandler`, every key below must be present in `HandlerConfigurations`:

| Key | Example value | Description |
|---|---|---|
| `Url` | `https://api.example.com/events` | The endpoint to POST the event to. |
| `TokenUrl` | `https://auth.example.com/token` | The OAuth token endpoint. |
| `ClientId` | `my-service` | OAuth client identifier. |
| `ClientSecret` | `s3cr3t` | OAuth client secret. |
| `Scope` | `orders` | The OAuth scope to request. |
| `GrantType` | `client_credentials` | Must be `client_credentials`. |

### Helper method

A helper that builds the configuration list keeps the listener registration readable:

```csharp
private static List<HandlerConfiguration> CreateRestBearerConfigurations(
    string baseUrl,
    string scope,
    DateTimeOffset now) =>
    new List<HandlerConfiguration>
    {
        new() { Id = Guid.NewGuid(), Name = "Url",
            Value = $"{baseUrl}/events", CreatedDate = now, UpdatedDate = now },
        new() { Id = Guid.NewGuid(), Name = "TokenUrl",
            Value = $"{baseUrl}/token", CreatedDate = now, UpdatedDate = now },
        new() { Id = Guid.NewGuid(), Name = "ClientId",
            Value = "my-service", CreatedDate = now, UpdatedDate = now },
        new() { Id = Guid.NewGuid(), Name = "ClientSecret",
            Value = "s3cr3t", CreatedDate = now, UpdatedDate = now },
        new() { Id = Guid.NewGuid(), Name = "Scope",
            Value = scope, CreatedDate = now, UpdatedDate = now },
        new() { Id = Guid.NewGuid(), Name = "GrantType",
            Value = "client_credentials", CreatedDate = now, UpdatedDate = now },
    };
```

### Listener registration

```csharp
var restHandler = new RestBearerEventHandler(Guid.NewGuid());

// Register the handler
client.V2.RegisterEventHandler(restHandler);

// Register the listener
await client.V2.EventListenerV2Client.RegisterEventListenerV2Async(new EventListenerV2
{
    Id = Guid.NewGuid(),
    Name = "Fulfilment API Listener",
    Description = "Posts the order to the fulfilment API.",
    HandlerId = restHandler.Id,
    HandlerName = restHandler.Name,
    HandlerConfigurations = CreateRestBearerConfigurations(
        "https://api.example.com", scope: "orders", now),
    EventAddressId = orderPlacedAddress.Id,
    CreatedDate = now,
    UpdatedDate = now
});
```

---

## 5. DelegateEventHandler

`DelegateEventHandler` lets you supply any C# function as the handler. No HTTP plumbing is built in — you decide exactly what happens.

The delegate signature is:

```csharp
Func<string, IReadOnlyDictionary<string, string>, CancellationToken, ValueTask<EventHandlerResult>>
//   ^^^^^^   ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^  ^^^^^^^^^^^^^^^^^^^   ^^^^^^^^^^^^^^^^^^^^
//   content  handler configuration key/value pairs      cancellation          what you return
```

Because `DelegateEventHandler` declares `RequiredParams` as empty, listener registrations always use an empty `HandlerConfigurations` list.

---

### 5.1 Inline lambda

The simplest form — logic lives directly inside the `new DelegateEventHandler(...)` call.

```csharp
var sumHandler = new DelegateEventHandler(
    Guid.NewGuid(),
    (content, _, cancellationToken) =>
    {
        // content is the string payload from the submitted EventV2
        int sum = content.Split(',')
            .Select(p => int.TryParse(p.Trim(), out int n) ? n : 0)
            .Sum();

        Console.WriteLine($"[Sum Handler] {content} => {sum}");

        // Return an EventHandlerResult — every field you set here
        // ends up on the ListenerEventV2 record you can query later.
        return ValueTask.FromResult(new EventHandlerResult
        {
            IsSuccess = true,
            Response = sum.ToString(),   // stored in ListenerEventV2.Response
            ResponseCode = "200",        // stored in ListenerEventV2.ResponseCode
            ResponseMessage = "OK"       // stored in ListenerEventV2.ResponseMessage
        });
    });
```

Register it the same way as any other handler:

```csharp
client.V2.RegisterEventHandler(sumHandler);

await client.V2.EventListenerV2Client.RegisterEventListenerV2Async(new EventListenerV2
{
    Id = Guid.NewGuid(),
    Name = "Sum Listener",
    Description = "Sums the comma-separated numbers in the event content.",
    HandlerId = sumHandler.Id,
    HandlerName = sumHandler.Name,
    HandlerConfigurations = new List<HandlerConfiguration>(),
    EventAddressId = orderPlacedAddress.Id,
    CreatedDate = now,
    UpdatedDate = now
});
```

---

### 5.2 Calling a method on your class

For anything non-trivial, extract the logic into a named method and have the delegate call it. This keeps `Main` readable and makes the method independently testable.

```csharp
// The delegate — a one-liner that delegates to the real method
var restViaDelegate = new DelegateEventHandler(
    Guid.NewGuid(),
    (content, _, cancellationToken) =>
        PostWithBearerTokenAsync(content, "https://api.example.com", cancellationToken));
```

The method itself contains all the logic:

```csharp
private static async ValueTask<EventHandlerResult> PostWithBearerTokenAsync(
    string content,
    string baseUrl,
    CancellationToken cancellationToken)
{
    using var http = new HttpClient();

    // 1. Obtain bearer token
    var tokenPayload = new FormUrlEncodedContent(new Dictionary<string, string>
    {
        ["client_id"]     = "my-service",
        ["client_secret"] = "s3cr3t",
        ["scope"]         = "orders",
        ["grant_type"]    = "client_credentials"
    });

    HttpResponseMessage tokenResponse =
        await http.PostAsync($"{baseUrl}/token", tokenPayload, cancellationToken);

    string tokenJson = await tokenResponse.Content.ReadAsStringAsync(cancellationToken);

    string token = JsonDocument.Parse(tokenJson)
        .RootElement.GetProperty("access_token").GetString() ?? string.Empty;

    // 2. POST event content with the bearer token
    http.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", token);

    var body = new StringContent(content, Encoding.UTF8, "application/json");

    HttpResponseMessage response =
        await http.PostAsync($"{baseUrl}/events", body, cancellationToken);

    string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

    // 3. Return a result — fields flow through to ListenerEventV2
    return new EventHandlerResult
    {
        IsSuccess = response.IsSuccessStatusCode,
        Response = responseBody,
        ResponseCode = ((int)response.StatusCode).ToString(),
        ResponseMessage = response.ReasonPhrase ?? string.Empty
    };
}
```

The delegate calls `PostWithBearerTokenAsync` directly — no wrapping, no extra indirection.

---

### 5.3 Reading what the handler returned

Every field your handler puts into `EventHandlerResult` is captured in the `ListenerEventV2` record that EventHighway creates after the handler runs.

```
EventHandlerResult.IsSuccess       → ListenerEventV2.Status  (true = Success, false = Error)
EventHandlerResult.Response        → ListenerEventV2.Response
EventHandlerResult.ResponseCode    → ListenerEventV2.ResponseCode
EventHandlerResult.ResponseMessage → ListenerEventV2.ResponseMessage
```

To read these values, retrieve all listener events and filter by the event ID:

```csharp
IQueryable<ListenerEventV2> all =
    await client.V2.ListenerEventV2Client.RetrieveAllListenerEventV2sAsync();

foreach (ListenerEventV2 result in all.Where(r => r.EventId == orderEvent.Id))
{
    Console.WriteLine($"  Status   : {result.Status}");        // Pending | Success | Error
    Console.WriteLine($"  Code     : {result.ResponseCode}");  // e.g. "200"
    Console.WriteLine($"  Message  : {result.ResponseMessage}");// e.g. "OK"
    Console.WriteLine($"  Response : {result.Response}");      // e.g. "144" (the computed sum)
}
```

Sample console output for the sum handler from [Section 5.1](#51-inline-lambda):

```
── Results for event 3f2a1b... ──
  Status   : Success
  Code     : 200
  Message  : OK
  Response : 144
```

This is how you close the loop — publish an event, fire it, and then read back exactly what each of your handlers computed or returned from an external API.

---

## 6. Building a Custom Event Handler

If neither `RestBearerEventHandler` nor `DelegateEventHandler` fits your needs, you can build your own. A custom handler is a plain C# class that implements `IEventHandler` and follows a specific exception-handling contract so that the EventHighway client can correctly classify any errors that occur.

This section uses `RestBearerEventHandler` as the reference implementation to show the full pattern.

---

### 6.1 The IEventHandler contract

Every handler must implement this interface from `EventHighway.Abstractions`:

```csharp
public interface IEventHandler
{
    Guid Id { get; }
    string Name { get; }
    IEnumerable<string> RequiredParams { get; }

    ValueTask<EventHandlerResult> HandleAsync(
        string content,
        IReadOnlyDictionary<string, string> handlerParams,
        CancellationToken cancellationToken = default);
}
```

| Member | What to put here |
|---|---|
| `Id` | A stable `Guid` that uniquely identifies this handler. Store it in configuration so it stays the same across restarts. |
| `Name` | A human-readable name. Using `nameof(YourHandlerClass)` is the convention. |
| `RequiredParams` | The names of every key your handler reads from `handlerParams`. EventHighway validates that all required keys are present before calling `HandleAsync`. Return `Array.Empty<string>()` if your handler needs no configuration. |
| `HandleAsync` | The method that does the real work. It must return an `EventHandlerResult` and must follow the try/catch structure described below. |

---

### 6.2 Step 1 – Define your exception classes

Every custom handler needs **three exception classes** — one for each category of error. They must inherit from `Xeption` (the base exception class used throughout EventHighway) **and** implement the corresponding marker interface from `EventHighway.Abstractions`.

The three interfaces and what they represent:

| Interface | When to throw it |
|---|---|
| `IEventHandlerValidationException` | The input or configuration is invalid — bad data that the caller can fix. |
| `IEventHandlerDependencyException` | A downstream dependency (database, API, network) failed — not the caller's fault. |
| `IEventHandlerServiceException` | An unexpected internal error occurred that does not fit either category above. |

Create one class per interface. The pattern is identical for all three — only the class name and interface differ:

```csharp
using System.Collections;
using EventHighway.Abstractions.EventHandlers.Exceptions;
using Xeptions;

// Thrown when input or configuration is invalid
public class SmtpEventHandlerValidationException : Xeption, IEventHandlerValidationException
{
    public SmtpEventHandlerValidationException(
        string message, Xeption innerException, IDictionary data)
        : base(message, innerException, data) { }
}

// Thrown when a dependency (e.g. the SMTP server) fails
public class SmtpEventHandlerDependencyException : Xeption, IEventHandlerDependencyException
{
    public SmtpEventHandlerDependencyException(
        string message, Xeption innerException, IDictionary data)
        : base(message, innerException, data) { }
}

// Thrown for any other unexpected error
public class SmtpEventHandlerServiceException : Xeption, IEventHandlerServiceException
{
    public SmtpEventHandlerServiceException(
        string message, Xeption innerException, IDictionary data)
        : base(message, innerException, data) { }
}
```

> **Why `Xeption`?**  
> `Xeption` extends `Exception` with support for structured error data (via `IDictionary data`). EventHighway's internal services use `Xeption` throughout, so your exceptions must also derive from it to be compatible.

---

### 6.3 Step 2 – Implement the handler class

The structure of a handler class always follows this shape, taken directly from `RestBearerEventHandler`:

```csharp
public class SmtpEventHandler : IEventHandler
{
    public SmtpEventHandler(Guid id)
    {
        this.Id = id;
    }

    public Guid Id { get; }
    public string Name => nameof(SmtpEventHandler);

    // Declare every key your handler reads from handlerParams.
    // EventHighway validates these are present before calling HandleAsync.
    public IEnumerable<string> RequiredParams =>
        new[] { "SmtpHost", "SmtpPort", "FromAddress", "ToAddress" };

    public async ValueTask<EventHandlerResult> HandleAsync(
        string content,
        IReadOnlyDictionary<string, string> handlerParams,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Your real logic goes here.
            // handlerParams contains the values from the listener's HandlerConfigurations.
            string host    = handlerParams["SmtpHost"];
            string port    = handlerParams["SmtpPort"];
            string from    = handlerParams["FromAddress"];
            string to      = handlerParams["ToAddress"];

            await SendEmailAsync(host, int.Parse(port), from, to, content, cancellationToken);

            return new EventHandlerResult
            {
                IsSuccess = true,
                Response = "Email sent.",
                ResponseCode = "200",
                ResponseMessage = "OK"
            };
        }
        catch (SmtpValidationException smtpValidationException)
        {
            // Catch inner service exceptions and re-wrap them in a handler exception
            // that carries the correct marker interface.
            throw CreateValidationException(smtpValidationException.InnerException as Xeption);
        }
        catch (SmtpDependencyException smtpDependencyException)
        {
            throw CreateDependencyException(smtpDependencyException.InnerException as Xeption);
        }
        catch (Exception exception)
        {
            throw CreateServiceException(exception.InnerException as Xeption);
        }
    }

    private static SmtpEventHandlerValidationException CreateValidationException(
        Xeption innerException) =>
        new SmtpEventHandlerValidationException(
            message: "SMTP event handler validation error occurred, fix errors and try again.",
            innerException,
            data: innerException.Data);

    private static SmtpEventHandlerDependencyException CreateDependencyException(
        Xeption innerException) =>
        new SmtpEventHandlerDependencyException(
            message: "SMTP event handler dependency error occurred, contact support.",
            innerException,
            data: innerException.Data);

    private static SmtpEventHandlerServiceException CreateServiceException(
        Xeption innerException) =>
        new SmtpEventHandlerServiceException(
            message: "SMTP event handler service error occurred, contact support.",
            innerException,
            data: innerException.Data);
}
```

Notice the pattern in `HandleAsync`:

- The `try` block contains only the real work.
- Each `catch` block catches a specific inner exception, **unwraps its `InnerException`**, and re-throws it wrapped in the appropriate handler exception.
- The final `catch (Exception exception)` is the safety net for anything unexpected.

The `InnerException as Xeption` unwrapping is deliberate — it strips one layer of wrapping so the data and message of the root cause travel through cleanly.

---

### 6.4 Step 3 – Use handlerParams to pass configuration

`handlerParams` is a read-only dictionary built from the `HandlerConfigurations` list on the listener registration. It is the mechanism for passing per-listener configuration into your handler at runtime — think of it as the handler's settings object.

**How it works end-to-end:**

1. You declare which keys your handler needs via `RequiredParams`.
2. When registering a listener, the caller provides `HandlerConfigurations` with a `HandlerConfiguration` entry for each required key.
3. EventHighway validates that every key in `RequiredParams` is present before calling `HandleAsync`.
4. Inside `HandleAsync`, you read values with `handlerParams["KeyName"]`.

**Listener registration — providing the configuration:**

```csharp
await client.V2.EventListenerV2Client.RegisterEventListenerV2Async(new EventListenerV2
{
    Id = Guid.NewGuid(),
    Name = "Order Confirmation Email Listener",
    Description = "Sends an order confirmation email via SMTP.",
    HandlerId = smtpHandler.Id,
    HandlerName = smtpHandler.Name,
    HandlerConfigurations = new List<HandlerConfiguration>
    {
        new() { Id = Guid.NewGuid(), Name = "SmtpHost",     Value = "smtp.example.com",    CreatedDate = now, UpdatedDate = now },
        new() { Id = Guid.NewGuid(), Name = "SmtpPort",     Value = "587",                 CreatedDate = now, UpdatedDate = now },
        new() { Id = Guid.NewGuid(), Name = "FromAddress",  Value = "no-reply@example.com",CreatedDate = now, UpdatedDate = now },
        new() { Id = Guid.NewGuid(), Name = "ToAddress",    Value = "orders@example.com",  CreatedDate = now, UpdatedDate = now },
    },
    EventAddressId = orderPlacedAddress.Id,
    CreatedDate = now,
    UpdatedDate = now
});
```

**Reading the values inside the handler:**

```csharp
string host = handlerParams["SmtpHost"];     // "smtp.example.com"
string port = handlerParams["SmtpPort"];     // "587"
string from = handlerParams["FromAddress"];  // "no-reply@example.com"
string to   = handlerParams["ToAddress"];    // "orders@example.com"
```

This design means the same handler class can behave differently for different listeners — one listener could send to `orders@example.com`, another to `support@example.com`, just by registering two listeners with different configuration values.

---

### 6.5 Step 4 – Register and use the handler

A custom handler is registered and used exactly the same way as the built-in ones:

```csharp
var smtpHandler = new SmtpEventHandler(Guid.NewGuid());

client.V2.RegisterEventHandler(smtpHandler);
```

After that, any listener whose `HandlerId` matches `smtpHandler.Id` will route events through `SmtpEventHandler.HandleAsync`.

---

### 6.6 Why the exception interfaces matter

When `HandleAsync` throws, EventHighway's internal `EventCallV2Service` catches it using the marker interfaces — not the concrete class names. This is the critical reason your exceptions must implement `IEventHandlerValidationException`, `IEventHandlerDependencyException`, or `IEventHandlerServiceException`.

The internal catch logic looks like this (simplified):

```csharp
catch (Exception exception) when (exception is IEventHandlerValidationException)
{
    // Treated as a validation error — the event call fails with a dependency-validation exception.
    // This signals that the caller or the listener configuration needs to be corrected.
}
catch (Exception exception) when (exception is IEventHandlerDependencyException)
{
    // Treated as a critical dependency failure — logged and escalated.
}
catch (Exception exception) when (exception is IEventHandlerServiceException)
{
    // Treated as an internal service error — logged as a dependency exception.
}
catch (Exception serviceException)
{
    // Anything that does not carry one of the three interfaces falls here
    // and is recorded as an unexpected service error.
}
```

If your exception does **not** implement one of the three interfaces, it falls through to the final `catch` block and is treated as a generic unexpected error. The error classification and logging will be wrong, and it becomes much harder to diagnose whether the problem was bad input, a network failure, or a bug in the handler.

**Summary of the exception contract:**

| Your exception implements | EventHighway treats it as | Typical cause |
|---|---|---|
| `IEventHandlerValidationException` | Dependency-validation error | Missing config key, null content, invalid input |
| `IEventHandlerDependencyException` | Critical dependency failure | External API down, network timeout, database error |
| `IEventHandlerServiceException` | Service error | Unexpected bug inside the handler |
| *(nothing)* | Unexpected service error | Falls through — avoid this |

---

## 7. Future Enhancements

### 7.1 Listener filter criteria

Today, every listener registered on an event address is invoked for every event published to that address. A planned enhancement would allow each listener to declare **filter criteria** — conditions that must be satisfied before the listener is invoked at all.

#### The problem this solves

Consider an event address called `New Release` where a streaming platform publishes every new title the moment it becomes available. Every release — regardless of genre, language, or series — is published to this single address.

Without filtering, every listener on that address receives every release. You might register a listener that sends a push notification to a subscriber who only watches action films, but right now that listener is invoked for every documentary, romance, and kids' show too. The handler has to inspect the content and silently discard anything it does not care about, or worse — it fires an external REST call that the receiving API then has to reject.

This becomes increasingly costly as the volume of events grows. An address with thousands of daily releases would invoke every listener thousands of times, the vast majority of which do nothing useful.

#### How filter criteria would work

Each listener would carry an optional list of filter rules alongside its existing `HandlerConfigurations`. A filter rule would express a condition on the event content — for example:

- `Genre = "Action"`
- `SeriesName = "Yellowstone"`
- `ReleaseType = "NewEpisode"`

Before invoking a listener's handler, EventHighway would evaluate the filter rules against the incoming event. The handler is only called if the event satisfies all of the listener's filters. If none of the rules match, the event is silently skipped for that listener and no `ListenerEventV2` record is created, keeping the audit log clean.

A listener with no filter rules would continue to behave exactly as it does today — it receives every event on the address.

#### What this looks like in practice

Imagine three listeners registered on the same `New Release` address, each targeting a different audience:

| Listener | Filter | Handler |
|---|---|---|
| Action Fan Notifier | `Genre = "Action"` | Sends a push notification |
| Expanse Watcher | `SeriesName = "Yellowstone"` | Posts to a webhook |
| All Releases Logger | *(no filter)* | Writes every release to a log |

When a new action film is published, only the Action Fan Notifier and the All Releases Logger are invoked. When a new episode of Yellowstone drops, only the Expanse Watcher and the All Releases Logger are invoked. Each listener's handler only fires when there is something relevant for it to do.

#### Benefits

- **Reduced unnecessary handler invocations.** Handlers are only called when the event is genuinely relevant to that listener.
- **External APIs are not spammed.** A REST-based listener that forwards events to a third-party endpoint will only make HTTP calls for events that meet its criteria, rather than for every event on the address.
- **Cleaner audit trail.** Because skipped events do not produce `ListenerEventV2` records, the results table only contains meaningful entries — things that were actually actioned.
- **Simpler handler logic.** Handlers no longer need to inspect the event content and decide whether to act. That responsibility moves to the declarative filter, and the handler can focus entirely on doing its job.
