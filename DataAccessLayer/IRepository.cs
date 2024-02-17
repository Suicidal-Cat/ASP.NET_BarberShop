namespace DataAccessLayer
{
	public interface IRepository<T> where T : class
	{
		IQueryable<T> GetByCondition(Func<T, bool> predicate);
		public List<T> GetAll();
		public void Add(T t);
		public void Update(T t);
		public void Delete(T t);
		public T GetById(int id);
	}
}