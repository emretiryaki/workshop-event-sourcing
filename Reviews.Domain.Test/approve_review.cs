using System;
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
                Caption = Caption,
                Owner = OwnerId,
                Content = Content
            });
        }

        private Guid AggregateId { get; } = Guid.NewGuid();
        private Guid Reviewer { get; } = Guid.NewGuid();
        private Guid OwnerId { get; } = Guid.NewGuid();
        private DateTime ChangedAt { get; } = DateTime.UtcNow;
        private string Caption { get; } = "First Review";
        private string Content { get; } = "This is my first review.";
        public override object[] Given()
        {
            var obj =new object[2];
            
            obj[0]= AutoFixture.Build<Events.V1.ReviewCreated>()
                .With(e => e.Id, AggregateId)
                .With(e => e.Owner, OwnerId)
                .With(e => e.Caption, Caption)
                .With(e => e.Content, Content)
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