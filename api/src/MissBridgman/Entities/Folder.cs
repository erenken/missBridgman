namespace MissBridgman.Entities
{
	public class Folder
	{
		public Dictionary<string, Folder>? Folders { get; set; }
		public List<File>? Files { get; set; }
	}
}
