namespace NineERP.Application.Interfaces.Common
{
    public interface ILocalizationService
    {
        string this[string key] { get; }
        string Get(string key);
    }
}
