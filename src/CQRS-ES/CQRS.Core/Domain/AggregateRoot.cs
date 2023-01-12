using CQRS.Core.Commands;
using CQRS.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRS.Core.Domain
{
    public abstract class AggregateRoot
    {
        protected Guid _id;
        private readonly List<BaseEvent> _Changes = new List<BaseEvent>();

        public Guid Id { get { return _id; } }

        public int Version { get; set; } = -1;

        public IEnumerable<BaseEvent> UncommitedChanges()
        {
            return _Changes;
        }

        public void MarkChangesAsCommited() 
        {
            _Changes.Clear();
        }

        private void ApplyCHanges(BaseEvent @event, bool isNew)
        {
            var method = this.GetType().GetMethod("Apply", new Type[] { @event.GetType() });

            if (method == null)
            {
                throw new ArgumentException(nameof(method),$"the method not found in aggregate for {@event.GetType().Name}");
            }

            method.Invoke(this, new object[] {@event});

            if (isNew)
            {
                _Changes.Add(@event);
            }
        }


        protected void RaiseEvent(BaseEvent @event)
        {
            ApplyCHanges(@event,true);
        }

        public void ReplayEvents(IEnumerable<BaseEvent> events)
        {
            foreach (var @event in events)
            {
                ApplyCHanges(@event,false);
            }
        }
        
    }
}
