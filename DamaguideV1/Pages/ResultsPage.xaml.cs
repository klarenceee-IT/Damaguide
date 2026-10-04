using Microsoft.Maui.Controls;

namespace DamaguideV1.Pages;

public partial class ResultsPage : ContentPage
{
    private readonly object _result;

    public ResultsPage(object result)
    {
        InitializeComponent();
        _result = result;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await DisplayResultsAsync(_result);
    }

    private async Task DisplayResultsAsync(object result)
    {
        dynamic analysis = result;

        string furnitureType = (analysis != null && analysis.FurnitureType != null)
            ? analysis.FurnitureType.ToString()
            : string.Empty;
        string damageStatus = (analysis != null && analysis.DamageStatus != null)
            ? analysis.DamageStatus.ToString()
            : "N/A";
        string damageLocation = (analysis != null && analysis.DamageLocation != null)
            ? analysis.DamageLocation.ToString()
            : "N/A";
        string damageSeverity = (analysis != null && analysis.DamageSeverity != null)
            ? analysis.DamageSeverity.ToString()
            : "N/A";
        string confidence = (analysis != null && analysis.Confidence != null)
            ? analysis.Confidence.ToString()
            : "0";
        string recommendation = (analysis != null && analysis.RepairRecommendation != null)
            ? analysis.RepairRecommendation.ToString()
            : string.Empty;

        // Check 1: Image is not recognized as furniture
        bool isUnrecognized = string.IsNullOrWhiteSpace(furnitureType) ||
                               furnitureType.Contains("Not Furniture", StringComparison.OrdinalIgnoreCase) ||
                               damageStatus.Contains("Not Recognizable", StringComparison.OrdinalIgnoreCase) ||
                               furnitureType.Contains("Unknown", StringComparison.OrdinalIgnoreCase);

        // Check 2: Furniture is intact / not broken
        bool isIntact = damageStatus.Contains("Intact", StringComparison.OrdinalIgnoreCase) ||
                        damageSeverity.Equals("None", StringComparison.OrdinalIgnoreCase) ||
                        damageStatus.Contains("No Damage", StringComparison.OrdinalIgnoreCase);

        if (isUnrecognized)
        {
            FurnitureTypeLabel.Text = "Not Recognized";
            DamageStatusLabel.Text = "N/A";
            DamageLocationLabel.Text = "N/A";
            DamageSeverityLabel.Text = "N/A";
            ConfidenceLabel.Text = "0%";
            RecommendationLabel.Text = "The uploaded image does not appear to contain a recognized furniture item. Please ensure the item is clearly visible and well-lit.";

            await DisplayAlertAsync(
                "Item Not Recognized",
                "We couldn't detect any furniture in this photo. Returning to the home screen.",
                "Back to Home Screen"
            );

            await GoBackToHomeAsync();
        }
        else if (isIntact)
        {
            FurnitureTypeLabel.Text = furnitureType;
            DamageStatusLabel.Text = "Intact (No Damage)";
            DamageLocationLabel.Text = "None Detected";
            DamageSeverityLabel.Text = "None";
            ConfidenceLabel.Text = $"{confidence}%";
            RecommendationLabel.Text = !string.IsNullOrWhiteSpace(recommendation)
                ? recommendation
                : "This item appears to be in good structural condition. No repairs are necessary at this time.";

            await DisplayAlertAsync(
                "No Damage Detected",
                "Great news! The furniture item appears to be completely intact and undamaged.",
                "OK"
            );
        }
        else
        {
            FurnitureTypeLabel.Text = furnitureType;
            DamageStatusLabel.Text = damageStatus;
            DamageLocationLabel.Text = damageLocation;
            DamageSeverityLabel.Text = damageSeverity;
            ConfidenceLabel.Text = $"{confidence}%";

            if (!string.IsNullOrEmpty(recommendation))
            {
                for (int i = 2; i <= 10; i++)
                {
                    recommendation = recommendation
                        .Replace($". {i}. ", $".\n\n{i}. ")
                        .Replace($" {i}. ", $"\n\n{i}. ");
                }
            }
            RecommendationLabel.Text = recommendation;
        }
    }

    private async void OnBackToHomeButtonClicked(object sender, EventArgs e)
    {
        await GoBackToHomeAsync();
    }

    private async Task GoBackToHomeAsync()
    {
        // Safely navigate back to the root HomePage using Shell navigation
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("//HomePage");
        }
        else if (Navigation != null)
        {
            await Navigation.PopToRootAsync();
        }
    }
}