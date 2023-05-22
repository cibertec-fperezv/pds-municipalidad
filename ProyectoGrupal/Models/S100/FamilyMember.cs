namespace ProyectoGrupal.Models;

public class FamilyMember
{
    public object DocumentType { get; set; }
    public string DocumentNumber { get; set; }
    public string LastNameDad { get; set; }
    public string LastNameMom { get; set; }
    public string Name { get; set; }
    public string BornDate { get; set; }
    public string Sex { get; set; }
    public object Relationship { get; set; }
    public bool CommonResident { get; set; }
}