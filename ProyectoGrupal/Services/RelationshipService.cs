using Microsoft.Data.SqlClient;
using ProyectoGrupal.Models;
using ProyectoGrupal.Utils;
using ProyectoGrupal.Utils.Enums;

namespace ProyectoGrupal.Service;

public class RelationshipService : IRelationshipService
{
    private readonly DatabaseUtils _databaseUtils;

    public RelationshipService(DatabaseUtils databaseUtils)
    {
        _databaseUtils = databaseUtils;
    }

    public List<Relationship> ListAll()
    {
        List<Relationship> viaTypes = new List<Relationship>();
        using (var sqlConnection = _databaseUtils.getConnection(DatabaseConnection.ULE_PUENTE_PIEDRA1))
        {
            sqlConnection.Open();
            var sqlCommand = new SqlCommand("SELECT * FROM TB_PARENTESCO", sqlConnection);
            var dataReader = sqlCommand.ExecuteReader();
            while (dataReader.Read())
            {
                viaTypes.Add(new Relationship()
                {
                    Id = (int)dataReader["idParentesco"],
                    Name = (string)dataReader["nomParentesco"]
                });
            }

            sqlConnection.Close();
        }

        return viaTypes;
    }
}