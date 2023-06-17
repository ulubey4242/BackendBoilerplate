using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using System.Reflection;
using WebAPI.Attributes;

namespace WebAPI.Filters
{
    public class SwaggerIgnoreFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var description in context.ApiDescriptions)
            {
                description.TryGetMethodInfo(out MethodInfo info);

                var attributes = info.GetCustomAttributes(true).OfType<IgnoreApiAttribute>().Distinct();

                if (attributes.Any())
                {
                    var path = description.RelativePath;

                    var exclueRoutes = swaggerDoc.Paths
                                            .Where(x => x.Key.ToLowerInvariant().Contains(path.ToLowerInvariant()))
                                            .ToList();

                    exclueRoutes.ForEach(route => swaggerDoc.Paths.Remove(route.Key));
                }
            }
        }
    }
}
