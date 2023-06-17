using Core.Localization;
using Core.Utilities.IoC;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Extensions
{
    public static class LocalizerExtensions
    {
        public static string ToLocale(this string input)
        {
            if (input == null)
                return "";

            try
            {
                var _localizer = ServiceTool.ServiceProvider.GetService<IStringLocalizer<Resource>>();

                return _localizer[input];
            }
            catch
            {
                return input;
            }
        }
    }
}
