using SonkwoNoMo.Server.Access.Config;
using SonkwoNoMo.Server.Access.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<AccessOptions>(builder.Configuration.GetSection("Access"));
builder.Services.AddHostedService<AccessService>();

var host = builder.Build();
host.Run();
