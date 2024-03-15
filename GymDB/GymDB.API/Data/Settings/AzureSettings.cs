using Azure.Storage;
using Microsoft.IdentityModel.Tokens;

namespace GymDB.API.Data.Settings
{
    public class AzureSettings
    {
        private string storageAccount = "";
        private string accessKey = "";
        private string imageContainer = "";
        private string videoContainer = "";

        public AzureSettings(IConfiguration config)
        {
            IConfigurationSection azureSettings = config.GetSection("AzureSettings");

            if (!azureSettings.Exists())
                throw new InvalidOperationException("'AzureSettings' section could not be found or is empty!");

            StorageAccount = azureSettings["StorageAccount"]!;
            
            AccessKey = azureSettings["AccessKey"]!;

            ImageContainer = azureSettings["ImageContainer"]!;

            VideoContainer = azureSettings["VideoContainer"]!;

            if (azureSettings["ImageTypesAccepted"].IsNullOrEmpty())
                throw new InvalidOperationException("'AzureSettings:ImageTypesAccepted' could not be found or is empty!");

            ImageTypesAccepted = azureSettings["ImageTypesAccepted"]!.Split(";").ToList();

            if (azureSettings["VideoTypesAccepted"].IsNullOrEmpty())
                throw new InvalidOperationException("'AzureSettings:VideoTypesAccepted' could not be found or is empty!");

            VideoTypesAccepted = azureSettings["VideoTypesAccepted"]!.Split(";").ToList();

            Credential = new StorageSharedKeyCredential(StorageAccount, AccessKey);

            BlobUri = new Uri($"https://{StorageAccount}.blob.core.windows.net");
        }

        private string StorageAccount {
            get { return storageAccount; }
            set {
                if (value.IsNullOrEmpty())
                    throw new InvalidOperationException("'AzureSettings:StorageAccount' could not be found or is empty!");

                storageAccount = value;
            }
        }

        private string AccessKey {
            get { return accessKey; }
            set
            {
                if (value.IsNullOrEmpty())
                    throw new InvalidOperationException("'AzureSettings:AccessKey' could not be found or is empty!");

                accessKey = value;
            }
        }

        public string ImageContainer
        {
            get { return imageContainer; }
            private set
            {
                if (value.IsNullOrEmpty())
                    throw new InvalidOperationException("'AzureSettings:ImageContainer' could not be found or is empty!");

                imageContainer = value;
            }
        }

        public string VideoContainer {
            get { return videoContainer; }
            private set
            {
                if (value.IsNullOrEmpty())
                    throw new InvalidOperationException("'AzureSettings:VideoContainer' could not be found or is empty!");

                videoContainer = value;
            }
        }

        public List<string> ImageTypesAccepted { get; private set; }

        public List<string> VideoTypesAccepted { get; private set; }

        public StorageSharedKeyCredential Credential { get; private set; }

        public Uri BlobUri { get; private set; }
    }
}
