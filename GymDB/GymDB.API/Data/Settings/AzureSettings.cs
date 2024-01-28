using Azure.Storage;
using Microsoft.IdentityModel.Tokens;

namespace GymDB.API.Data.Settings
{
    public class AzureSettings
    {
        private string storageAccount = "";
        private string accessKey = "";
        private string exerciseImageContainer = "";
        private string exerciseVideoContainer = "";

        public AzureSettings(IConfiguration config)
        {
            IConfigurationSection azureSettings = config.GetSection("AzureSettings");

            if (!azureSettings.Exists())
                throw new InvalidOperationException("'AzureSettings' section could not be found or is empty!");

            StorageAccount = azureSettings["StorageAccount"]!;
            
            AccessKey = azureSettings["AccessKey"]!;

            ExerciseImageContainer = azureSettings["ExerciseImageContainer"]!;

            ExerciseVideoContainer = azureSettings["ExerciseVideoContainer"]!;

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

        public string ExerciseImageContainer
        {
            get { return exerciseImageContainer; }
            private set
            {
                if (value.IsNullOrEmpty())
                    throw new InvalidOperationException("'AzureSettings:ExerciseImageContainer' could not be found or is empty!");

                exerciseImageContainer = value;
            }
        }

        public string ExerciseVideoContainer {
            get { return exerciseVideoContainer; }
            private set
            {
                if (value.IsNullOrEmpty())
                    throw new InvalidOperationException("'AzureSettings:ExerciseVideoContainer' could not be found or is empty!");

                exerciseVideoContainer = value;
            }
        }

        public StorageSharedKeyCredential Credential { get; private set; }

        public Uri BlobUri { get; private set; }
    }
}
