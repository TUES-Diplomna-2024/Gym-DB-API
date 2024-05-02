using System.ComponentModel.DataAnnotations;

namespace GymDB.API.Data.Settings.HelperClasses
{
    public class AzureSettingsConfig
    {
        [Required]
        public string StorageAccount { get; init; }

        [Required]
        public string AccessKey { get; init; }

        [Required]
        public string ImageContainer { get; init; }

        [Required]
        public string VideoContainer { get; init; }

        [Required]
        public string ImageTypesAccepted { get; init; }

        [Required]
        public string VideoTypesAccepted { get; init; }
    }
}
