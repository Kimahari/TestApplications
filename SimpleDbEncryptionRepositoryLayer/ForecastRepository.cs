using Microsoft.EntityFrameworkCore;

using SimpleDbEncryptionRepositoryLayer.EntityFramework;
using SimpleDbEncryptionRepositoryLayer.Interfaces;

namespace SimpleDbEncryptionRepositoryLayer;
//public class ForecastRepository : IForecastRepository {
//    private readonly ForecastDbContext _context;
//    private readonly IDataProtector dataProtector;

//    public ForecastRepository(ForecastDbContext context, IDataProtector dataProtector) {
//        _context = context;
//        this.dataProtector = dataProtector;
//    }

//    public async Task AddForecast(ForecastDbObject forecast) {
//        forecast.SecureData = dataProtector.Protect(forecast.SecureData);
//        _context.WeatherForecasts.Add(forecast);
//        await _context.SaveChangesAsync();
//    }

//    public async Task<IEnumerable<ForecastDbObject>> GetForecasts() {
//        var data = await _context.WeatherForecasts.ToListAsync();
//        return data.Select(oo => {
//            oo.SecureData = dataProtector.Unprotect(oo.SecureData);
//            return oo;
//        });
//    }

//    public async Task<ForecastDbObject> GetForecast(int id) {
//        var result = await _context.WeatherForecasts.FindAsync(id);
//        result.SecureData = dataProtector.Unprotect(result.SecureData);
//        return result;
//    }

//    public async Task UpdateForecast(ForecastDbObject forecast) {
//        forecast.SecureData = dataProtector.Protect(forecast.SecureData);
//        _context.Entry(forecast).State = EntityState.Modified;
//        await _context.SaveChangesAsync();
//    }

//    public async Task DeleteForecast(int id) {
//        var forecast = await _context.WeatherForecasts.FindAsync(id);
//        _context.WeatherForecasts.Remove(forecast);
//        await _context.SaveChangesAsync();
//    }
//}