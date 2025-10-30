SELECT *
FROM Repos
WHERE DATE(date_retrieved) = DATE('now', printf('-%d days', ? * 14))
