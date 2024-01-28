namespace GymDB.API.Data.Settings
{
    public class AzureSettings
    {
        public AzureSettings(IConfiguration config)
        {
            IConfigurationSection azureSettings = config.GetSection("AzureSettings");

            if (!azureSettings.Exists())
                throw new InvalidOperationException("'AzureSettings' section could not be found or is empty!");

            StorageAccount = azureSettings["StorageAccount"] ??
                             throw new InvalidOperationException("'AzureSettings:StorageAccount' could not be found!");

            AccessKey = azureSettings["AccessKey"] ??
                        throw new InvalidOperationException("'AzureSettings:AccessKey' could not be found!");

            BlobUri = $"https://{StorageAccount}.blob.core.windows.net";
        }

        public string StorageAccount { get; private set; }

        public string AccessKey { get; private set; }

        public string BlobUri { get; private set; }
    }
}
