using SonkwoNoMo.Server.Config;
using SonkwoNoMo.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RegionListOptions>(builder.Configuration.GetSection("RegionList"));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHostedService<AccessService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
