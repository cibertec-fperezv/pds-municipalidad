using Microsoft.Data.SqlClient;
using ProyectoGrupal.Utils.Enums;

namespace ProyectoGrupal.Utils;

public class DatabaseUtils
{
    private readonly IConfiguration _configuration;

    public DatabaseUtils(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public SqlConnection getConnection(DatabaseConnection databaseConnection)
    {
        var connectionName = databaseConnection switch
        {
            DatabaseConnection.ULE_PUENTE_PIEDRA1 => "UlePuentePiedra1URL",
            DatabaseConnection.PROYECTO_MUNICIPALIDAD => "ProyectoMunicipalidad",
            _ => throw new NotSupportedException("Connection Not Supported")
        };
        return new SqlConnection(_configuration["ConnectionStrings:" + connectionName]);
    }
}