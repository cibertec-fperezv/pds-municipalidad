using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
                try
                {
                    sqlConnection.Open();

                    var cmd = new SqlCommand("usp_Register_s100", sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@homeViaType", form.homeViaType);
                    cmd.Parameters.AddWithValue("@homeAddress", form.homeAddress ?? string.Empty);
                    cmd.Parameters.AddWithValue("@homeDoorNumber", form.homeDoorNumber ?? string.Empty);
                    cmd.Parameters.AddWithValue("@homeBlock", form.homeBlock ?? string.Empty);
                    cmd.Parameters.AddWithValue("@homeFloor", form.homeFloor ?? string.Empty);
                    cmd.Parameters.AddWithValue("@homeInterior", form.homeInterior ?? string.Empty);
                    cmd.Parameters.AddWithValue("@homeZone", form.homeZone ?? string.Empty);
                    cmd.Parameters.AddWithValue("@homeZoneNumber", form.homeZoneNumber ?? string.Empty);
                    cmd.Parameters.AddWithValue("@requesterLastNameDad", form.requesterLastNameDad ?? string.Empty);
                    cmd.Parameters.AddWithValue("@requesterLastNameMom", form.requesterLastNameMom ?? string.Empty);
                    cmd.Parameters.AddWithValue("@requesterName", form.requesterName ?? string.Empty);
                    cmd.Parameters.AddWithValue("@requesterEmail", form.requesterEmail ?? string.Empty);
                    cmd.Parameters.AddWithValue("@requesterPhone", form.requesterPhone ?? string.Empty);
                    cmd.Parameters.AddWithValue("@familyMembers", "");
                    cmd.Parameters.AddWithValue("@services", "");
                    /*cmd.Parameters.AddWithValue("@familyMembers", form.familyMembers.Select(fm => ));
                    cmd.Parameters.AddWithValue("@services", form.services.Select());*/
                    cmd.ExecuteNonQuery();

                    sqlConnection.Close();

                    return "Solicitud registrada correctamente";
                }
                catch (Exception exception)
                {
                    return "Error: " + exception.Message;
                }
            }
        }
    }
}