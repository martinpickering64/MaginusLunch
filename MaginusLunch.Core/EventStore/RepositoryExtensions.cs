using MaginusLunch.Core.Aggregates;
using System;
using System.Threading.Tasks;

namespace MaginusLunch.Core.EventStore
{
    public static class RepositoryExtensions
    {
        public static void Save(this IRepository repository, IAggregate aggregate, Guid commitId)
        {
            repository.Save(aggregate, commitId, a => { });
        }
        public static Task SaveAsync(this IRepository repository, IAggregate aggregate, Guid commitId)
        {
            return repository.SaveAsync(aggregate, commitId, a => { });
        }
    }
}
