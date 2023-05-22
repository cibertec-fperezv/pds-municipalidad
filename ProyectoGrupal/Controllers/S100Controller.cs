using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using ProyectoGrupal.Models;
using ProyectoGrupal.Service;
using ProyectoGrupal.Utils;
using ProyectoGrupal.Utils.Enums;

namespace ProyectoGrupal.Controllers
{
    public class S100Controller : Controller
    {
        private readonly DatabaseUtils _databaseUtils;
        private readonly IViaTypeService _viaTypeService;
        private readonly IServiceTypeService _serviceTypeService;
        private readonly IDocumentTypeService _documentTypeService;
        private readonly IRelationshipService _relationshipService;

        public S100Controller(
            DatabaseUtils databaseUtils,
            IViaTypeService viaTypeService,
            IServiceTypeService serviceTypeService,
            IDocumentTypeService documentTypeService,
            IRelationshipService relationshipService
        )
        {
            _databaseUtils = databaseUtils;
            _viaTypeService = viaTypeService;
            _serviceTypeService = serviceTypeService;
            _documentTypeService = documentTypeService;
            _relationshipService = relationshipService;
        }

        public IActionResult Index()
        {
            ViewBag.ViaTypes = _viaTypeService.ListAll();
            ViewBag.ServiceTypes = _serviceTypeService.ListAll();
            ViewBag.DocumentTypes = _documentTypeService.ListAll();
            ViewBag.Relationships = _relationshipService.ListAll();

            return View();
        }

        [HttpPost]
        public async Task<String> Register(S100 form)
        {
            using (var sqlConnection = _databaseUtils.getConnection(DatabaseConnection.ULE_PUENTE_PIEDRA1))
            {
                sqlConnection.Open();
                using (var sqlTransaction = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        var addressId = string.Empty;
                        var addressCommand = new SqlCommand("usp_Register_Address", sqlConnection, sqlTransaction);
                        addressCommand.CommandType = CommandType.StoredProcedure;
                        addressCommand.Parameters.AddWithValue("@viaType", form.homeViaType);
                        addressCommand.Parameters.AddWithValue("@address", form.homeAddress ?? string.Empty);
                        addressCommand.Parameters.AddWithValue("@doorNumber", form.homeDoorNumber ?? string.Empty);
                        addressCommand.Parameters.AddWithValue("@block", form.homeBlock ?? string.Empty);
                        addressCommand.Parameters.AddWithValue("@floor", form.homeFloor ?? string.Empty);
                        addressCommand.Parameters.AddWithValue("@interior", form.homeInterior ?? string.Empty);
                        addressCommand.Parameters.AddWithValue("@zone", form.homeZone ?? string.Empty);
                        addressCommand.Parameters.AddWithValue("@zoneNumber", form.homeZoneNumber ?? string.Empty);
                        addressCommand.Parameters.AddWithValue("@reference", form.homeReference ?? string.Empty);
                        addressId = addressCommand.ExecuteScalar().ToString()!;

                        var requesterId = string.Empty;
                        var requesterCommand = new SqlCommand("usp_Register_Requester", sqlConnection, sqlTransaction);
                        requesterCommand.CommandType = CommandType.StoredProcedure;
                        requesterCommand.Parameters.AddWithValue("@address", addressId);
                        requesterCommand.Parameters.AddWithValue("@lastNameDad",
                            form.requesterLastNameDad ?? string.Empty);
                        requesterCommand.Parameters.AddWithValue("@lastNameMom",
                            form.requesterLastNameMom ?? string.Empty);
                        requesterCommand.Parameters.AddWithValue("@name", form.requesterName ?? string.Empty);
                        requesterCommand.Parameters.AddWithValue("@email", form.requesterEmail ?? string.Empty);
                        requesterCommand.Parameters.AddWithValue("@phone", form.requesterPhone ?? string.Empty);
                        requesterId = requesterCommand.ExecuteScalar().ToString()!;

                        var familyMemberIds = new List<string>();
                        foreach (var fm in form.familyMembers!)
                        {
                            var familyMemberCommand = new SqlCommand("usp_Register_FamilyMember", sqlConnection,
                                sqlTransaction);
                            familyMemberCommand.CommandType = CommandType.StoredProcedure;
                            familyMemberCommand.Parameters.AddWithValue("@documentType", fm.documentType.id);
                            familyMemberCommand.Parameters.AddWithValue("@documentNumber", fm.documentNumber);
                            familyMemberCommand.Parameters.AddWithValue("@lastNameDad", fm.lastNameDad);
                            familyMemberCommand.Parameters.AddWithValue("@lastNameMom", fm.lastNameMom);
                            familyMemberCommand.Parameters.AddWithValue("@name", fm.name);
                            familyMemberCommand.Parameters.AddWithValue("@relationship", fm.relationship);
                            familyMemberCommand.Parameters.AddWithValue("@sex", fm.sex);
                            familyMemberCommand.Parameters.AddWithValue("@commonResident", fm.commonResident);
                            familyMemberCommand.Parameters.AddWithValue("@bornDate", fm.bornDate);
                            familyMemberCommand.Parameters.AddWithValue("@address", addressId);
                            familyMemberIds.Add(familyMemberCommand.ExecuteScalar().ToString()!);
                        }

                        foreach (var s in form.services!)
                        {
                            var serviceCommand = new SqlCommand("usp_Register_PublicService", sqlConnection,
                                sqlTransaction);
                            serviceCommand.CommandType = CommandType.StoredProcedure;
                            serviceCommand.Parameters.AddWithValue("@type", s.serviceType.id);
                            serviceCommand.Parameters.AddWithValue("@companyName", s.serviceCompanyName);
                            serviceCommand.Parameters.AddWithValue("@supply", s.serviceSupply);
                            var serviceId = serviceCommand.ExecuteScalar().ToString()!;

                            var s100Command = new SqlCommand("usp_Register_s100", sqlConnection, sqlTransaction);
                            s100Command.CommandType = CommandType.StoredProcedure;
                            s100Command.Parameters.AddWithValue("@requester", requesterId);
                            s100Command.Parameters.AddWithValue("@familyMembers", familyMemberIds.Count);
                            s100Command.Parameters.AddWithValue("@service", serviceId);
                            s100Command.ExecuteNonQuery();
                        }

                        sqlTransaction.Commit();
                        return "Solicitud registrada correctamente";
                    }
                    catch (Exception exception)
                    {
                        sqlTransaction.Rollback();
                        return "Error: " + exception.Message;
                    }
                }
            }
        }
    }
}