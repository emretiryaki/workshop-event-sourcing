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
    public class approve_review : Spesification<Review,Contracts.Reviews.V1.ReviewApprove>
    {
        public approve_review(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        public readonly Fixture AutoFixture = new Fixture();

        [Fact]
        public void review_content_is_published()
        {
            RaisedEvents.Should().BeEquivalentTo(new Domain.Events.V1.ReviewApproved
            {
                Id = AggregateId,
                 ReviewAt = ChangedAt,
                ReviewBy = Reviewer,
            });
        }

        private Guid AggregateId { get; } = Guid.NewGuid();
        private Guid Reviewer { get; } = Guid.NewGuid();
        private Guid OwnerId { get; } = Guid.NewGuid();
        private DateTime ChangedAt { get; } = DateTime.UtcNow;

        public override object[] Given()
        {
            var obj =new object[2];
            
            obj[0]= AutoFixture.Build<Events.V1.ReviewCreated>()
                .With(e => e.Id, AggregateId)
                .With(e => e.Owner, new UserId(OwnerId))
                .With(e => e.Caption, "First Review")
                .With(e => e.Content, "This is my first review.")
                .Create();
            
            obj[1]= AutoFixture.Build<Events.V1.ReviewPublished>()
                .With(e => e.Id, AggregateId)
                .With(e => e.PublishAt, ChangedAt)
                .Create();

            return obj;

        }

        public override Contracts.Reviews.V1.ReviewApprove When() => AutoFixture
            .Build<Contracts.Reviews.V1.ReviewApprove>()
            .With(e => e.Id, AggregateId)
            .With(e=>e.ReviewAt,ChangedAt)
            .With(e=>e.ReviewBy,Reviewer)
            .Create();
        
        
        public override Func<Contracts.Reviews.V1.ReviewApprove, Task> GetHandler(SpecificationAggregateStore store)
        {
            return cmd => new ApplicationService(store).Handle(cmd);
        }

       
    }
}