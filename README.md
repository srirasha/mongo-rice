
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
- search
- paginated search
- update

See all the features [here](src/Library/Repositories/IMongoRiceRepository.cs).

# Getting started

If you are using IOC, declare MongoRice like this:

```C#
builder.Services.AddMongoRice(new MongoConfiguration(){ ConnectionString = "myGreatConnectionString", Database = "mySuperDatabase" });
```
That will be useful to automatically inject all your IMongoRiceRepository without declaring them on the services handler.

# Usage

1. Create a class that will represent your document on Mongo:
```C#
[Collection("avatars")]
public class AvatarDocument : Document
{
    public string Description { get; set; }

    public string Name { get; set; }
}
```

2. Declare your instance of your repository:
- by instance injection:
```C#
public class GetAvatarByIdQueryHandler : IRequestHandler<GetAvatarByIdQuery, Maybe<AvatarDocument>>
{
    private readonly IMongoRiceRepository<AvatarDocument> _avatars;

    public GetAvatarByIdQueryHandler(IMongoRiceRepository<AvatarDocument> avatars)
    {
        _avatars = avatars;
    }

    public async Task<MaybeAvatarDocument>> Handle(GetAvatarByIdQuery request, CancellationToken cancellationToken)
    {
        return await _avatars.FindById(request.Id, cancellationToken);
    }
}
```
 - by manually create an instance of a collection:

```C#
MongoRiceRepository<AvatarDocument> avatars = new(new MongoConfiguration() { ConnectionString = "myGreatConnectionString", Database = "mySuperDatabase" });

Maybe<AvatarDocument> maybeAvatar = await avatars.FindById("62779e4718dd7e243339b187");
```
That's it!

# Thank you!
