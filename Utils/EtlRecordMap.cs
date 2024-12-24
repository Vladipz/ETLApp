using CsvHelper.Configuration;

using ETLApp.Data.Models;

namespace ETLApp.Utils
{
    public sealed class EtlRecordMap : ClassMap<EtlRecord>
    {
        public EtlRecordMap()
        {
            // Map CSV headers to class properties
            Map(static m => m.PickupDateTime).Name("tpep_pickup_datetime");
            Map(static m => m.DropoffDateTime).Name("tpep_dropoff_datetime");
            Map(static m => m.PassengerCount).Name("passenger_count");
            Map(static m => m.TripDistance).Name("trip_distance");
            Map(static m => m.StoreAndFwdFlag).Name("store_and_fwd_flag");
            Map(static m => m.PULocationID).Name("PULocationID");
            Map(static m => m.DOLocationID).Name("DOLocationID");
            Map(static m => m.FareAmount).Name("fare_amount");
            Map(static m => m.TipAmount).Name("tip_amount");
        }
    }
}