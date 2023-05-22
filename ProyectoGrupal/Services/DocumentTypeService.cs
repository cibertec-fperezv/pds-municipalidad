using Microsoft.Data.SqlClient;
using ProyectoGrupal.Models;
using ProyectoGrupal.Utils;
using ProyectoGrupal.Utils.Enums;

namespace ProyectoGrupal.Service;

public class DocumentTypeService : IDocumentTypeService
{
    private readonly DatabaseUtils _databaseUtils;
    
    public DocumentTypeService(DatabaseUtils databaseUtils)
    {
        _databaseUtils = databaseUtils;
    }

    public List<DocumentType> ListAll()
    {
        List<DocumentType> viaTypes = new List<DocumentType>();
        using (var sqlConnection = _databaseUtils.getConnection(DatabaseConnection.ULE_PUENTE_PIEDRA1))
        {
            sqlConnection.Open();
            var sqlCommand = new SqlCommand("SELECT * FROM TB_TIPODOCUMENTO", sqlConnection);
            var dataReader = sqlCommand.ExecuteReader();
            while (dataReader.Read())
            {
                viaTypes.Add(new DocumentType()
                {
                    Id = (int)dataReader["idTipoDoc"],
                    Name = (string)dataReader["nomDoc"]
                });
            }

            sqlConnection.Close();
        }

        return viaTypes;
    }
}