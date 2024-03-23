using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MissBridgman.Repositories;

var host = new HostBuilder()
	.ConfigureFunctionsWebApplication()
	.ConfigureServices(services =>
	{
		services.AddSingleton<IStorage, Storage>();
	})
	.Build();

host.Run();
