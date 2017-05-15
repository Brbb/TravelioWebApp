using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.File;
using System.IO;

namespace StorageManager
{
    public class CloudStorageManager: ISourceContainer
    {

        private string _connectionString = "";
        public CloudStorageManager(string connectionString)
        {
            _connectionString = connectionString;
        }


        public async Task<string> DownloadContentAsync(string shareName, string directory, string fileName)
        {
			var storageAccount = CloudStorageAccount.Parse(_connectionString);

			var fileClient = storageAccount.CreateCloudFileClient();

			// Get a reference to the file share we created previously.
			var share = fileClient.GetShareReference(shareName);

			// Ensure that the share exists.
            if (await share.ExistsAsync())
			{
				// Get a reference to the root directory for the share.
				var rootDir = share.GetRootDirectoryReference();

                // Get a reference to the directory we created previously.
                if (!string.IsNullOrEmpty(directory))
                    rootDir = rootDir.GetDirectoryReference(directory);

				// Ensure that the directory exists.
                if (await rootDir.ExistsAsync())
				{
					// Get a reference to the file we created previously.
                    var file = rootDir.GetFileReference(fileName);

					// Ensure that the file exists.
                    if (await file.ExistsAsync())
					{
						// Write the contents of the file to the console window.
						Console.WriteLine("OK");
                        return await file.DownloadTextAsync();
					}
				}
			}

            return string.Empty;
        }

        /// <summary>
        /// Downloads the content async. (Not supported for the cloud).
        /// </summary>
        /// <returns>The content async.</returns>
        /// <param name="fullPath">Full path.</param>
        public Task<string> DownloadContentAsync(string fullPath)
        {
            throw new NotSupportedException("Direct file downloading is not supported for cloud storage.");
        }
    }
}
