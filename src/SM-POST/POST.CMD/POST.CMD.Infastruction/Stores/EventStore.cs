using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Infrastructure;
using POST.CMD.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POST.CMD.Infrastructure.Stores
{
    public class EventStore : IEventStore
    {

        private readonly IEventStoreRepository _eventStoreRepository;

        public EventStore(IEventStoreRepository eventStoreRepository)
        {
            _eventStoreRepository = eventStoreRepository;
        }



        public async Task<List<BaseEvent>> GetEventAsync(Guid AggregateId)
        {
            var eventStream = await _eventStoreRepository.FindByAggregateId(AggregateId);

            if (eventStream == null || !eventStream.Any()) 
            {
                throw new AggregateException("Inccorect post id provided");
            }

            return eventStream.OrderBy(x=>x.Version).Select(x=>x.EventData).ToList();

        }

        public async Task SaveEventAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int ExpectedVersion)
        {
            var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);

            if (ExpectedVersion != -1 && eventStream[^1].Version != ExpectedVersion)
            {
                throw new AggregateException("Version Errror");
            }

            var version = ExpectedVersion;


            foreach(var @event in events)
            {
                version++;
                @event.Version = version;
                var eventType = @event.GetType().Name;

                var eventModel = new EventModel
                {
                    TimeStamp = DateTime.UtcNow,
                    AggrigateIdentifier = aggregateId,
                    AggregateType = nameof(PostAggregate),
                    Version= version,
                    EventType= eventType,
                    EventData= @event

                };

                await _eventStoreRepository.SaveAsync(eventModel);

            }

        }
    }
}
