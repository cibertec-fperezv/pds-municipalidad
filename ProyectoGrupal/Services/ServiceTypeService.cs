using Microsoft.Data.SqlClient;
using ProyectoGrupal.Models;
using ProyectoGrupal.Utils;
using ProyectoGrupal.Utils.Enums;

namespace ProyectoGrupal.Service;

public class ServiceTypeService : IServiceTypeService
{
    private readonly DatabaseUtils _databaseUtils;
    
    public ServiceTypeService(DatabaseUtils databaseUtils)
    {
        _databaseUtils = databaseUtils;
    }
    
    public List<ServiceType> ListAll()
    {
        List<ServiceType> serviceTypes = new List<ServiceType>();
        using (var sqlConnection = _databaseUtils.getConnection(DatabaseConnection.ULE_PUENTE_PIEDRA1))
        {
            sqlConnection.Open();
            var sqlCommand = new SqlCommand("SELECT * FROM TB_TIPOSERVICIO", sqlConnection);
            var dataReader = sqlCommand.ExecuteReader();
            while (dataReader.Read())
            {
                serviceTypes.Add(new ServiceType()
                {
                    Id = (int)dataReader["idTipoServicio"],
                    Name = (string)dataReader["nomTipoServicio"]
                });
            }

            sqlConnection.Close();
        }

        return serviceTypes;
    }
}