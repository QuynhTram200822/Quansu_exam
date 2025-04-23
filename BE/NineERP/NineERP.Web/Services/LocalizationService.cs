using Microsoft.Extensions.Localization;
using NineERP.Application.Interfaces.Common;

namespace NineERP.Web.Services
{
    public class LocalizationService(IStringLocalizer<SharedResource> translate) : ILocalizationService
    {
        public string this[string key] => translate[key];

        public string Get(string key) => translate[key];
    }

}
