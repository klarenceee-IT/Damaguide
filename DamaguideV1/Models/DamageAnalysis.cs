namespace DamaguideV1.Models;

public class DamageAnalysis
{
    public string FurnitureType { get; set; } = "";
    public string DamageStatus { get; set; } = "";
    public string DamageLocation { get; set; } = "";
    public string DamageSeverity { get; set; } = "";
    public int Confidence { get; set; }
    public string RepairRecommendation { get; set; } = "";
}
