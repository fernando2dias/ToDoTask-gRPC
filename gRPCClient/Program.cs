using Grpc.Net.Client;
using GrpcServer;
using System;
using System.Threading.Tasks;


var channel = GrpcChannel.ForAddress("https://localhost:7136");
var client = new TaskTracker.TaskTrackerClient(channel);
string[] tag = {"Baixa", "Normal", "Urgente"}; 

while (true)
{
    Console.WriteLine("\n----------------------");
    Console.WriteLine("Gerenciador de tarefas");
    Console.WriteLine("----------------------\n");
    Console.WriteLine("Escolha uma opção:");
    Console.WriteLine("1. Criar Tarefa");
    Console.WriteLine("2. Listar Tarefas");
    Console.WriteLine("3. Executar Tarefa");
    Console.WriteLine("4. Finalizar Tarefa");
    Console.WriteLine("5. Remover Tarefa");
    Console.WriteLine("6. Sair");
    Console.Write("Opção: ");

    if (int.TryParse(Console.ReadLine(), out int choice))
    {
        switch (choice)
        {
            case 1:
                Console.Write("\nTítulo da Tarefa: ");
                string title = Console.ReadLine();
                
                Console.Write("\nConteúdo da Tarefa: ");
                string content = Console.ReadLine();
                
                Console.WriteLine("\nEscolha uma opção para definir uma prioridade:");
                Console.WriteLine("0 - Baixa prioridade");
                Console.WriteLine("1 - Média prioridade");
                Console.WriteLine("2 - Alta prioridade");
                
                Console.Write("\nOpção: ");

                var p = Int32.TryParse(Console.ReadLine(),out var priority);
               
                var createResponse = await client.CreateTaskAsync(new CreateTaskRequest
                {
                    Title = title,
                    Content = content,
                    Tag = (TaskPriority)priority
                });

                Console.WriteLine($"\nTarefa criada com ID: {createResponse.TaskId}\n");
                await channel.ShutdownAsync();
                break;

            case 2:
                var listResponse = await client.ListTaskAsync(new ListTaskRequest());
                Console.WriteLine("\nLista de Tarefas:");
                foreach (var task in listResponse.List)
                {
                    Console.WriteLine($"ID: {task.Id}\n Título: {task.Title}\n Prioridade: {GetPriority(task.Tag.ToString())}\n");
                    Console.WriteLine("");
                }
                break;

            case 3:
                Console.Write("ID da Tarefa a ser executada: ");
                if (int.TryParse(Console.ReadLine(), out int taskId))
                {
                    var executeResponse = await client.ExecuteTaskAsync(new ExecuteTaskRequest
                    {
                        TaskId = taskId
                    });

                    if (executeResponse.Error == 0)
                    {
                        Console.WriteLine($"Tarefa: {taskId} estã sendo executada...");
                    }
                    else
                    {
                        Console.WriteLine($"Erro ao tentar executar a Tarefa ID: {taskId}");
                    }
                }
                break;

            case 4:
                Console.Write("ID da Tarefa a ser finalizada: ");
                if (int.TryParse(Console.ReadLine(), out int finalizeTaskId))
                {
                    var finalizeResponse = await client.FinalizeTaskAsync(new FinalizeTaskRequest
                    {
                        TaskId = finalizeTaskId
                    });

                    if (finalizeResponse.Error == 0)
                    {
                        Console.WriteLine($"Tarefa ID: {finalizeTaskId} finalizada com sucesso!");
                    }
                    else
                    {
                        Console.WriteLine($"Erro ao finalizar a Tarefa ID: {finalizeTaskId}");
                    }
                }
                break;

            case 5:
                Console.Write("ID da Tarefa a ser removida: ");
                if (int.TryParse(Console.ReadLine(), out int removeTaskId))
                {
                    var removeResponse = await client.RemoveTaskAsync(new RemoveTaskRequest
                    {
                        TaskId = removeTaskId
                    });

                    if (removeResponse.Error == 0)
                    {
                        Console.WriteLine($"Tarefa ID: {removeTaskId} removida com sucesso!");
                    }
                    else
                    {
                        Console.WriteLine($"Erro ao remover a Tarefa ID: {removeTaskId}");
                    }
                }
                break;

            case 6:
                Environment.Exit(0);
                break;

            default:
                Console.WriteLine("Opção inválida. Tente novamente.");
                break;
        }
    }
    else
    {
        Console.WriteLine("Opção inválida. Tente novamente.");
    }
}


string GetPriority(string p)
{
    if (p == "TpPriority") return "alta";
    if (p == "TpUrgent") return "normal";
    return "baixa";
}



//async void CreateTask()
//{
//    string title = "";
//    string description = "";
//    int priority = 0;
//    TaskPriority _taskPriority;

//    Console.Write("Escreva o Titulo da Tarefa: ");
//    title = Console.ReadLine();

//    Console.Write("Escreva a descriçao da tarefa: ");
//    description = Console.ReadLine();

//    Console.WriteLine("\nDigite uma opçao para definir a prioridade: " );
//    Console.WriteLine("0 - Prioridade baixa");
//    Console.WriteLine("1 - Prioridade normal");
//    Console.WriteLine("2 - Prioridade alta");
//    Console.Write("Opçao: ");
//    do { 
//        priority = Int32.Parse(Console.ReadLine());
//    }while(priority != 0 && priority != 1 && priority != 2);

//    var createResponse = await client.CreateTaskAsync(

//    new CreateTaskRequest
//    {
//        Title = title,
//        Content = description,
//        Tag = (TaskPriority)priority
//    });


//}


