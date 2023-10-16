using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace GrpcServer.Services
{
    public class TaskTrackerService : TaskTracker.TaskTrackerBase
    {
        private List<TaskModel> tasks = new List<TaskModel>();
        private int taskIdCounter = 1;
        private readonly ILogger<TaskTrackerService> _logger;


        public TaskTrackerService(ILogger<TaskTrackerService> logger)
        {
            _logger = logger;
        }


        public override Task<CreateTaskResponse> CreateTask(CreateTaskRequest request, ServerCallContext context)
        {
            var newTask = new TaskModel
            {
                Id = taskIdCounter++,
                Title = request.Title,
                Content = request.Content,
                 Tag = request.Tag,
                Created = Timestamp.FromDateTime(DateTime.UtcNow)
            };

            tasks.Add(newTask);

            return Task.FromResult(new CreateTaskResponse { TaskId = newTask.Id });

        }

        public override Task<ListTaskResponse> ListTask(ListTaskRequest request, ServerCallContext context)
        {
            var response = tasks.ToList();
            //depois vou tentar filtrar por prioridade

            return Task.FromResult(new ListTaskResponse { List = { tasks } });
        }

        public override Task<ExecuteTaskResponse> ExecuteTask(ExecuteTaskRequest request, ServerCallContext context)
        {
            var task = tasks.FirstOrDefault(t => t.Id == request.TaskId);

            if (task == null)
            {
                return Task.FromResult(new ExecuteTaskResponse { Error = -1 });
            }

           // vai executar algo?

            return Task.FromResult(new ExecuteTaskResponse { Error = 0 });
        }

        public override Task<FinalizeTaskResponse> FinalizeTask(FinalizeTaskRequest request, ServerCallContext context)
        {
            var task = tasks.FirstOrDefault(t => t.Id == request.TaskId);

            if (task == null)
            {
                return Task.FromResult(new FinalizeTaskResponse { Error = -1 });
            }

            Console.WriteLine($"Tarefa ID: {request.TaskId} foi finalizada com sucesso!");

            return Task.FromResult(new FinalizeTaskResponse { Error = 0 });
        }

        public override Task<RemoveTaskResponse> RemoveTask(RemoveTaskRequest request, ServerCallContext context)
        {
            var task = tasks.FirstOrDefault(t => t.Id == request.TaskId);

            if (task == null)
            {
                return Task.FromResult(new RemoveTaskResponse { Error = -1 });
            }

            tasks.Remove(task);

            return Task.FromResult(new RemoveTaskResponse { Error = 0 });
        }


    }
}
