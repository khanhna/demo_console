﻿// See https://aka.ms/new-console-template for more information

using Demo_Console.ElasticSearch;

Console.WriteLine("Hello, World!");

var esClient = IndexBuilder.GetElasticClient( new Uri("https://localhost:9200"));
var workspaceService = new WorkspaceIndexService(esClient);
await workspaceService.ExecutePath();