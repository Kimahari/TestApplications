using SimpleDbEncryptionRepositoryLayer;

namespace SimpleDbEncryptionRepositoryLayer.Interfaces; 
public interface IForecastService {
    public Task AddForecast(ForecastDbObject forecast);
    public Task<IEnumerable<ForecastDbObject>> GetForecasts();
    public Task<ForecastDbObject> GetForecast(int id);
    public Task UpdateForecast(ForecastDbObject forecast);
    public Task DeleteForecast(int id);
}