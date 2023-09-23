using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using MissBridgman.Repositories;

namespace MissBridgman.Functions
{
	public class GetDocuments
	{
		private readonly ILogger _logger;
		private readonly IStorage _storage;

		public GetDocuments(ILoggerFactory loggerFactory, IStorage storage)
		{
			_logger = loggerFactory.CreateLogger<GetDocuments>();
			_storage = storage;
		}

		[Function("GetDocuments")]
		public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
		{
			var documents = await _storage.GetItems("documents");
			var response = req.CreateResponse(HttpStatusCode.OK);
			await response.WriteAsJsonAsync(documents);

			return response;
		}
	}
}
