using Azure.Storage;

namespace GymDB.API.Data.Settings
{
    public class AzureSettings
    {
        public string ImageContainer { get; set; }

        public List<string> AcceptedImageTypes { get; set; }

        public StorageSharedKeyCredential Credential { get; set; }

        public Uri BaseBlobUri { get; set; }
    }
}
