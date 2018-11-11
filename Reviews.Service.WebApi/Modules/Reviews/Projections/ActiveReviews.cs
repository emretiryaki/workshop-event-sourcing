using System;
using System.Threading.Tasks;
using Reviews.Core.Projections;

namespace Reviews.Service.WebApi.Modules.Reviews.Projections
{
    public class ActiveReviews : Projection
    {
        public override Task Handle(object e)
        {
            throw new System.NotImplementedException();
        }
        
        
        private static string DocumentId(Guid id) => $"ActiveReviews/{id}";
    }
}
