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
            DatabaseConnection.ULE_PUENTE_PIEDRA1 => "Server=(local);DataBase=Ule_Puente_Piedra1;TrustServerCertificate=true;uid=sa;pwd=MSSQL.01062017",
            DatabaseConnection.PROYECTO_MUNICIPALIDAD => "Server=(local);DataBase=Proyecto_Municipalidad;TrustServerCertificate=true;uid=sa;pwd=MSSQL.01062017",
            _ => throw new NotSupportedException("Connection Not Supported")
        };
        return new SqlConnection(_configuration["ConnectionStrings:" + connectionName]);
    }
}