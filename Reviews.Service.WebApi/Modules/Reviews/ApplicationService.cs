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


        private async Task HandleForUpdate(Guid aggregateId, Action<Domain.Review> handle)
        {
            var aggregate = await aggrigateStore.Load<Domain.Review>(aggregateId.ToString());
            handle(aggregate);
            await aggrigateStore.Save(aggregate);
        }
        
    }
}
