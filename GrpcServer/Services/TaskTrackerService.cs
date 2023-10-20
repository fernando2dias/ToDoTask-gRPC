using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace GrpcServer.Services
{
    public class TaskTrackerService : TaskTracker.TaskTrackerBase
    {
        private static List<TaskModel> tasks = new List<TaskModel>();
        private static int taskIdCounter = 1;
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
            _logger.LogInformation($"Task {newTask.Id} created");

            return Task.FromResult(new CreateTaskResponse { TaskId = newTask.Id });

        }

        public override Task<ListTaskResponse> ListTask(ListTaskRequest request, ServerCallContext context)
        {
            var response = new ListTaskResponse();

            if (request.Filter == TaskFilter.TfAll)
            {
                foreach (var task in tasks)
                {
                    if (request.Q != TaskQueue.TqTodo &&
                       request.Q != TaskQueue.TqDone &&
                       request.Q != TaskQueue.TqDoing
                    )
                    {
                        response.List.Add(task);
                    }
                    else if (request.Q == TaskQueue.TqDone && task.Ended != null)
                    {
                        response.List.Add(task);
                    }
                    else if (request.Q == TaskQueue.TqDoing && task.Started != null && task.Ended == null)
                    {
                        response.List.Add(task);
                    }
                    else if (request.Q == TaskQueue.TqTodo && task.Created != null)
                    {
                        response.List.Add(task);
                    }
                }
            }
            else
            {
                foreach (var task in tasks)
                {
                    if (task.Tag == (TaskPriority)request.Filter - 1)
                    {
                        if (request.Q != TaskQueue.TqTodo &&
                       request.Q != TaskQueue.TqDone &&
                       request.Q != TaskQueue.TqDoing)
                        {
                            response.List.Add(task);
                        }
                        else if (request.Q == TaskQueue.TqDone && task.Ended != null)
                        {
                            response.List.Add(task);
                        }
                        else if (request.Q == TaskQueue.TqDoing && task.Started != null && task.Ended == null)
                        {
                            response.List.Add(task);
                        }
                        else if (request.Q == TaskQueue.TqTodo && task.Created != null)
                        {
                            response.List.Add(task);
                        }
                    }
                }
            }

            return Task.FromResult(response);
        }


        public override Task<ExecuteTaskResponse> ExecuteTask(ExecuteTaskRequest request, ServerCallContext context)
        {
            var task = tasks.FirstOrDefault(t => t.Id == request.TaskId);

            if (task == null || task.Ended != null)
            {
                return Task.FromResult(new ExecuteTaskResponse { Error = -1 });
            }

            task.Started = Timestamp.FromDateTime(DateTime.UtcNow);

            return Task.FromResult(new ExecuteTaskResponse { Error = 0 });
        }

        public override Task<FinalizeTaskResponse> FinalizeTask(FinalizeTaskRequest request, ServerCallContext context)
        {
            var task = tasks.FirstOrDefault(t => t.Id == request.TaskId);

            if (task == null)
            {
                return Task.FromResult(new FinalizeTaskResponse { Error = -1 });
            }

            if (task.Started == null)
            {
                Console.WriteLine("A tarefa não foi iniciada ainda, portanto não pode ser finalizada!");
                return Task.FromResult(new FinalizeTaskResponse { Error = -1 });
            }

            task.Ended = Timestamp.FromDateTime(DateTime.UtcNow);
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
