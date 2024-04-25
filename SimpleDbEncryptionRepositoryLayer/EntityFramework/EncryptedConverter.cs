using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SimpleDbEncryptionRepositoryLayer.EntityFramework;

//public class EncryptedConverter : ValueConverter<string, string> {
//    public EncryptedConverter(IDataProtector dataProtector, ConverterMappingHints mappingHints = null) : base((a) => dataProtector.Protect(a), (a) => dataProtector.Unprotect(a), mappingHints) {

//    }
//}