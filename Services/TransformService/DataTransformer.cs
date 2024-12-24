using ETLApp.Data.Models;

namespace ETLApp.Services.TransformService
{
    public class DataTransformer
    {
        public IEnumerable<EtlRecord> Transform(IEnumerable<EtlRecord> records)
        {
            var estTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            return records
                .Where(static r => r.PickupDateTime != default && r.DropoffDateTime != default)
                .Select(r =>
                {
                    UpdateStoreAndFwdFlag(r);
                    ConvertToUtc(r, estTimeZone);
                    return r;
                })
                .ToList();
        }

        public IEnumerable<EtlRecord> RemoveDuplicates(IEnumerable<EtlRecord> records, out List<EtlRecord> duplicates)
        {
            duplicates = records
                .GroupBy(static r => new { r.PickupDateTime, r.DropoffDateTime, r.PassengerCount })
                .Where(static g => g.Count() > 1)
                .SelectMany(static g => g.Skip(1))
                .ToList();

            return records.Except(duplicates);
        }

        // Метод для оновлення прапорця StoreAndFwdFlag
        private void UpdateStoreAndFwdFlag(EtlRecord record)
        {
            record.StoreAndFwdFlag = record.StoreAndFwdFlag.Equals("N", StringComparison.OrdinalIgnoreCase) ? "No" : "Yes";
        }

        // Метод для конвертації часу в UTC
        private void ConvertToUtc(EtlRecord record, TimeZoneInfo estTimeZone)
        {
            record.PickupDateTime = TimeZoneInfo.ConvertTimeToUtc(record.PickupDateTime, estTimeZone);
            record.DropoffDateTime = TimeZoneInfo.ConvertTimeToUtc(record.DropoffDateTime, estTimeZone);
        }
    }
}