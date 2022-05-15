
# mongo-rice

MongoRice is a generic Mongo repository library built for .NET applications.

# Features

MongoRice allow you to execute the main actions on a collection:

- create
- delete
- search
- paginated search
- update

See all the features [here](src/Library/Repositories/IMongoRiceRepository.cs).

# Getting started

If you are using IOC, you can declare MongoRice like this:

```C#
builder.Services.AddMongoRice(new MongoConfiguration(){ ConnectionString = "myGreatConnectionString", Database = "mySuperDatabase" });
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
public class GetPlayerProfileByIdQueryHandler : IRequestHandler<GetPlayerProfileByIdQuery, Maybe<PlayerProfileDocument>>
{
    private readonly IMongoRiceRepository<PlayerProfileDocument> _playerProfiles;

    public GetPlayerProfileByIdQueryHandler(IMongoRiceRepository<PlayerProfileDocument> playerProfiles)
    {
        _playerProfiles = playerProfiles;
    }

    public async Task<Maybe<PlayerProfileDocument>> Handle(GetPlayerProfileByIdQuery request, CancellationToken cancellationToken)
    {
        return await _playerProfiles.FindById(request.Id, cancellationToken);
    }
}
```
 - by manual instanciation:

```C#
MongoRiceRepository<PlayerProfileDocument> playerProfiles = new(new MongoConfiguration() { ConnectionString = "myGreatConnectionString", Database = "mySuperDatabase" });

Maybe<PlayerProfileDocument> maybePlayerProfiles = await playerProfiles.FindById("62779e4718dd7e243339b187");
```

# Examples

## Filtered search:

Get all the players with a level greater or equal to 100:

```C#
IEnumerable<PlayerProfileDocument> searchResult = await _playerProfiles.Find(Builders<PlayerProfileDocument>.Filter.Gte(profile => profile.Level, 100), cancellationToken);
```

## Filtered and ordered search:

Get all the players with a level greater or equal to 100 ordered by level descending:

```C#
PaginatedResult<PlayerProfileDocument> paginatedResult =
await _playerProfiles.Find(Builders<PlayerProfileDocument>.Filter.Gte(profile => profile.Level, 100),
                           Builders<PlayerProfileDocument>.Sort.Descending(profile => profile.Level),
                           cancellationToken);
```


## Paginated search:

Get the first page of players profiles with a level greater or equal to 100, ordered by level descending, paginated by 10 elements:

```C#
PaginatedResult<PlayerProfileDocument> paginatedResult =
await _playerProfiles.Find(Builders<PlayerProfileDocument>.Filter.Gte(profile => profile.Level, 100),
                           Builders<PlayerProfileDocument>.Sort.Descending(profile => profile.Level),
                           1,
                           10,
                           cancellationToken);
```