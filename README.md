# ADO.NET Wrapper
Convenient ADO.NET wrapper for various providers.

## Examples:
### SQL Server:
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
