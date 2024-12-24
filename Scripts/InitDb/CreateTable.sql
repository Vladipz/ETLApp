-- Use the newly created database
-- USE TaxiTrips;

-- Create the table to store the processed data
CREATE TABLE ProcessedTrips (
  tpep_pickup_datetime DATETIME NOT NULL,
  tpep_dropoff_datetime DATETIME NOT NULL,
  passenger_count TINYINT NOT NULL,
  trip_distance FLOAT NOT NULL,
  store_and_fwd_flag CHAR(3) NOT NULL,
  PULocationID SMALLINT NOT NULL,
  DOLocationID SMALLINT NOT NULL,
  fare_amount DECIMAL(10, 2) NOT NULL,
  tip_amount DECIMAL(10, 2) NOT NULL
);
