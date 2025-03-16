using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace BlocklistAPI.Classes;

/// <summary>
/// I'm not sure if this is still in use or not
/// </summary>
public class EnumSchemaFilter : ISchemaFilter
{
    /// <inheritdoc/>
    public void Apply( OpenApiSchema schema, SchemaFilterContext context )
    {
        if ( context.Type.IsEnum )
        {
            schema.Enum.Clear( );
            Enum.GetNames( context.Type )
                .ToList( )
                .ForEach( name => schema.Enum.Add( new OpenApiString( $"{name}" ) ) );
        }
    }
}
