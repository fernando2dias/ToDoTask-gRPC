using Grpc.Net.Client;
using GrpcServer;
using System;
using System.Threading.Tasks;


var channel = GrpcChannel.ForAddress("https://localhost:7136");
var client = new TaskTracker.TaskTrackerClient(channel);


Menu();
static void Menu() {
    string option = "";
    Console.WriteLine("-----------------------------------------");
    Console.WriteLine("Seja bem ao seu gerenciador de Tarefas!");
    Console.WriteLine("-----------------------------------------");
    Console.WriteLine("Aperte uma opção para prosseguir:\n");
    Console.WriteLine("1: Criar uma nova tarefa");
    Console.WriteLine("2: Listar todas tarefas");
    Console.WriteLine("3: Executar uma tarefa");
    Console.WriteLine("4: Remover uma tarefa`\n");
    Console.Write("Digite sua opção: ");
    option = Console.ReadLine();

    switch (option)
    {
        case "1":
            Console.WriteLine("Opção 1 selecionada: Criar uma nova tarefa");
            // Adicione aqui o código para criar uma nova tarefa
            break;
        case "2":
            Console.WriteLine("Opção 2 selecionada: Listar todas as tarefas");
            // Adicione aqui o código para listar todas as tarefas
            break;
        case "3":
            Console.WriteLine("Opção 3 selecionada: Executar uma tarefa");
            // Adicione aqui o código para executar uma tarefa
            break;
        case "4":
            Console.WriteLine("Opção 4 selecionada: Remover uma tarefa");
            // Adicione aqui o código para remover uma tarefa
            break;
        case "5":
            Console.WriteLine("Saindo do programa. Até logo!");
            return;
        default:
            Console.WriteLine("Opção inválida. Por favor, escolha uma opção válida.");
            break;
    }
}






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