using Grpc.Core;
using Grpc.Net.Client;
using GrpcServer;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;


var channel = GrpcChannel.ForAddress("https://localhost:7136");
var client = new TaskTracker.TaskTrackerClient(channel);
string[] tag = {"TODAS","COMUM", "PRIORIDADE", "URGENTE"};
string[] filter = {"TF_ALL", "TF_COMMON", "TF_PRIORITY", "TF_URGENT"};
string[] queue = {"PARA FAZER","FAZENDO","FINALIZADAS","TODAS"};

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
                Console.Clear();
                Console.Write("\nTítulo da Tarefa: ");
                string title;
                string text;
                int times = 0;
                do
                {
                    if(times == 0)
                    {
                        text = Console.ReadLine();
                        times++;
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Você digitou algum caracter não suportado\nou acima de 20 caracteres.\nTente novamente!!!");
                        Console.Write("\nTítulo da Tarefa: ");

                        text = Console.ReadLine();
                        times++;
                    }
                  
                } while (TextValidate(text) && TextLengthValidate(text));
                title = text;

                
                Console.Write("\nConteúdo da Tarefa: ");
                string content = Console.ReadLine();
                
                Console.WriteLine("\nEscolha uma opção para definir uma prioridade:");
                Console.WriteLine("0 - Comum");
                Console.WriteLine("1 - Prioridade");
                Console.WriteLine("2 - Urgente");
                
                Console.Write("\nOpção: ");

                var p = Int32.TryParse(Console.ReadLine(),out var priority);
                try
                {
                    var createResponse = await client.CreateTaskAsync(new CreateTaskRequest
                    {
                        Title = title,
                        Content = content,
                        Tag = (TaskPriority)priority
                    });

                    Console.WriteLine($"\nTarefa criada com ID: {createResponse.TaskId}\n");
                    await channel.ShutdownAsync();
                }
                catch (RpcException ex)
                {
                    Console.WriteLine($"Erro ao tentar criar uma tarefa: {ex.Message}" );
                }
                
                Console.WriteLine("Aperte qualquer tecla para continuar...");
                Console.ReadKey();
                Console.Clear();

                break;

            case 2:
                Console.Clear();
                Console.WriteLine("\nEscolha uma opção para definir um filtro de prioridade:");
                Console.WriteLine("0 - Todas");
                Console.WriteLine("1 - Comum");
                Console.WriteLine("2 - Prioridade");
                Console.WriteLine("3 - Urgente");

                Console.Write("\nOpção: ");
                var f = Int32.TryParse(Console.ReadLine(), out var taskFilter);

                Console.Clear();
                Console.WriteLine("\nEscolha uma opção de fila da tarefa:");
                Console.WriteLine("0 - Para fazer");
                Console.WriteLine("1 - Fazendo");
                Console.WriteLine("2 - Finalizada");
                Console.WriteLine("3 - Todas");

                Console.Write("\nOpção: ");
                var q = Int32.TryParse(Console.ReadLine(), out var taskQueue);

                try
                {
                    var listResponse = await client.ListTaskAsync(new ListTaskRequest
                                                                 {Filter=(TaskFilter)taskFilter, 
                                                                 Q = (TaskQueue)taskQueue});

                    Console.WriteLine("-----------------------------------");
                    Console.WriteLine("**********Lista de Tarefas*********");
                    Console.WriteLine("-----------------------------------");
                    Console.WriteLine($"Filtro de Prioridade: {tag[taskFilter]}");
                    Console.WriteLine($"Filtro de Fila: {queue[taskQueue]}");
                    Console.WriteLine("-----------------------------------");

                    foreach (var task in listResponse.List)
                    {
                        Console.WriteLine($"ID:{task.Id}\nTítulo: {task.Title}\nTag: {GetPriority(task.Tag.ToString())}");
                        Console.WriteLine("-----------------------------------");
                    }
                }
                catch (RpcException ex)
                {
                    Console.WriteLine($"Erro ao tentar carregar as tarefas: {ex.Message}");
                }
                
                Console.WriteLine("Aperte qualquer tecla para continuar...");
                Console.ReadKey();
                Console.Clear();
                break;

            case 3:
                Console.Clear();
                Console.Write("ID da Tarefa a ser executada: ");

                try
                {
                    if (int.TryParse(Console.ReadLine(), out int taskId))
                    {
                        var executeResponse = await client.ExecuteTaskAsync(new ExecuteTaskRequest
                        {
                            TaskId = taskId
                        });

                        if (executeResponse.Error == 0)
                        {
                            Console.WriteLine($"Tarefa: {taskId} está sendo executada...\n");
                            Console.WriteLine("Aperte qualquer tecla para continuar...");
                            Console.ReadKey();
                            Console.Clear();
                        }
                        else
                        {
                            Console.WriteLine($"Erro ao tentar executar a Tarefa ID: {taskId}");
                            Console.WriteLine("Aperte qualquer tecla para continuar...");
                            Console.ReadKey();
                            Console.Clear();
                        }
                    }
                }
                catch (RpcException ex)
                {
                    Console.WriteLine($"Erro ao tentar carregar as tarefas: {ex.Message}");
                }
                
                break;

            case 4:
                Console.Clear();
                try
                {
                    Console.Write("ID da Tarefa a ser finalizada: ");
                    if (int.TryParse(Console.ReadLine(), out int finalizeTaskId))
                    {
                        var finalizeResponse = await client.FinalizeTaskAsync(new FinalizeTaskRequest
                        {
                            TaskId = finalizeTaskId
                        });

                        if (finalizeResponse.Error == 0)
                        {
                            Console.WriteLine($"Tarefa ID: {finalizeTaskId} finalizada com sucesso!\n");

                            Console.WriteLine("Aperte qualquer tecla para continuar...");
                            Console.ReadKey();
                            Console.Clear();
                        }
                        else
                        {
                            Console.WriteLine($"Erro ao finalizar a Tarefa ID: {finalizeTaskId}");
                        }
                    }
                }
                catch (RpcException ex)
                {
                    Console.WriteLine($"Erro ao tentar finalizar a tarefa: {ex.Message}");
                }
                
                break;

            case 5:
                Console.Clear();

                try
                {
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
                        Console.WriteLine("Aperte qualquer tecla para continuar...");
                        Console.ReadKey();
                        Console.Clear();
                    }

                }
                catch (RpcException ex)
                {
                    Console.WriteLine($"Erro ao tentar remover a tarefa: {ex.Message}");
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
    if (p == "TpPriority") return "urgente";
    if (p == "TpUrgent") return "prioridade";
    return "comum";
}

bool TextValidate(string text)
{
    return (!Regex.IsMatch(text, "^[a-zA-Z0-9#\\$%&/]*$")) ? true : false;
}

bool TextLengthValidate(string text)
{
   return (text.Length >= 20) ? true : false;
}