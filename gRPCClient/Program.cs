using Grpc.Net.Client;
using GrpcServer;
using System;
using System.Threading.Tasks;


var channel = GrpcChannel.ForAddress("https://localhost:7136");
var client = new TaskTracker.TaskTrackerClient(channel);

var createResponse = await client.CreateTaskAsync(
    new CreateTaskRequest
    {
        Title = "Minha tarefa 01",
        Content = "Uma descrição qualquer",
        Tag = TaskPriority.TpCommon
    });

Console.WriteLine($"Tarefa criada com o ID: {createResponse.TaskId}");

// Feche o canal gRPC quando não for mais necessário
channel.ShutdownAsync().Wait();

Console.WriteLine("Pressione qualquer tecla para sair...");
Console.ReadKey();