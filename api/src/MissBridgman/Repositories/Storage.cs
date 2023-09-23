using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using MissBridgman.Entities;
using System.Text.RegularExpressions;

namespace MissBridgman.Repositories
{
	public class Storage : IStorage
	{
		private static BlobServiceClient _storageAccount = default!;
		private static Dictionary<string, BlobContainerClient?> _containerClients = new();

		public Storage(IConfiguration configuration)
		{
			_storageAccount = new BlobServiceClient(configuration.GetValue<string>("AzureWebJobsStorage"));
		}

		public async Task<Folder> GetItems(string containerName)
		{
			_containerClients.TryGetValue(containerName, out var containerClient);
			if (containerClient == null)
			{
				containerClient = _storageAccount.GetBlobContainerClient(containerName);
				_containerClients.TryAdd(containerName, containerClient);
			}

			List<BlobItem> items = new List<BlobItem>();
			var resultSegment = containerClient.GetBlobsAsync().AsPages(default, 20);

			await foreach(Page<BlobItem> page in resultSegment)
			{
				items.AddRange(page.Values);
			}

			return ConvertToFolders(items, containerClient);
		}

		private Folder ConvertToFolders(List<BlobItem> items, BlobContainerClient containerClient)
		{
			Folder folder = new();
			foreach(var item in items)
			{
				var url = containerClient.GetBlobClient(item.Name).Uri.ToString();
				ParseFolder(item.Name.Split('/'), url, folder);
			}

			return folder;
		}

		private void ParseFolder(string[] parts, string url, Folder folder, int level = 0)
		{
			if (level == parts.Length - 1)
			{
				if (folder.Files == null) folder.Files = new();
				folder.Files.Add(new Entities.File(RemoveNumbers(parts[level]), "", url));
				folder.Folders = null;
			}
			else
			{
				if (folder.Folders == null) folder.Folders = new();
				folder.Folders.TryGetValue(parts[level], out var nextFolder);
				if (nextFolder == null)
				{
					nextFolder = new Folder();
					folder.Folders.TryAdd(parts[level], nextFolder);
				}

				ParseFolder(parts, url, nextFolder, level + 1);
			}
		}

		private string RemoveNumbers(string name)
		{
			var pattern = @"^\d+-|\.(png|jpg|pdf|docx)$";
			return Regex.Replace(name, pattern, string.Empty);
		}
	}
}
