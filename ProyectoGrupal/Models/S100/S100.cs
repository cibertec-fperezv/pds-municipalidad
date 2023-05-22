namespace ProyectoGrupal.Models;

public class S100
{
    public int? homeViaType { get; set; }
    public string? homeAddress { get; set; }
    public string? homeDoorNumber { get; set; }
    public string? homeBlock { get; set; }
    public string? homeFloor { get; set; }
    public string? homeInterior { get; set; }
    public string? homeZone { get; set; }
    public string? homeZoneNumber { get; set; }
    public string? homeReference { get; set; }
    public string? requesterLastNameDad { get; set; }
    public string? requesterLastNameMom { get; set; }
    public string? requesterName { get; set; }
    public string? requesterEmail { get; set; }
    public string? requesterPhone { get; set; }
    public List<FamilyMember>? familyMembers { get; set; }
    public List<Service>? services { get; set; }
}