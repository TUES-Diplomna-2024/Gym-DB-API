namespace GymDB.API.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CustomAuthorizeAttribute : Attribute
    {
        public string? Roles { get; set; }
    }
}
