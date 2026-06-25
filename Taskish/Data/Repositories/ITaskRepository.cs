using Taskish.Models;

namespace Taskish.Data.Repositories
{
    public interface ITaskRepository
    {
        List<TaskItem> GetAll();
        TaskItem? GetById(Guid id);
        void Add(TaskItem task);
        void Update(TaskItem task);
        void Delete(Guid id);
    }
}
