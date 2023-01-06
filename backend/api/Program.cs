var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ConnectionStringContainer>(provider => {
    var cfg = provider.GetService<IConfiguration>();
    string connString = cfg.GetConnectionString("sqlite");

    return new ConnectionStringContainer(connString);
});

builder.Services.AddCors(options => 
{ 
    options.AddDefaultPolicy(policy => 
    {
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowAnyOrigin();
    }); 	
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
//app.UseAuthorization();
app.UseCors();

app.MapControllers();

app.Run();

public class ConnectionStringContainer
{
    private string _val;

    public ConnectionStringContainer(string val)
    {
        _val = val;
    }

    public string  Get() => _val;
};
