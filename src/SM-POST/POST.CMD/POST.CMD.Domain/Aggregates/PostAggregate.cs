using CQRS.Common.Events;
using CQRS.Core.Domain;
using CQRS.Core.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace POST.CMD.Domain.Aggregates
{
    public class PostAggregate : AggregateRoot
    {

        private bool _active;
        private string _author;
        private readonly Dictionary<Guid, Tuple<string, string>> _comments = new Dictionary<Guid, Tuple<string, string>>();


        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }

        public PostAggregate()
        {

        }


        public PostAggregate(Guid id,string author,string message)
        {
            RaiseEvent(new PostCreatedEvent
            {
                Id= id,
                Author = author,
                Message = message,
                DatePosted= DateTime.Now,
            });
        }

        public void Apply(PostCreatedEvent @event)
        {
            _id= @event.Id;
            _active = true;
            _author= @event.Author;
        }

        public void EditMessage(string message)
        {
            if (!_active)
            {
                throw new InvalidOperationException("you cannot edit message of inactive post!");
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new InvalidOperationException($"the value of {nameof(message)} is null or empty!");

            }


            RaiseEvent(new MessageUpdatedEvent
            {
                Id= _id,
                Message = message,
            });
        }

        public void Apply(MessageUpdatedEvent @event)
        {
            _id = @event.Id;

        }


        public void LikePost()
        {
            if (!_active)
            {
                throw new InvalidOperationException("you cannot like the inactive post!");
            }

            RaiseEvent(new PostLikedEvent
            {
                Id = _id
                
            });
        }

        public void Apply(PostLikedEvent @event)
        {
            _id = @event.Id;

        }



        public void AddComment(string comment,string username)
        {
            if (!_active)
            {
                throw new InvalidOperationException("you cannot comment the inactive post!");
            }


            if (string.IsNullOrWhiteSpace(username))
            {
                throw new InvalidOperationException($"the value of {nameof(username)} is null or empty!");

            }


            RaiseEvent(new CommentAddedEvent
            {
                Id = _id,
                Comment = comment,
                Username = username,
                CommentDate= DateTime.Now
                
                
            });
        }


        public void Apply(CommentAddedEvent @event)
        {
            _id = @event.Id;
            _comments.Add(@event.CommentId, new Tuple<string, string>(@event.Comment, @event.Username));

        }


        public void EditComment(Guid commentId,string comment,string username)
        {
            if (!_active)
            {
                throw new InvalidOperationException("you cannot edit comment on the inactive post!");
            }


            if (!_comments[commentId].Item2.Equals(username,StringComparison.CurrentCultureIgnoreCase))
            {
                throw new InvalidOperationException($"You are not allowed edit comment created by another user!");

            }

            RaiseEvent(new CommentUpdatedEvent
            {
                Id = _id,
                CommentId= commentId,
                Comment = comment,
                Username = username,
                EditDate = DateTime.Now
            });
        }

        public void Apply(CommentUpdatedEvent @event)
        {
            _id = @event.Id;
            _comments[@event.CommentId] = new Tuple<string, string>(@event.Comment,@event.Username);

        }


        public void RemoveComment(Guid commentId, string username)
        {
            if (!_active)
            {
                throw new InvalidOperationException("you cannot remove comment on the inactive post!");
            }

            if (!_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new InvalidOperationException($"You are not allowed remove comment created by another user!");

            }

            RaiseEvent(new CommentRemovedEvent
            {
                Id = _id,
                CommentId = commentId
                
            });
        }


        public void Apply(CommentRemovedEvent @event)
        {
            _id = @event.Id;
            _comments.Remove(@event.CommentId);

        }


        public void DeletePost(string username)
        {
            if (!_active)
            {
                throw new InvalidOperationException("you cannot remove Post on the inactive post!");
            }

            if (!_author.Equals(username,StringComparison.CurrentCultureIgnoreCase))
            {
                throw new InvalidOperationException($"You are not allowed remove Post created by another user!");

            }


            RaiseEvent(new PostRemovedEvent
            {
                Id = _id
            });
        }


        public void Apply(PostRemovedEvent @event)
        {
            _id = @event.Id;
            _active = false;

        }

    }
}
