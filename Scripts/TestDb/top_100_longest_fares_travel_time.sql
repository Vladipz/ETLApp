SELECT
  TOP 100 *,
  DATEDIFF (
    SECOND,
    tpep_pickup_datetime,
    tpep_dropoff_datetime
  ) AS travel_time
FROM
  ProcessedTrips
ORDER BY
  travel_time DESC;
