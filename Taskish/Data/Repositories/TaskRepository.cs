using Taskish.Models;

namespace Taskish.Data.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly IStorage _storage;
        private List<TaskItem> _cache;

        public TaskRepository(IStorage storage)
        {
            _storage = storage;
            _cache = _storage.Load<TaskItem>();
        }

        public List<TaskItem> GetAll() => _cache;

        public TaskItem? GetById(Guid id)
            => _cache.FirstOrDefault(t => t.Id == id);

        public void Add(TaskItem task)
        {
            _cache.Add(task);
            _storage.Save(_cache);
        }

        public void Update(TaskItem task)
        {
            var index = _cache.FindIndex(t => t.Id == task.Id);
            if (index >= 0) _cache[index] = task;
            _storage.Save(_cache);
        }

        public void Delete(Guid id)
        {
            _cache.RemoveAll(t => t.Id == id);
            _storage.Save(_cache);
        }
    }
}
