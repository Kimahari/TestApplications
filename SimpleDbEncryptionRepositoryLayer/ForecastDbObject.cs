using EncryptionHelpersGenerators;

namespace SimpleDbEncryptionRepositoryLayer;

[Encrypt]
public partial class ForecastDbObject {
    public int Id { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string Date { get; set; }
    public string TemperatureC { get; set; }
    public string Summary { get; set; }
    public string SecureData { get; set; }
}

[Encrypt]
public partial class ForecastDbObject2 {
    public int Id { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string Date { get; set; }
    public string TemperatureC { get; set; }
    public string Summary { get; set; }
    public string SecureData { get; set; }
}