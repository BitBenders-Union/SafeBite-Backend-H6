namespace SafeBite_Backend_H6.API.Interfaces.Services.OCR;

public interface IAllergyMatcher
{
    List<DetectedAllergyAnalysisResult> MatchLocalAllergies(string text, List<AllergyAnalysisItem> allergies);
    void MergeResults(ScanAnalysisResult ai, List<DetectedAllergyAnalysisResult> local);
}
