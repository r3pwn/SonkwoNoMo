using SonkwoNoMo.Server.Gateway.Config;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RegionListOptions>(builder.Configuration.GetSection("RegionList"));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// not needed - the gateway service is HTTP-only
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
