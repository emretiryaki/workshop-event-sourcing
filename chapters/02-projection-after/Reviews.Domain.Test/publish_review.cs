using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Reviews.Service.WebApi.Modules.Reviews;
using Xunit;
using Xunit.Abstractions;

namespace Reviews.Domain.Test
{
    public class publish_review : Spesification<Review,Contracts.Reviews.V1.ReviewPublish>
    {
        public publish_review(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        public readonly Fixture AutoFixture = new Fixture();

        [Fact]
        public void review_content_is_published()
        {
            RaisedEvents.Should().BeEquivalentTo(new Domain.Events.V1.ReviewPublished
            {
                Id = AggregateId,
                PublishAt = ChangedAt,
                OwnerId = OwnerId
            });
        }

        private Guid AggregateId { get; } = Guid.NewGuid();
        private Guid OwnerId { get; } = Guid.NewGuid();
        private DateTime ChangedAt { get; } = DateTime.UtcNow;
        
        public override object[] Given() => AutoFixture.Build<Events.V1.ReviewCreated>()
            .With(e => e.Id, AggregateId)
            .With(e=>e.Owner,OwnerId)
            .With(e=> e.Caption,"First Review")
            .With(e=> e.Content, "This is my first review.")
            .CreateMany(1)
            .ToArray();

        public override Contracts.Reviews.V1.ReviewPublish When() => AutoFixture
            .Build<Contracts.Reviews.V1.ReviewPublish>()
            .With(e => e.Id, AggregateId)
            .With(e=>e.ChangedAt,ChangedAt)
            .Create();
        
        
        public override Func<Contracts.Reviews.V1.ReviewPublish, Task> GetHandler(SpecificationAggregateStore store)
        {
            return cmd => new ApplicationService(store).Handle(cmd);
        }

       
    }
}