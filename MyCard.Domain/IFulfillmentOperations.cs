using Microsoft.Marketplace.SaaS.Models;
using Microsoft.Rest.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyCard.Domain
{
   public interface IFulfillmentOperations
    {
        Task<AzureOperationResponse<ResolvedSubscription>> ResolveWithHttpMessagesAsync(string xMsMarketplaceToken, System.Guid? requestId = default(System.Guid?), System.Guid? correlationId = default(System.Guid?), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}
