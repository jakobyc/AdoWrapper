# ADO.NET Wrapper
Convenient ADO.NET wrapper for [.NET Standard](https://docs.microsoft.com/en-us/dotnet/standard/net-standard).

## Examples:
### DataReaders:
#### With mapping:
```
public class TestRepository : Repository<SqlConnection>
{
  public TestRepository(string connectionString) : base(connectionString) { }
  
  // DTO:
  public class User
  {
    [Column("Id")]
    public string Id { get; set; }
    [Column("Name")]
    public string DisplayName { get; set; }
    [Column("Email")]
    public string Email { get; set; }

    [Column("Confirmed")]
    public bool EmailConfirmed { get; set; }

    [Column("JoinedOn")]
    public DateTime JoinedDate { get; set; }
  }

  public ICollection<User> GetUsers()
  {
    return Execute<User>("SELECT * FROM Users");
  }
}
```
#### Without mapping:
```
public ICollection<User> GetUsers()
{
  return Execute("SELECT * FROM Users",
                 // This can be replaced with a mapping function for improved readability:
                 (reader) =>
                 {
                    return new User()
                    {
                      Id = reader.Get<string>("Id"),
                      DisplayName = reader.Get<string>("Name"),
                      Email = reader.Get<string>("Email"),
                      EmailConfirmed = reader.Get<bool>("Confirmed"),
                      JoinedDate = reader.Get<DateTime>("JoinedOn")
                    };
                 });
}
```
----
#### DataTables:
```
public class TestRepository : Repository<SqliteConnection>
{
  public TestRepository(string connectionString) : base(connectionString) { }
  
  // Without parameters:
  public DataTable GetUsers()
  {
    return GetDataTable("SELECT * FROM Users");
  }
  
  // With parameters:
  public DataTable GetUser(string id, string name, string email)
  {
    AdoParameters parameters = new AdoParameters()
    {
        { "@id", id },
        { "@name", name },
        { "@email", email }
    };
    return GetDataTable(@"SELECT *
                          FROM Users
                          WHERE Id = @id
                            AND Name = @name
                            AND Email = @email", parameters);
  }
}
```
----
### Non-queries:
```
public int AddUser(string name, string email)
{
    AdoParameters parameters = new AdoParameters()
    {
        { "@name", name },
        { "@email", email }
    };
    return ExecuteNonQuery(@"INSERT INTO Users (DisplayName, Email)
                             VALUES (@name, @email)", parameters);
}
```
----
