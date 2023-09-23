using MissBridgman.Entities;

namespace MissBridgman.Repositories
{
	public interface IStorage
	{
		Task<Folder> GetItems(string containerName);
	}
}
