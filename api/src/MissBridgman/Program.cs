using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MissBridgman.Repositories;

var host = new HostBuilder()
	.ConfigureFunctionsWorkerDefaults()
	.ConfigureServices(services =>
	{
		services.AddSingleton<IStorage, Storage>();
	})
	.Build();

host.Run();
