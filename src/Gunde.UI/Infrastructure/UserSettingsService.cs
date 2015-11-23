namespace Gunde.UI.Infrastructure
{
    public class UserSettingsService
    {
        public string LastTaskAssemblyPath
        {
            get { return Properties.Settings.Default.LastTaskAssemblyPath; }
            set
            {
                Properties.Settings.Default.LastTaskAssemblyPath = value;
                Properties.Settings.Default.Save();
            }
        }
    }
}