
using DineEase.Data;
using Microsoft.EntityFrameworkCore;

namespace DineEase.Models
{
	public class Repository<T> : IRepository<T> where T : class
	{
		protected ApplicationDbContext _context;
		private DbSet<T> _entities;
		public Repository(ApplicationDbContext context)
		{
			_context = context;
			_entities = context.Set<T>();

		}
		public async Task AddAsync(T entity)
		{
			await _entities.AddAsync(entity);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(int id)
		{
			await _entities.FindAsync(id);
			if (id == null)
			{
				throw new ArgumentNullException(nameof(id));
			}
			else
			{
				_entities.Remove(_entities.Find(id));
				await _context.SaveChangesAsync();
			}
		}

		public async Task<IEnumerable<T>> GetAllAsync()
		{
			var query = _entities.AsQueryable().ToList();
			return await Task.FromResult(query.AsEnumerable());
		}

		public async Task<IEnumerable<T>> GetAllByIdAsync<TKey>(TKey id, string propertyName, QueryOptions<T> options)
		{
			IQueryable<T> query = _entities;
			if (options.HasWhere)
			{
				query = query.Where(options.Where);
			}
			if (options.HasOrderBy)
			{
				query = query.OrderBy(options.OrderBy);
			}
			foreach (var include in options.GetIncludes())
			{
				query = query.Include(include);
			}

			// Filter by the specified property name and id --> TODO: need to check this logic
			query = query.Where(e => EF.Property<TKey>(e, propertyName).Equals(id));

			return await query.ToListAsync();
		}

		public async Task<T> GetByIdAsync(int id, QueryOptions<T> options)
		{
			var query = _entities.AsQueryable();

			if (options.HasWhere)
				query = query.Where(options.Where);

			if (options.HasOrderBy)
				query = query.OrderBy(options.OrderBy);

			foreach (var include in options.GetIncludes())
				query = query.Include(include);

			var entityType = _context.Model.FindEntityType(typeof(T));
			var key = entityType?.FindPrimaryKey()?.Properties.FirstOrDefault();
			if (key == null)
			{
				throw new InvalidOperationException($"Primary key not found for entity type {typeof(T).Name}");
			}
			string primaryKeyName = key.Name;

			return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, primaryKeyName) == id);
		}

		public async Task UpdateAsync(T entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}
			else
			{
				_context.Update(entity);
				await _context.SaveChangesAsync();
			}
		}
	}
}
