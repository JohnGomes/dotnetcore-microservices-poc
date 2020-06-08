using MediatR;
using PolicySearchService.Api.Queries;
using PolicySearchService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PolicySearchService.Queries
{
    public class FindPolicyHandler : IRequestHandler<FindPolicyQuery, FindPolicyResult>
    {
        private readonly IPolicyRepository policis;

        public FindPolicyHandler(IPolicyRepository policis)
        {
            this.policis = policis;
        }

        public async Task<FindPolicyResult> Handle(FindPolicyQuery request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"@@@@@@@@@@@@@@@@@@@@@@@ Handle Policy Query {JsonConvert.SerializeObject(request)}");
   
            var searchResults = await policis.Find(request.QueryText);

            return FindPolicyResult(searchResults);
        }

        private FindPolicyResult FindPolicyResult(List<Policy> searchResults)
        {
            return new FindPolicyResult
            {
                Policies = searchResults.Select(p => new Api.Queries.Dtos.PolicyDto
                {
                    PolicyNumber = p.PolicyNumber,
                    PolicyStartDate = p.PolicyStartDate,
                    PolicyEndDate = p.PolicyEndDate,
                    ProductCode = p.ProductCode,
                    PolicyHolder = p.PolicyHolder,
                    PremiumAmount = p.PremiumAmount
                })
                .ToList()
            };
        }
    }
}
