using System.Threading.Tasks;

namespace StorageManager
{
    public interface ISourceContainer
    {
        Task<string> DownloadContentAsync(string fullPath);
        Task<string> DownloadContentAsync(string shareName, string directory, string fileName);
    }
}