# ADO.NET Wrapper
Convenient ADO.NET wrapper for various providers.

## Examples:
### SQL Server:
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
    IDictionary<string, object> parameters = new Dictionary<string, object>()
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
#### DataReaders:
##### With mapping:
```
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
```
##### Without mapping:
```
public ICollection<User> GetUsers()
{
  ICollection<User> users = new List<User>();
  
  using (IDbConnection connection = CreateConnection())
  {
    using (IDbCommand command = CreateCommand(connection, "SELECT * FROM Users"))
    {
      using (IDataReader reader = command.ExecuteReader())
      {
        while (reader.Read())
        {
          // Mapping logic:
          User user = new User();
          user.Id = reader["Id"].ToString();
          user.DisplayName = reader["Name"].ToString();
          user.Email = reader["Email"].ToString();
          user.EmailConfirmed = Convert.ToBoolean(reader["Confirmed"]);
          user.JoinedDate = Convert.ToDateTime(reader["JoinedOn"]);
          
          users.Add(user);
        }
      }
    }
  }
  return users;
}
```
----
#### Non-queries:
```
public int AddUser(string name, string email)
{
    IDictionary<string, object> parameters = new Dictionary<string, object>()
    {
        { "@name", name },
        { "@email", email }
    };
    return ExecuteNonQuery(@"INSERT INTO Users (DisplayName, Email)
                             VALUES (@name, @email)", parameters);
}
```
----
