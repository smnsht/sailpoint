using Xunit;
using api.Controllers;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace api.tests;

public class UnitTest1
{
    
    private string _connectionString;
    private ConnectionStringContainer _connStrContainer;

    public UnitTest1()
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json");

        var cfg = builder.Build();
        
        _connectionString = cfg.GetConnectionString("sqlite") ?? string.Empty;
        _connStrContainer = new ConnectionStringContainer(_connectionString);
    }

    private CitiesController BuildContrller() => new CitiesController(_connStrContainer);

    private async Task<List<string>> ReadResponse(IAsyncEnumerable<string> enumerable)
    {
        List<string> list = new List<string>();

        await foreach(var str in enumerable)        
            list.Add(str);
        
        return list;
    }

    [Fact]
    public void ConnectionString_Is_Not_Blank()
    {        
        Assert.NotNull(_connectionString);
        Assert.NotEqual(0, _connectionString.Length);
    }

    [Fact]
    public async Task Got_Equal_Results_For_Mixied_Case_Terms()
    {
        var token = new CancellationToken();
        var ctrl = BuildContrller();

        ctrl.Limit = 10;
        ctrl.Term = "bab";
        
        var babResponse = ctrl.Get(token);
        var bab = await ReadResponse(babResponse);
        
        Assert.Equal(10, bab.Count);

        // mix letters case
        ctrl.Term = "BaB";

        var BaB_Response = ctrl.Get(token);
        var BaB = await ReadResponse(BaB_Response);

        Assert.Equal(10, BaB.Count);

        // search sould be not case-sensitive
        Assert.Equal(bab, BaB);
    }
}