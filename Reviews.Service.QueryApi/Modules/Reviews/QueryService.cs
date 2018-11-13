using System;
using System.Linq;
using Raven.Client.Documents.Session;

namespace Reviews.Service.QueryApi.Modules.Reviews
{
    public class ReviewQueryService
    {
        private readonly Func<IAsyncDocumentSession> getSession;

        public ReviewQueryService(Func<IAsyncDocumentSession> session) => getSession = session;

    }
}
