namespace MusicManager.Repositories
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetById(int id);
        void Add(T entity);
        void AddRange(List<T> entity);
        void Update(T entity);
        void Delete(int id);
    }
}
