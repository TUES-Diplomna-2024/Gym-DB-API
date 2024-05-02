using Azure.Storage;

namespace GymDB.API.Data.Settings
{
    public class AzureSettings
    {
        public string ImageContainer { get; set; }

        public string VideoContainer { get; set; }

        public List<string> ImageTypesAccepted { get; set; }

        public List<string> VideoTypesAccepted { get; set; }

        public StorageSharedKeyCredential Credential { get; set; }

        public Uri BlobUri { get; set; }
    }
}
