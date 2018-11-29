using System;
using System.Threading.Tasks;
using Reviews.Core;
using Reviews.Domain;

namespace Reviews.Service.WebApi.Modules.Reviews
{
    public interface  IDomainService{}
    
    public class ApplicationService : IDomainService
    {
        private IAggrigateStore aggrigateStore { get; }

        public ApplicationService(IAggrigateStore store)
        {
            aggrigateStore = store;
        }

        public Task Handle(Contracts.Reviews.V1.ReviewCreate command) =>
            aggrigateStore.Save(Domain.Review.Create(command.Id,command.Owner,command.Caption,command.Content));

        public Task Handle(Contracts.Reviews.V1.ReviewApprove command) => 
            HandleForUpdate(command.Id, r => r.Approve(new UserId(command.ReviewBy), command.ReviewAt));

        public async Task Handle(Contracts.Reviews.V1.UpdateReview command) =>
            HandleForUpdate(command.Id, r => r.UpdateCaptionAndContent(command.Caption, command.Content,command.ChangedAt));


        public async Task Handle(Contracts.Reviews.V1.ReviewPublish command)
            => HandleForUpdate(command.Id, r => r.Publish(command.ChangedAt));
        
        private async Task HandleForUpdate(Guid aggregateId, Action<Domain.Review> handle)
        {
            var aggregate = await aggrigateStore.Load<Domain.Review>(aggregateId.ToString());
            handle(aggregate);
            await aggrigateStore.Save(aggregate);
        }
        
    }
}
