-- USE TaxiTrips;

-- Indexes for query optimization
CREATE INDEX idx_tip_amount_avg ON ProcessedTrips (PULocationID, tip_amount);

CREATE INDEX idx_trip_distance ON ProcessedTrips (trip_distance DESC);

CREATE INDEX idx_trip_duration ON ProcessedTrips (tpep_pickup_datetime, tpep_dropoff_datetime);
