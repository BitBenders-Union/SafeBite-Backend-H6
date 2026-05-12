namespace SafeBite_Backend_H6.API.Contracts.Requests.Analysis;

public class AllergyAnalysisItem
{
    public Guid AllergyId { get; set; }
    public string AllergyName { get; set; } = string.Empty;
    public AllergyType AllergyType { get; set; }
}
