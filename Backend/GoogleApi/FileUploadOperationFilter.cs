using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleApi
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var fileParams = context.MethodInfo
                .GetParameters()
                .Where(p => p.ParameterType == typeof(UploadFileRequest))
                .ToList();

            if (!fileParams.Any())
                return;

            operation.RequestBody = new OpenApiRequestBody
            {
                Content =
            {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Properties = new Dictionary<string, OpenApiSchema>
                        {
                            // single file
                            ["File"] = new OpenApiSchema
                            {
                                Type = "string",
                                Format = "binary"
                            },
                            // multiple files
                            ["Files"] = new OpenApiSchema
                            {
                                Type = "array",
                                Items = new OpenApiSchema
                                {
                                    Type = "string",
                                    Format = "binary"
                                }
                            }
                        }
                    }
                }
            }
            };
        }
    }
}
