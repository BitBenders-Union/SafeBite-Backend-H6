

namespace SafeBite_Backend_H6.API.Interfaces.Services;

public interface IScanAnalysisService
{
    Task<ScanAnalysisResult> AnalyzeIngredientsAsync(ScanAnalysisRequest request);
}
