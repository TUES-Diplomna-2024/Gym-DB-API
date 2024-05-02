namespace GymDB.API.Data.Settings.HelperClasses
{
    public class RoleDefinition
    {
        public RoleDefinition(string name, string normalizedName, string color)
        {
            Name = name;
            NormalizedName = normalizedName;
            Color = color;
        }

        public string Name { get; private set; }

        public string NormalizedName { get; private set; }

        public string Color { get; private set; }
    }
}
