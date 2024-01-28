namespace GymDB.API.Data.Settings
{
    public class SessionSettings
    {
        public SessionSettings(IConfiguration config)
        {
            IConfigurationSection sessionSettings = config.GetSection("SessionSettings");

            if (!sessionSettings.Exists())
                throw new InvalidOperationException("'SessionSettings' section could not be found or is empty!");

            TimeSpan result;

            if (!TimeSpan.TryParse(sessionSettings["SessionLifetime"], out result))
                throw new InvalidOperationException("'SessionSettings:SessionLifetime' could not be found, is empty or is in invalid format!");

            SessionLifetime = result;
        }

        public TimeSpan SessionLifetime { get; private set; }
    }
}
