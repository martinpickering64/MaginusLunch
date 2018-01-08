using MaginusLunch.MongoDB;
using MaginusLunch.Orders.Domain;
//using MongoDB.Bson.Serialization;
//using MongoDB.Bson.Serialization.IdGenerators;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MaginusLunch.Orders.Repository
{
    public class MongoCalendarRepository : MongoRepository<Calendar>, ICalendarRepository
    {
        //public MongoCalendarRepository(IConfiguration config) : base(config)
        //{
        //}

        public MongoCalendarRepository(MongoRepositorySettings settings,
                                    RetryPolicy asyncRetryPolicy,
                                    RetryPolicy retryPolicy)
            : base(settings, asyncRetryPolicy, retryPolicy)
        {
        }

        //public MongoCalendarRepository(string connectionString, string collectionName) : base(connectionString, collectionName)
        //{
        //}

        //public override void ConfigureSerialzation()
        //{
        //    if (!BsonClassMap.IsClassMapRegistered(typeof(Calendar)))
        //    {
        //        BsonClassMap.RegisterClassMap<Calendar>(cm =>
        //        {
        //            cm.AutoMap();
        //            cm.MapCreator(cal => new Calendar(cal.Id));
        //        });
        //    }
        //}

        Calendar ICalendarRepository.GetCalendar(Guid id)
        {
            return Get(id);
        }

        public Task<Calendar> GetCalendarAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetAsync(id, cancellationToken);
        }

        Calendar ICalendarRepository.GetCalendar()
        {
            return FindAll(0, 1).FirstOrDefault();
        }

        public async Task<Calendar> GetCalendarAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var x = await FindAllAsync(0, 1, cancellationToken).ConfigureAwait(false);
            return x.FirstOrDefault();
        }

        void ICalendarRepository.SaveCalendar(Calendar calendar)
        {
            //calendar.WithdrawnDates = new HashSet<DateTime>(calendar.WithdrawnDates.Select(wd => wd.Date.ToUniversalTime()));
            if (calendar.Version == 0)
            {
                Insert(calendar);
            }
            else
            {
                Replace(calendar);
            }
        }

        public Task SaveCalendarAsync(Calendar calendar, CancellationToken cancellationToken = default(CancellationToken))
        {
            //calendar.WithdrawnDates = new HashSet<DateTime>(calendar.WithdrawnDates.Select(wd => wd.Date.ToUniversalTime()));
            if (calendar.Version == 0)
            {
                return InsertAsync(calendar, cancellationToken);
            }
            else
            {
                return ReplaceAsync(calendar, cancellationToken);
            }
        }
    }
}
