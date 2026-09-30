using DamaguideV1.Models;

namespace DamaguideV1.Pages;

public partial class ResultsPage : ContentPage
{
    public ResultsPage(DamageAnalysis result)
    {
        InitializeComponent();
        FurnitureTypeLabel.Text = result.FurnitureType;
        DamageStatusLabel.Text = result.DamageStatus;
        DamageLocationLabel.Text = result.DamageLocation;
        DamageSeverityLabel.Text = result.DamageSeverity;
        ConfidenceLabel.Text = $"{result.Confidence}%";
        RecommendationLabel.Text =
            result.RepairRecommendation;
    }

    private async void AnotherPhoto_Clicked(
        object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }
}
