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
type Human implements Character {
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
  friends: [Character]
  # The friends of the human exposed as a connection with edges
  friendsConnection(first: Int, after: ID): FriendsConnection!
  # The movies this human appears in
  appearsIn: [Episode]!
  # A list of starships this person has piloted, or an empty list if none
  starships: [Starship]
}
# An autonomous mechanical character in the Star Wars universe
type Droid implements Character {
  # The ID of the droid
  id: ID!
  # What others call this droid
  name: String!
  # This droid's friends, or an empty list if they have none
  friends: [Character]
  # The friends of the droid exposed as a connection with edges
  friendsConnection(first: Int, after: ID): FriendsConnection!
  # The movies this droid appears in
  appearsIn: [Episode]!
  # This droid's primary function
  primaryFunction: String
}
# A connection object for a character's friends
type FriendsConnection {
  # The total number of friends
  totalCount: Int
  # The edges for each of the character's friends.
  edges: [FriendsEdge]
  # A list of the friends, as a convenience when edges are not needed.
  friends: [Character]
  # Information for paginating this connection
  pageInfo: PageInfo!
}
# An edge object for a character's friends
type FriendsEdge {
  # A cursor used for pagination
  cursor: ID!
  # The character represented by this friendship edge
  node: Character
}
# Information for paginating this connection
type PageInfo {
  startCursor: ID
  endCursor: ID
  hasNextPage: Boolean!
}
# Represents a review for a movie
type Review {
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
type Starship {
  # The ID of the starship
  id: ID!
  # The name of the starship
  name: String!
  # Length of the starship, along the longest axis
  length(unit: LengthUnit = METER): Float
  coordinates: [[Float!]!]
}

extend type Starship{
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
    public Character hero { get; set; };
    public List<Review> reviews { get; set; };
    public List<SearchResult> search { get; set; };
    public Character character { get; set; };
    public Droid droid { get; set; };
    public Human human { get; set; };
    public Starship starship { get; set; };
}

public class Mutation
{
    public Review createReview { get; set; };
}

public enum Episode
{
    NEWHOPE,
    EMPIRE,
    JEDI,
}

public interface Character
{
    Guid id { get; set; };
    string name { get; set; };
    List<Character> friends { get; set; };
    FriendsConnection friendsConnection { get; set; };
    List<Episode> appearsIn { get; set; };
}

public enum LengthUnit
{
    METER,
    FOOT,
    CUBIT,
}

public class Human : Character
{
    public Guid id { get; set; };
    public string name { get; set; };
    public string homePlanet { get; set; };
    public float height { get; set; };
    public float mass { get; set; };
    public List<Character> friends { get; set; };
    public FriendsConnection friendsConnection { get; set; };
    public List<Episode> appearsIn { get; set; };
    public List<Starship> starships { get; set; };
}

public class Droid : Character
{
    public Guid id { get; set; };
    public string name { get; set; };
    public List<Character> friends { get; set; };
    public FriendsConnection friendsConnection { get; set; };
    public List<Episode> appearsIn { get; set; };
    public string primaryFunction { get; set; };
}

public class FriendsConnection
{
    public int totalCount { get; set; };
    public List<FriendsEdge> edges { get; set; };
    public List<Character> friends { get; set; };
    public PageInfo pageInfo { get; set; };
}

public class FriendsEdge
{
    public Guid cursor { get; set; };
    public Character node { get; set; };
}

public class PageInfo
{
    public Guid startCursor { get; set; };
    public Guid endCursor { get; set; };
    public bool hasNextPage { get; set; };
}

public class Review
{
    public int stars { get; set; };
    public string commentary { get; set; };
}

public class Starship : SpeedFactor
{
    public Guid id { get; set; };
    public string name { get; set; };
    public float length { get; set; };
    public List<List<float>> coordinates { get; set; };
    public float speed { get; set; };
    public string speedType { get; set; };
    public int speedFactor { get; set; };
}


public interface SpeedFactor
{
    int speedFactor { get; set; };
}
```
