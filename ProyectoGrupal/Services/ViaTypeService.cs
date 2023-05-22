using Microsoft.Data.SqlClient;
using ProyectoGrupal.Models;
using ProyectoGrupal.Utils;
using ProyectoGrupal.Utils.Enums;

namespace ProyectoGrupal.Service;

public class ViaTypeService : IViaTypeService
{
    private readonly DatabaseUtils _databaseUtils;
    
    public ViaTypeService(DatabaseUtils databaseUtils)
    {
        _databaseUtils = databaseUtils;
    }

    public List<ViaType> ListAll()
    {
        List<ViaType> viaTypes = new List<ViaType>();
        using (var sqlConnection = _databaseUtils.getConnection(DatabaseConnection.ULE_PUENTE_PIEDRA1))
        {
            sqlConnection.Open();
            var sqlCommand = new SqlCommand("SELECT * FROM TB_TIPOVIA", sqlConnection);
            var dataReader = sqlCommand.ExecuteReader();
            while (dataReader.Read())
            {
                viaTypes.Add(new ViaType()
                {
                    Id = (int)dataReader["idTipoVia"],
                    Name = (string)dataReader["nomVia"]
                });
            }

            sqlConnection.Close();
        }

        return viaTypes;
    }
}