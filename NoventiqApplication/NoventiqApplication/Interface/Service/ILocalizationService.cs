namespace NoventiqApplication.Interface
{
    public interface ILocalizationService
    {
        string GetCurrentCulture();
        string GetLocalizedString(string key);
    }
}
