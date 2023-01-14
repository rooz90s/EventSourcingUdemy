using CQRS.Core.Domain;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using POST.CMD.Infrastructure.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POST.CMD.Infrastructure.Repositories
{
    public class EventStoreRepository : IEventStoreRepository
    {

        private readonly IMongoCollection<EventModel> _eventStoreCollection;


        public EventStoreRepository(IOptions<MongoDbConfig> config)
        {
            var mongoClient = new MongoClient(config.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(config.Value.Database);

            _eventStoreCollection = mongoDatabase.GetCollection<EventModel>(config.Value.Collection);

        }


        public async Task<List<EventModel>> FindByAggregateId(Guid AggregateId)
        {
            return await _eventStoreCollection.Find(x=> x.AggrigateIdentifier == AggregateId).ToListAsync().ConfigureAwait(false);
        }

        public async Task SaveAsync(EventModel @event)
        {
            await _eventStoreCollection.InsertOneAsync(@event).ConfigureAwait(false); 
        }
    }
}
