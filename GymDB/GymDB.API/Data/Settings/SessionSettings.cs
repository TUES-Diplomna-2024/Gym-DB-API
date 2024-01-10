namespace GymDB.API.Data.Settings
{
    public class SessionSettings
    {
        public SessionSettings(IConfiguration config)
        {
            IConfigurationSection sessionSettings = config.GetSection("SessionSettings");

            SessionLifetime = TimeSpan.Parse(sessionSettings["SessionLifetime"] ??
                              throw new InvalidOperationException("'SessionSettings:SessionLifetime' could not be found!"));
        }

        public TimeSpan SessionLifetime { get; private set; }
    }
}
