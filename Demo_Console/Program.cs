﻿// See https://aka.ms/new-console-template for more information

using Demo_Console.Dispose;
using Demo_Console.ElasticSearch;
using Demo_Console.Mapperly;
using Demo_Console.Playground;

Console.WriteLine("Hello, World!");

// var esClient = IndexBuilder.GetElasticClient( new Uri("https://localhost:9200"));
// var workspaceService = new WorkspaceIndexService(esClient);
// await workspaceService.ExecutePath();

DisposeTest.ReadLine();

// var test = MapperlyTest.MappingFromSource();
// Console.ReadLine();