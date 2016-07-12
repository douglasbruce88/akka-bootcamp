namespace GithubActors
{
    /// <summary>
    /// Begin processing a new Github repository for analysis
    /// </summary>
    public class ProcessRepo
    {
        public ProcessRepo(string repoUri)
        {
            RepoUri = repoUri;
        }

        public string RepoUri { get; private set; }
    }

    public class RepoKey
    {
        public RepoKey(string owner, string repo)
        {
            Repo = repo;
            Owner = owner;
        }

        public string Owner { get; private set; }

        public string Repo { get; private set; }
    }


    public class RetryableQuery
    {
        public RetryableQuery(object query, int allowableTries) : this(query, allowableTries, 0)
        {
        }

        private RetryableQuery(object query, int allowableTries, int currentAttempt)
        {
            AllowableTries = allowableTries;
            Query = query;
            CurrentAttempt = currentAttempt;
        }


        public object Query { get;  }

        private int AllowableTries { get;  }

        private int CurrentAttempt { get; }

        public bool CanRetry => RemainingTries > 0;

        private int RemainingTries => AllowableTries - CurrentAttempt;

        public RetryableQuery NextTry()
        {
            return new RetryableQuery(Query, AllowableTries, CurrentAttempt+1);
        }
    }
}
