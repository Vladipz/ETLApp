SELECT
  TOP 1 PULocationID,
  AVG(tip_amount) AS avg_tip
FROM
  ProcessedTrips
GROUP BY
  PULocationID
ORDER BY
  avg_tip DESC;
