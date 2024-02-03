namespace GymDB.API.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RefreshTokenRequiredAttribute : Attribute { }
}
