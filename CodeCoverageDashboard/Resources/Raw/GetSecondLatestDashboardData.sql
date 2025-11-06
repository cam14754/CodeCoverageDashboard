SELECT *
FROM Dashboards
WHERE date_retrieved = (
    SELECT DISTINCT date_retrieved
    FROM Dashboards
    ORDER BY date_retrieved DESC
    LIMIT 1 OFFSET 1
);
