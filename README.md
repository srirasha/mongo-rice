
# mongo-rice

MongoRice is a generic Mongo repository library built for .NET applications.

# Installation

MongoRice is available on [Nuget](https://www.nuget.org/packages/MongoRice).

# Features

MongoRice allows you to execute the main actions on a collection:

- create
- delete
- search
- paginated search
- update

See all the features [here](src/MongoRice/Repositories/IMongoRiceRepository.cs).

# Getting started

If you are using IOC, you can declare MongoRice like this:

```C#
builder.Services.AddMongoRice(new MongoConfiguration(){ ConnectionString = "myConnectionString", Database = "myDatabase" });
```
That will be useful to automatically inject all your IMongoRiceRepository without declaring them on the services handler.

# Usage

1. Create a class that will represent your document on Mongo:

```C#
[Collection("player-profiles")]
public class PlayerProfileDocument : Document
{
    public string Level { get; set; }

    public string Name { get; set; }
}
```

2. Declare your instance of your repository:

- by dependency injection:

```C#
public class PlayerProfileService : IPlayerProfileService
{
    private readonly IMongoRiceRepository<PlayerProfileDocument> _playerProfiles;

    public PlayerProfileService(IMongoRiceRepository<PlayerProfileDocument> playerProfiles)
    {
        _playerProfiles = playerProfiles;
    }

    public async Task<Maybe<PlayerProfile>> GetById(string id, CancellationToken cancellationToken = default)
    {
        return await _playerProfiles.FindById(id, cancellationToken);
    }
}
```

 - by manual instanciation:

```C#
MongoRiceRepository<PlayerProfileDocument> playerProfiles = new(new MongoConfiguration("myConnectionString", "myDatabase"));

Maybe<PlayerProfile> maybePlayerProfile = await playerProfiles.FindById("62779e4718dd7e243339b187");
```

# Examples

## Filtered search:

Get all the players with a level greater or equal to 100:

```C#
IEnumerable<PlayerProfile> searchResult = await _playerProfiles.Find(profile => profile.Level >= 100), cancellationToken);
```

## Filtered and ordered search:

Get all the players with a level greater or equal to 100 ordered by level descending:

```C#
IEnumerable<PlayerProfile> searchResult =
await _playerProfiles.Find(profile => profile.Level >= 100,
                           Builders<PlayerProfileDocument>.Sort.Descending(profile => profile.Level),
                           cancellationToken);
```

## Paginated search:

Get the first page of players profiles with a level greater or equal to 100, ordered by level descending, paginated by 10 elements:

```C#
PaginatedResult<PlayerProfile> paginatedSearchResult =
await _playerProfiles.Find(profile => profile.Level >= 100,
                           Builders<PlayerProfileDocument>.Sort.Descending(profile => profile.Level),
                           1,
                           10,
                           cancellationToken);
```