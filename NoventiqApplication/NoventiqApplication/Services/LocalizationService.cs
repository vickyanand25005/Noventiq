using Microsoft.AspNetCore.Localization;
using NoventiqApplication.Interface;
using NoventiqApplication.Resources;

namespace NoventiqApplication.Services
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LocalizationService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetCurrentCulture()
        {
            var requestCulture = _httpContextAccessor.HttpContext?.Features.Get<IRequestCultureFeature>();
            return requestCulture?.RequestCulture.Culture.Name ?? "en";
        }

        public string GetLocalizedString(string key)
        {
            var culture = GetCurrentCulture();
            return SharedResourceHelper.GetString(key, culture);
        }
    }
}
