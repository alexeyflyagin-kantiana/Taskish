
using Taskish.Data.Repositories;
using Taskish.Models;

namespace Taskish.Services
{
    public class TaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public List<TaskItem> GetAll()
        {
            return _taskRepository.GetAll();
        }

        public TaskItem? GetById(Guid id)
        {
            return _taskRepository.GetById(id);
        }

        public void Add(TaskItem task)
        {
            if (task.Deadline.HasValue && task.Deadline < task.CreatedAt)
                throw new ArgumentException("Deadline cannot be earlier than date of creation");

            _taskRepository.Add(task);
        }

        public void Update(TaskItem task)
        {
            var existing = _taskRepository.GetById(task.Id);
            if (existing == null) throw new ArgumentException("Task was not found");
            if (existing.IsCompleted) throw new InvalidOperationException("Updating of a completed task is not allowed");

            task.CreatedAt = existing.CreatedAt;
            task.CompletedAt = existing.CompletedAt;

            _taskRepository.Update(task);
        }

        public void Complete(Guid id)
        {
            var task = _taskRepository.GetById(id);
            if (task == null) throw new ArgumentException("Task was not found");

            task.IsCompleted = true;
            task.CompletedAt = DateTime.Now;
            _taskRepository.Update(task);
        }

        public void Uncomplete(Guid id)
        {
            var task = _taskRepository.GetById(id);
            if (task == null) throw new ArgumentException("Task was not found");

            task.IsCompleted = false;
            task.CompletedAt = null;
            _taskRepository.Update(task);
        }

        public void Delete(Guid id)
        {
            var existing = _taskRepository.GetById(id);
            if (existing == null) throw new ArgumentException("Task was not found");

            _taskRepository.Delete(id);
        }
    }
}
