namespace ETLApp.Data.Models
{
    public class EtlRecord
    {
        public DateTime PickupDateTime { get; set; }

        public DateTime DropoffDateTime { get; set; }

        //TODO: Can be nullable
        public int? PassengerCount { get; set; }

        public decimal TripDistance { get; set; }

        public string StoreAndFwdFlag { get; set; }

        public int PULocationID { get; set; }

        public int DOLocationID { get; set; }

        public decimal FareAmount { get; set; }

        public decimal TipAmount { get; set; }
    }
}