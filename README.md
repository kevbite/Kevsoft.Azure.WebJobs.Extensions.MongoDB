# Kevsoft.Azure.WebJobs.Extensions.MongoDB

WebJobs/Azure Functions trigger providing reading and writing MongoDB documents directly from your functions

[![install from nuget](http://img.shields.io/nuget/v/Kevsoft.Azure.WebJobs.Extensions.MongoDB.svg?style=flat-square)](https://www.nuget.org/packages/Kevsoft.Azure.WebJobs.Extensions.MongoDB)
[![downloads](http://img.shields.io/nuget/dt/Kevsoft.Azure.WebJobs.Extensions.MongoDB.svg?style=flat-square)](https://www.nuget.org/packages/Kevsoft.Azure.WebJobs.Extensions.MongoDB)
[![Build status](https://ci.appveyor.com/api/projects/status/c8y4icg4accsvv5e/branch/master?svg=true)](https://ci.appveyor.com/project/kevbite/kevsoft-azure-webjobs-extensions-mongodb/branch/master)


## Get started

### Install the NuGet Package

You can install the package using the standard dotnet CLI:

```bash
dotnet add package Kevsoft.Azure.WebJobs.Extensions.MongoDB
```

or by using the package manager within Visual Studio:

```powershell
PM> Install-Package Kevsoft.Azure.WebJobs.Extensions.MongoDB
```

### Configure Bindings

The package supports lots of binding types, For a full list see the example project within this repository.

#### Native MongoDB Objects

We support binding to the `MongoClient`, `IMongoDatabase` and `IMongoCollection<>`:

```csharp
[FunctionName("NativeObjects")]
public static async Task<IActionResult> RunNativeObjects(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "native")] HttpRequest req,

    [MongoDb(ConnectionStringSetting = "MongoDbUrl")]
    MongoClient client,

    [MongoDb("test", ConnectionStringSetting = "MongoDbUrl")]
    IMongoDatabase database,

    [MongoDb("test", "test", ConnectionStringSetting = "MongoDbUrl")]
    IMongoCollection<BsonDocument> collection)
{
    // Do something with `client`, `database` or `collection`.
    
    return new OkObjectResult();
}
```

#### Bind by ID

We can also bind directly to a document within your database:

```csharp
[FunctionName("QueryIdByObjectId")]
public static IActionResult RunByObjectId(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{database}/{collection}/{id}")] HttpRequest req,
    [MongoDb("{database}", "{collection}", "{id}", ConnectionStringSetting = "MongoDbUrl")]
    BsonDocument document)
{
    var value = document.ToJson();

    return new OkObjectResult(value);
}
```

If we're not using an `ObjectId` as our `_id` we can set the type on the binding:
```csharp
[MongoDb("{database}", "{collection}", "{id}", ConnectionStringSetting = "MongoDbUrl", IdType = typeof(int))]
```

#### Adding Documents

Adding documents is done by binding to an `IAsyncCollector<>`:

```csharp
[FunctionName("InsertFunction")]
public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "{database}/{collection}")] HttpRequest req,
    [MongoDb("{database}", "{collection}", ConnectionStringSetting = "MongoDbUrl")] IAsyncCollector<BsonDocument> documents)
{
    var json = await req.ReadAsStringAsync().ConfigureAwait(false);

    var document = BsonDocument.Parse(json);

    await documents.AddAsync(document)
        .ConfigureAwait(false);

    return new AcceptedResult();
}
```

## Contribute

1. Fork
1. Hack!
1. Pull Request
