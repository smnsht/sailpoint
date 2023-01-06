using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

string connectionString = app.Configuration.GetConnectionString("sqlite");

// defaults

const int DEFAULT_LIMIT = 10;
const int MIN_LIMIT = 10;
const int MAX_LIMIT = 1000;

const int MIN_SEARCH_VAL_LEN = 1;
const int MAX_SEARCH_VAL_LEN = 20;


// Validate 'limit' query parameter and return it, throws if invalid
int GetLimit(string queryStringVal)
{			
	int limit = int.Parse(queryStringVal ?? "");

	if(limit < MIN_LIMIT || limit > MAX_LIMIT)
	{
		throw new ArgumentOutOfRangeException("limit");
	}

	return limit;
}

// Validate search value and return it in lower case, throws if invalid
string GetSearchValue(string? queryStringVal)
{
	if( string.IsNullOrEmpty(queryStringVal) || 
	   (queryStringVal.Length < MIN_SEARCH_VAL_LEN || queryStringVal.Length > MAX_SEARCH_VAL_LEN))
	{
		throw new ArgumentOutOfRangeException("value");
	}

	return queryStringVal.ToLower();
}


// I desided to implement the whole API as a middleware for the sake of the excercise
app.Run(async context => 
{	
	// limit from query string - might be empty
	var qsLimit = context.Request.Query["limit"].FirstOrDefault();

	// search what?
	var qsValue = context.Request.Query["value"].FirstOrDefault();

	// assume English language
	context.Response.Headers.Add("Content-Language", "en");

	try 
	{
		// limit param defined how many results to select
		int limit = string.IsNullOrEmpty(qsLimit) ? DEFAULT_LIMIT :  GetLimit(qsLimit);

		// autocomplete what?
		string val = GetSearchValue(qsValue);

		// results
		List<string> cities = new List<string>();
		
		using (var connection = new SqliteConnection(connectionString))
		{
			connection.Open();

			var command = connection.CreateCommand();
			command.CommandText = "SELECT name FROM world_cities WHERE instr(lower(name), $value) LIMIT $limit";
			command.Parameters.AddWithValue("$value", val);
			command.Parameters.AddWithValue("$limit", limit);

			using (var reader = await command.ExecuteReaderAsync())
			{
				while (await reader.ReadAsync())								
					cities.Add(reader.GetString(0));							
			}
		}
		
		// OK, return array
		await context.Response.WriteAsJsonAsync(cities);	
	} 
	catch (ArgumentOutOfRangeException are)
	{
		context.Response.StatusCode = 400;
		await context.Response.WriteAsync(are.Message);		
	}
	catch(Exception e) 
	{
		context.Response.StatusCode = 500;
		await context.Response.WriteAsync(e.Message);
	}
});

app.Run();

