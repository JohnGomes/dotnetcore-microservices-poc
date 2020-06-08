using Nest;
using PolicySearchService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace PolicySearchService.DataAccess.ElasticSearch
{
    public class PolicyRepository : IPolicyRepository
    {
        private readonly ElasticClient elasticClient;
        private ILogger<PolicyRepository> _logger;

        public PolicyRepository(ElasticClient elasticClient, ILogger<PolicyRepository> logger)
        {
            this.elasticClient = elasticClient;
            _logger = logger;
        }

        public async Task Add(Policy policy)
        {
            await elasticClient.IndexDocumentAsync(policy);
        }

        public async Task<List<Policy>> Find(string queryText)
        {
            _logger.LogInformation($"@@@@@@@@@@@@@@@@@@@@@@@ Query Text {queryText}");

            var result = await elasticClient
                .SearchAsync<Policy>(
                    s =>
                        s.From(0)
                        .Size(10)
                        .Query(q =>
                            q.MultiMatch(mm =>
                                mm.Query(queryText)
                                .Fields(f => f.Fields(p => p.PolicyNumber, p => p.PolicyHolder))
                                .Type(TextQueryType.BestFields)
                                .Fuzziness(Fuzziness.Auto)
                            )
                    ));

           return result.Documents.ToList();
        }
    }
}
