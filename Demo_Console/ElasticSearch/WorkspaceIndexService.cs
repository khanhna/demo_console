using Demo_Console.ElasticSearch.Models;
using Elasticsearch.Net;
using Nest;

namespace Demo_Console.ElasticSearch;

public class WorkspaceIndexService
{
    private readonly IElasticClient _esClient;

    public WorkspaceIndexService(IElasticClient esClient)
    {
        _esClient = esClient;
    }
    
    public async Task ExecutePath()
    {
        // var esClient = IndexBuilder.GetElasticClient( new Uri("https://localhost:9200"));
        // var workspaceService = new WorkspaceIndexService(esClient);

        var cancellationTokenSource = new CancellationTokenSource(15000);
        // await CreateWorkspacesIndex();
        // await SeedData(Constants.IndexName);

        // var test = await GetBySubscriber(Constants.IndexName, Guid.Parse("be287e10-aa3b-4eeb-b66c-99e2b200b762"),
        //     cancellationTokenSource.Token);
        // Console.WriteLine(test.Documents.Count);

        // var getSettlementDateProposal = await GetSettlements(new DateTime(2022, 11, 01),
        //     new DateTime(2022, 11, 30), Constants.IndexName, cancellationTokenSource.Token);
        // if (getSettlementDateProposal.IsValid)
        // {
        //     Console.WriteLine("--> Got {0} participant document from ES", getSettlementDateProposal.Documents.Count);
        // }

        var getInvitations = await GetInvitations(Guid.Parse("be287e10-aa3b-4eeb-b66c-99e2b200b762"),
            new object[] { 2, 3, 4 }, Constants.IndexName, cancellationTokenSource.Token);
        if (getInvitations.IsValid)
        {
            Console.WriteLine("--> Got {0} participant document from ES", getInvitations.Documents.Count);
        }
    }
    
    public async Task CreateWorkspacesIndex(string indexName = Constants.IndexName, CancellationToken cancellationToken = default)
    {
        try
        {   
            var indexExistsResponse = await _esClient.Indices.ExistsAsync(indexName, ct: cancellationToken);
            // if (!indexExistsResponse.IsValid)
            // {
            //     throw new Exception("indexExistsResponse is invalid!");
            // }
            if (indexExistsResponse.Exists)
            {
                var deleteIndexResponse = await _esClient.Indices.DeleteAsync(indexName, ct: cancellationToken);
                if (!deleteIndexResponse.IsValid)
                {
                    throw new Exception("DeleteIndexAsync response is not valid!");
                }
            }
        }
        catch (Exception)
        {
            Console.WriteLine("--> Error deleting index {0} if it exists", indexName);
            throw;
        }

        try
        {
            var createIndexResponse = await _esClient.Indices.CreateAsync(new CreateIndexRequest(indexName), cancellationToken);
            
            if (!createIndexResponse.IsValid)
            {
                throw new Exception("createIndexResponse is invalid!");
            }
        }
        catch (Exception)
        {
            Console.WriteLine("--> Error creating new index for {0}", indexName);
            throw;
        }
    }

    public async Task SeedData(string indexName, CancellationToken cancellationToken = default)
    {
        var firstWorkspace = Guid.Parse("B4A45AAF-974E-45AC-BA26-0545AE1B3956");
        var firstSympliId = "100000091";
        var secondWorkspace = Guid.Parse("99A275A8-C30D-4506-80EC-1ADAE4A2510B");
        var secondSympliId = "100000092";
        var thirdWorkspace = Guid.Parse("24A68322-0217-4D25-9CDE-2B4147FE9BF4");
        var thirdSympliId = "100000093";

        var settlementDate = new DateTime(2022, 10, 10);
        var settlementDateProposal = new DateTime(2022, 11, 11);

        var firstSubscriberId = Guid.NewGuid();
        var secondSubscriberId = Guid.NewGuid();

        var docs = new ParticipantDocument[]
        {
            new()
            {
                Id = Guid.NewGuid(),
                SubscriberId = firstSubscriberId,
                WorkspaceId = firstWorkspace,
                WorkspaceRoles = new()
                {
                    new() { Id = 1, Name = "Purchaser" }
                },
                InvitedBy = new()
                {
                    SubscriberId = firstSubscriberId, WorkspaceRoles = new() { new() { Id = 3, Name = "Vendor" } }
                },
                Workspace = new()
                {
                    Id = firstWorkspace, SympliId = secondSympliId, SettlementDate = settlementDate
                },
            },
            new()
            {
                Id = Guid.NewGuid(),
                SubscriberId = firstSubscriberId,
                WorkspaceId = firstWorkspace,
                WorkspaceRoles = new()
                {
                    new() { Id = 4, Name = "Discharging Mortgagee" }
                },
                InvitedBy = new()
                {
                    SubscriberId = firstSubscriberId, WorkspaceRoles = new() { new() { Id = 3, Name = "Vendor" } }
                },
                Workspace = new()
                {
                    Id = firstWorkspace, SympliId = secondSympliId, SettlementDate = settlementDate
                },
            },
            new()
            {
                Id = Guid.NewGuid(),
                SubscriberId = firstSubscriberId,
                WorkspaceId = firstWorkspace,
                WorkspaceRoles = new()
                {
                    new() { Id = 3, Name = "Vendor" }
                },
                Workspace = new()
                {
                    Id = firstWorkspace, SympliId = secondSympliId, SettlementDate = settlementDate
                },
            },
            new()
            {
                Id = Guid.NewGuid(),
                SubscriberId = firstSubscriberId,
                WorkspaceId = firstWorkspace,
                WorkspaceRoles = new()
                {
                    new() { Id = 2, Name = "Incoming Mortgagee" }
                },
                InvitedBy = new()
                {
                    SubscriberId = firstSubscriberId, WorkspaceRoles = new() { new() { Id = 3, Name = "Vendor" } }
                },
                Workspace = new()
                {
                    Id = firstWorkspace, SympliId = secondSympliId, SettlementDate = settlementDate
                },
            },
            new()
            {
                Id = Guid.NewGuid(),
                SubscriberId = secondSubscriberId,
                WorkspaceId = secondSubscriberId,
                WorkspaceRoles = new()
                {
                    new() { Id = 1, Name = "Purchaser" }
                },
                Workspace = new()
                {
                    Id = firstWorkspace, SympliId = secondSympliId, SettlementDateProposal = settlementDateProposal
                },
            }
        };

        foreach (var doc in docs)
        {
            await _esClient.IndexAsync(doc, id => id.Index(indexName), cancellationToken);
        }
    }

    public Task<ISearchResponse<ParticipantDocument>> GetBySubscriber(string indexName, Guid subscriberId, CancellationToken cancellationToken = default)
    {
        var request = new SearchRequest(Indices.Index(indexName))
        {
            Query = new TermQuery()
            {
                Field = $"subscriberId.keyword",
                Value = subscriberId
            }
        };
        
        var queryJson = _esClient.RequestResponseSerializer.SerializeToString(request);
        Console.WriteLine($"--> Elastic Search query: \n {queryJson}");
        
        return _esClient.SearchAsync<ParticipantDocument>(request, cancellationToken);
    }
    
    public Task<ISearchResponse<ParticipantDocument>> GetSettlements(DateTime fromUtc, DateTime toUtc, string indexName, CancellationToken cancellationToken = default)
    {
        var request = new SearchRequest(Indices.Index(indexName))
        {
            Query = new BoolQuery()
            {
                Should = new QueryContainer[]
                {
                    new BoolQuery()
                    {
                        Must = new QueryContainer[]
                        {
                            new ExistsQuery{Field = "Workspace.settlementDate"},
                            new DateRangeQuery{Field = "Workspace.settlementDate", GreaterThanOrEqualTo = fromUtc, LessThanOrEqualTo = toUtc}
                        }
                    },
                    new BoolQuery()
                    {
                        Must = new QueryContainer[]
                        {
                            new ExistsQuery{Field = "Workspace.settlementDateProposal"},
                            new DateRangeQuery{Field = "Workspace.settlementDateProposal", GreaterThanOrEqualTo = fromUtc, LessThanOrEqualTo = toUtc}
                        }
                    }
                }
            }
        };
        
        var queryJson = _esClient.RequestResponseSerializer.SerializeToString(request);
        Console.WriteLine($"--> Elastic Search query: \n {queryJson}");
        return _esClient.SearchAsync<ParticipantDocument>(request, cancellationToken);
    }
    
    public Task<ISearchResponse<ParticipantDocument>> GetInvitations(Guid subscriberId, object[] workspaceRoles, string indexName, CancellationToken cancellationToken = default)
    {
        var request = new SearchRequest(Indices.Index(indexName))
        {
            Query = new BoolQuery()
            {
                Should = new QueryContainer[]
                {
                    new BoolQuery()
                    {
                        Must = new QueryContainer[]
                        {
                            new TermQuery{Field = "subscriberId.keyword", Value = subscriberId},
                            new TermQuery{Field = "InvitedByParticipant.subscriberId.keyword", Value = subscriberId},
                            new BoolQuery
                            {
                                Should = new QueryContainer[]
                                {
                                    new TermsQuery{Field = "WorkspaceRole.id", Terms = workspaceRoles},
                                    new TermsQuery{Field = "InvitedByParticipant.WorkspaceRole.id", Terms = workspaceRoles}
                                }
                            }
                        }
                    },
                    new BoolQuery()
                    {
                        Must = new QueryContainer[]
                        {
                            new TermQuery{Field = "subscriberId.keyword", Value = subscriberId},
                            new TermsQuery{Field = "WorkspaceRole.id", Terms = workspaceRoles}
                        }
                    },
                    new BoolQuery()
                    {
                        Must = new QueryContainer[]
                        {
                            new TermQuery{Field = "InvitedByParticipant.subscriberId.keyword", Value = subscriberId},
                            new TermsQuery{Field = "InvitedByParticipant.WorkspaceRole.id", Terms = workspaceRoles}
                        }
                    }
                }
            }
        };
        
        var queryJson = _esClient.RequestResponseSerializer.SerializeToString(request);
        Console.WriteLine($"--> Elastic Search query: \n {queryJson}");
        return _esClient.SearchAsync<ParticipantDocument>(request, cancellationToken);
    }
}