using Elasticsearch.Net;
using Nest;
using Nest.JsonNetSerializer;
using Newtonsoft.Json;

namespace Demo_Console.ElasticSearch;

public class IndexBuilder
{
    public static IElasticClient GetElasticClient(Uri connectionString)
    {
        // var connectionSettings = new ConnectionSettings(new SingleNodeConnectionPool(connectionString),
        //     sourceSerializer: (builtin, settings) => new JsonNetSerializer(builtin, settings,
        //         () => new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include }));
        var connectionSettings = new ConnectionSettings(new SingleNodeConnectionPool(connectionString));
        
        return new ElasticClient(connectionSettings
            .ServerCertificateValidationCallback((o, certificate, chain, errors) => true)
            .ServerCertificateValidationCallback(CertificateValidations.AllowAll)
            .DisableDirectStreaming()
            .BasicAuthentication("elastic", "CYyehUoqDdkjDPbYU2HB"));
    }
}