using System.Globalization;
using System.Resources;

namespace NoventiqApplication.Resources
{
    public static class SharedResourceHelper
    {
        private static readonly ResourceManager ResourceManager =
            new ResourceManager("NoventiqApplication.Resources.SharedResources", typeof(SharedResourceHelper).Assembly);

        public static string GetString(string key, string cultureName = "en")
        {
            var culture = new CultureInfo(cultureName);
            return ResourceManager.GetString(key, culture) ?? key;
        }
    }
}
