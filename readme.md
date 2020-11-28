Features

- Converting Enums
- Converting Interfaces
- Converting Types to Classes
- Apply TypeExtension to Classes if Type(Class) with the defined name exists othervise creates new class
- Automatically implements Interface fields inside Class
- other coming soon

# WORK IN PROGRESS

## Current working example

At this point it converts this schema

```graphql
schema {
    query: Query
    mutation: Mutation
}
# The query type, represents all of the entry points into our object graph
type Query {
    hero(episode: Episode): Character
    reviews(episode: Episode!): [Review]
    search(text: String): [SearchResult]
    character(id: ID!): Character
    droid(id: ID!): Droid
    human(id: ID!): Human
    starship(id: ID!): Starship
}
# The mutation type, represents all updates we can make to our data
type Mutation {
    createReview(episode: Episode, review: ReviewInput!): Review
}
# The episodes in the Star Wars trilogy
enum Episode {
    # Star Wars Episode IV: A New Hope, released in 1977.
    NEWHOPE
    # Star Wars Episode V: The Empire Strikes Back, released in 1980.
    EMPIRE
    # Star Wars Episode VI: Return of the Jedi, released in 1983.
    JEDI
}
# A character from the Star Wars universe
interface Character {
    # The ID of the character
    id: ID!
    # The name of the character
    name: String!
    # The friends of the character, or an empty list if they have none
    friends: [Character]
    # The friends of the character exposed as a connection with edges
    friendsConnection(first: Int, after: ID): FriendsConnection!
    # The movies this character appears in
    appearsIn: [Episode]!
}
# Units of height
enum LengthUnit {
    # The standard unit around the world
    METER
    # Primarily used in the United States
    FOOT
    # Ancient unit used during the Middle Ages
    CUBIT @deprecated(reason: ""Test deprecated enum case"")
}
# A humanoid creature from the Star Wars universe
type Human implements Character @entity{
    # The ID of the human
    id: ID!
    # What this human calls themselves
    name: String!
    # The home planet of the human, or null if unknown
    homePlanet: String
    # Height in the preferred unit, default is meters
    height(unit: LengthUnit = METER): Float
    # Mass in kilograms, or null if unknown
    mass: Float
    # This human's friends, or an empty list if they have none
    friends: [Character] @relation
    # The friends of the human exposed as a connection with edges
    friendsConnection(first: Int, after: ID): FriendsConnection!
    # The movies this human appears in
    appearsIn: [Episode]! @relation
    # A list of starships this person has piloted, or an empty list if none
    starships: [Starship] @relation
}
# An autonomous mechanical character in the Star Wars universe
type Droid implements Character @entity{
    # The ID of the droid
    id: ID!
    # What others call this droid
    name: String!
    # This droid's friends, or an empty list if they have none
    friends: [Character] @relation
    # The friends of the droid exposed as a connection with edges
    friendsConnection(first: Int, after: ID): FriendsConnection!
    # The movies this droid appears in
    appearsIn: [Episode]! @relation
    # This droid's primary function
    primaryFunction: String
}
# A connection object for a character's friends
type FriendsConnection @entity{
    # The total number of friends
    totalCount: Int
    # The edges for each of the character's friends.
    edges: [FriendsEdge]
    # A list of the friends, as a convenience when edges are not needed.
    friends: [Character] @relation
    # Information for paginating this connection
    pageInfo: PageInfo!
}
# An edge object for a character's friends
type FriendsEdge @entity{
    # A cursor used for pagination
    cursor: ID!
    # The character represented by this friendship edge
    node: Character
}
# Information for paginating this connection
type PageInfo @entity{
    startCursor: ID
    endCursor: ID
    hasNextPage: Boolean!
}
# Represents a review for a movie
type Review @entity{
    # The number of stars this review gave, 1-5
    stars: Int!
    # Comment about the movie
    commentary: String
}
# The input object sent when someone is creating a new review
input ReviewInput {
    # 0-5 stars
    stars: Int!
    # Comment about the movie, optional
    commentary: String
    # Favorite color, optional
    favorite_color: ColorInput
}
# The input object sent when passing in a color
input ColorInput {
    red: Int!
    green: Int!
    blue: Int!
}
type Starship @entity{
    # The ID of the starship
    id: ID!
    # The name of the starship
    name: String!
    # Length of the starship, along the longest axis
    length(unit: LengthUnit = METER): Float
    coordinates: [[Float!]!]
}

extend type Starship {
    speed: Float
}

interface SpeedFactor {
    speedFactor: Int
}

extend type Starship implements SpeedFactor{
    speedType: String
}

union SearchResult = Human | Droid | Starship
```

to this c# code

```c#
public class Query
{
    public Character Hero { get; set; };
    public List<Review> Reviews { get; set; };
    public List<SearchResult> Search { get; set; };
    public Character Character { get; set; };
    public Droid Droid { get; set; };
    public Human Human { get; set; };
    public Starship Starship { get; set; };
}

public class Mutation
{
    public Review CreateReview { get; set; };
}

public enum Episode
{
    NEWHOPE,
    EMPIRE,
    JEDI,
}

public interface Character
{
    Guid Id { get; set; };
    string Name { get; set; };
    List<Character> Friends { get; set; };
    FriendsConnection FriendsConnection { get; set; };
    List<Episode> AppearsIn { get; set; };
}

public enum LengthUnit
{
    METER,
    FOOT,
    CUBIT,
}

public class Human : Character
{
    public Guid Id { get; set; };
    public string Name { get; set; };
    public string HomePlanet { get; set; };
    public float Height { get; set; };
    public float Mass { get; set; };
    public List<Character> Friends { get; set; };
    public FriendsConnection FriendsConnection { get; set; };
    public List<Episode> AppearsIn { get; set; };
    public List<Starship> Starships { get; set; };
}

public class Droid : Character
{
    public Guid Id { get; set; };
    public string Name { get; set; };
    public List<Character> Friends { get; set; };
    public FriendsConnection FriendsConnection { get; set; };
    public List<Episode> AppearsIn { get; set; };
    public string PrimaryFunction { get; set; };
}

public class FriendsConnection
{
    public int TotalCount { get; set; };
    public List<FriendsEdge> Edges { get; set; };
    public List<Character> Friends { get; set; };
    public PageInfo PageInfo { get; set; };
}

public class FriendsEdge
{
    public Guid Cursor { get; set; };
    public Character Node { get; set; };
}

public class PageInfo
{
    public Guid StartCursor { get; set; };
    public Guid EndCursor { get; set; };
    public bool HasNextPage { get; set; };
}

public class Review
{
    public int Stars { get; set; };
    public string Commentary { get; set; };
}

public class Starship : SpeedFactor
{
    public Guid Id { get; set; };
    public string Name { get; set; };
    public float Length { get; set; };
    public List<List<float>> Coordinates { get; set; };
    public float Speed { get; set; };
    public string SpeedType { get; set; };
    public int SpeedFactor { get; set; };
}


public interface SpeedFactor
{
    int SpeedFactor { get; set; };
}


using Microsoft.EntityFrameworkCore;

public class AppDBContext : DbContext
{
  public DbSet<Human> Humans { get; set; }
  public DbSet<Droid> Droids { get; set; }
  public DbSet<FriendsConnection> FriendsConnections { get; set; }
  public DbSet<FriendsEdge> FriendsEdges { get; set; }
  public DbSet<PageInfo> PageInfos { get; set; }
  public DbSet<Review> Reviews { get; set; }
  public DbSet<Starship> Starships { get; set; }
}
```
