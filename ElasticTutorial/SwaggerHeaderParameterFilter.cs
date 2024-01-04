using Swashbuckle.AspNetCore.SwaggerGen;

namespace ElasticTutorial
{
    using Swashbuckle.AspNetCore.SwaggerGen;
    using System.Collections.Generic;
    using Microsoft.OpenApi.Models;

    public class SwaggerHeaderParameterFilter : IOperationFilter
    {
        private readonly IEnumerable<SwaggerHeaderParameter> _headerParameters;

        public SwaggerHeaderParameterFilter(IEnumerable<SwaggerHeaderParameter> headerParameters)
        {
            _headerParameters = headerParameters;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (_headerParameters != null)
            {
                if (operation.Parameters == null)
                {
                    operation.Parameters = new List<OpenApiParameter>();
                }

                foreach (var headerParameter in _headerParameters)
                {
                    operation.Parameters.Add(new OpenApiParameter
                    {
                        Name = headerParameter.Name,
                        Description = headerParameter.Description,
                        In = ParameterLocation.Header,
                        Required = headerParameter.Required,
                        Schema = new OpenApiSchema { Type = "string" } // Adjust the type as needed
                    });
                }
            }
        }
    }

}
