using Sybrin.Core.SourceGeneration;

namespace SimpleDbEncryptionRepositoryLayer;

//[PIIEncrypt, PIIDecrypt]
public partial class ForecastDbObject {
    public int Id { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string Date { get; set; }
    public string TemperatureC { get; set; }
    public string Summary { get; set; }
    public string SecureData { get; set; }
}
