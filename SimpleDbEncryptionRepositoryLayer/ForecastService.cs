using SimpleDbEncryptionRepositoryLayer.Interfaces;

namespace SimpleDbEncryptionRepositoryLayer;
public class ForecastService : IForecastService {
    private readonly IForecastRepository _repository;
    private readonly IDataProtector dataProtector;

    public ForecastService(IDataProtector dataProtector, IForecastRepository repository) {
        this.dataProtector = dataProtector;
        _repository = repository;
    }

    public async Task AddForecast(ForecastDbObject forecast) {
        await _repository.AddForecast(dataProtector.ProtectObject(forecast));
    }

    public async Task<IEnumerable<ForecastDbObject>> GetForecasts() {
        return (await _repository.GetForecasts()).Select(oo => dataProtector.UnProtectObject(oo));
    }

    public async Task<ForecastDbObject> GetForecast(int id) {
        return dataProtector.UnProtectObject(await _repository.GetForecast(id));
    }

    public async Task UpdateForecast(ForecastDbObject forecast) {
        await _repository.UpdateForecast(dataProtector.ProtectObject(forecast));
    }

    public async Task DeleteForecast(int id) {
        await _repository.DeleteForecast(id);
    }
}
