# mongo-rice

Mongo-rice is a generic Mongo repository library built for .NET applications.

## Table of Contents

* [Features](#features)
* [Getting started](#getting-started)
* [Usage](#usage)

# Features

Mongo-rice allow you to execute the main actions on a collection:

- create
- delete
- filter
- update

See all the features [here](src/Library/Repositories/IMongoRiceRepository.cs).

# Getting started

If you are using IOC, inject MongoRice like this:

```C#
builder.Services.AddMongoRice(new MongoConfiguration(){ ConnectionString = "myGreatConnectionString", Database = "mySuperDatabase" });
```

# Usage

Create a class that will represent your document on Mongo:
```C#
[Collection("avatars")]
public class AvatarDocument : Document
{
    public string Description { get; set; }

    public string Name { get; set; }
}
```

Inject a IMongoRiceRepository on your class:
```C#
public class GetAvatarByIdQueryHandler : IRequestHandler<GetAvatarByIdQuery, AvatarDocument>>
{
    private readonly IMongoRiceRepository<AvatarDocument> _avatars;

    public GetAvatarByIdQueryHandler(IMongoRiceRepository<AvatarDocument> avatars)
    {
        _avatars = avatars;
    }

    public async Task<AvatarDocument>> Handle(GetAvatarByIdQuery request, CancellationToken cancellationToken)
    {
        return await _avatars.FindById(request.Id, cancellationToken);
    }
}
```

That's it!

# Thank you!
