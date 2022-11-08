using Nest;

namespace Demo_Console.ElasticSearch.Models;

[ElasticsearchType(RelationName = "participant", IdProperty = "Id")]
public class ParticipantDocument
{
    [Text]
    public Guid Id { get; set; }
    [Keyword]
    public Guid WorkspaceId { get; set; }
    [Keyword]
    public Guid SubscriberId { get; set; }
    [Nested(Name = nameof(WorkspaceRole))]
    public List<WorkspaceRole> WorkspaceRoles { get; set; } = new ();
    [Nested(Name = nameof(InvitedByParticipant))]
    public InvitedByParticipant InvitedBy { get; set; }
    [Nested(Name = nameof(Workspace))]
    public Workspace Workspace { get; set; }
}

[ElasticsearchType(RelationName = nameof(WorkspaceRole))]
public class WorkspaceRole
{
    [Number]
    public int Id { get; set; }
    [Text]
    public string Name { get; set; }
}

[ElasticsearchType(RelationName = nameof(InvitedByParticipant))]
public class InvitedByParticipant
{
    [Text]
    public Guid ParticipantId { get; set; }
    [Text]
    public Guid SubscriberId { get; set; }
    [Text]
    public Guid GroupId { get; set; }
    [Nested(Name = nameof(WorkspaceRole))]
    public List<WorkspaceRole> WorkspaceRoles { get; set; }
}

[ElasticsearchType(RelationName = nameof(Workspace))]
public class Workspace
{
    [Text]
    public Guid Id { get; set; }
    [Text]
    public string SympliId { get; set; }
    [Date]
    public DateTime? SettlementDate { get; set; }
    [Date]
    public DateTime? SettlementDateProposal { get; set; }
}