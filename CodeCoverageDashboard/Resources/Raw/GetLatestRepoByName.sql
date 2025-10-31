SELECT * 
FROM Repos AS r1
WHERE r1.id IN 
	(SELECT MAX(r2.id) FROM Repos as r2 GROUP BY r2.repo_name)
    AND r1.repo_name = @RepoName