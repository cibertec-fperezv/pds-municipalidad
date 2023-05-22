namespace ProyectoGrupal.Models;

public class FamilyMember
{
    public DocumentTypeModel documentType { get; set; }
    public string documentNumber { get; set; }
    public string lastNameDad { get; set; }
    public string lastNameMom { get; set; }
    public string name { get; set; }
    public string bornDate { get; set; }
    public string sex { get; set; }
    public object relationship { get; set; }
    public bool commonResident { get; set; }
}