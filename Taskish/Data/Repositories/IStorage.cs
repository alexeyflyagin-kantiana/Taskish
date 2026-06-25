namespace Taskish.Data.Repositories
{
    public interface IStorage
    {
        public List<T> Load<T>();
        public void Save<T>(List<T> data);
    }
}
