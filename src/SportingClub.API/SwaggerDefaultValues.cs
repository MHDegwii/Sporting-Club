using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SportingClub.API;

/// <summary>
/// Represents the Swagger/Swashbuckle operation filter used to document the implicit API version parameter.
/// </summary>
public class SwaggerDefaultValues : IOperationFilter
{
    /// <summary>
    /// Applies the filter to the specified operation using the given context.
    /// </summary>
    /// <param name="operation">The operation to apply the filter to.</param>
    /// <param name="context">The current operation filter context.</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var apiDescription = context.ApiDescription;

        if (operation.Parameters == null)
        {
            return;
        }

        // RESTful APIs do not use query strings to specify the API version.
        // The version should be specified either in the URL path or
        // using request headers. In this case, the version is specified
        // using the "api-version" header.
        foreach (var parameter in operation.Parameters.OfType<OpenApiParameter>().Where(p => p.Name == "api-version"))
        {
            parameter.Description = "The requested API version";
        }
    }
}
