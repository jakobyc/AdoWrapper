# ADO.NET Wrapper
Convenient ADO.NET wrapper for various providers.

## Examples:
### SQL Server:
----
#### DataReaders:
##### With mapping:
```
public TestRepository : SqlRepository
{
  public TestRepository() : base("ConnectionStringName") { }
  
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
##### Without mapping:
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
                      EmailConfirmed = reader.Get<bool>("Conmfirmed"),
                      JoinedDate = reader.Get<DateTime>("JoinedOn")
                    };
                 });
}
```
----
#### DataTables:
```
public TestRepository : SqlRepository
{
  public TestRepository() : base("ConnectionStringName") { }
  
  // Without parameters:
  public DataTable GetUsers()
  {
    return GetDataTable("SELECT * FROM Users");
  }
  
  // With parameters:
  public DataTable GetUser(string name)
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
#### Non-queries:
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
