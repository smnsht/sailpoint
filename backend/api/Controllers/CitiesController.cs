using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class CitiesController : ControllerBase
{
	private readonly string connectionString;	

	[FromQuery]
	[Range(10, 1000)]
	public int Limit { get; set; } = 10;

	[FromQuery]
	[Required]
	[MinLength(2)]
	[MaxLength(20)]
	[RegularExpression(@"^[\w\d\s]+$")]	
	public string? Term { get; set; }

	public CitiesController(ConnectionStringContainer csc)
	{		
		connectionString = csc.Get();		
	}


	[HttpGet(Name = "GetCities")]
	public async IAsyncEnumerable<string> Get([EnumeratorCancellation] CancellationToken token)
	{				
		using (var connection = new SqliteConnection(connectionString))
		{
			connection.Open();

			var command = connection.CreateCommand();
			command.CommandText = "SELECT name FROM world_cities WHERE instr(lower(name), $value) LIMIT $limit";
			command.Parameters.AddWithValue("$value", Term?.ToLower());
			command.Parameters.AddWithValue("$limit", Limit);

			// we want to be able to cancel potentially heavy server operation 
			// via request cancellation from client, that's why ExecuteReader receaives cancellation token 
			// as it's parameter
			using (var reader = await command.ExecuteReaderAsync(token))
			{				
				while (await reader.ReadAsync())												
					yield return reader.GetString(0);														
			}
		}				
	}
}