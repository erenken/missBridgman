using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using MissBridgman.Repositories;

namespace MissBridgman.Functions
{
    public class GetImages
    {
        private readonly ILogger _logger;
		private readonly IStorage _storage;

		public GetImages(ILoggerFactory loggerFactory, IStorage storage)
        {
            _logger = loggerFactory.CreateLogger<GetImages>();
			_storage = storage;
		}

        [Function("GetImages")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
			var year = req.Query["year"];

			var images = await _storage.GetItems("images");

			if (!string.IsNullOrWhiteSpace(year))
				images.Folders?.TryGetValue(year, out images);

			var response = req.CreateResponse(HttpStatusCode.OK);
			await response.WriteAsJsonAsync(images);

			return response;
		}
    }
}
