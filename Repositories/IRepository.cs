
using System.Collections.Generic;
using System.Threading.Tasks;
using WeaponGameRestApi.Core;
namespace WeaponGameRestApi.Repositories
{
    public interface IRepository<T> where T : class, IEntity
    {
        Task<T?> GetAsync(string id);
        Task<IEnumerable<T>> ListAsync();
        Task AddAsync(T item);
        Task UpdateAsync(T item);
        Task DeleteAsync(string id);
    }
    public class InMemoryRepository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly Dictionary<string, T> _items = new Dictionary<string, T>();
        public Task<T?> GetAsync(string id)
        {
            _items.TryGetValue(id, out var item);
            return Task.FromResult(item);
        }
        public Task<IEnumerable<T>> ListAsync()
        {
            return Task.FromResult<IEnumerable<T>>(_items.Values.ToList());
        }
        public Task AddAsync(T item)
        {
            if (string.IsNullOrWhiteSpace(item.Id))
            {
                throw new InvalidOperationException("Item must have an Id property.");
            }
            
            if (!_items.TryAdd(item.Id, item))
            {
                throw new InvalidOperationException($"Item with the {item.Id} Id already exists.");
            }
            _items[item.Id] = item;
            return Task.CompletedTask;
        }
        public Task UpdateAsync(T item)
        {
            if (string.IsNullOrWhiteSpace(item.Id))
            {
                throw new InvalidOperationException("Id cannot be null or empty when updating.");
            }
            
            if (item.Id == null || !_items.ContainsKey(item.Id))
            {
                throw new KeyNotFoundException($"Item with the {item.Id} Id does not exist.");
            }
            _items[item.Id] = item;
            return Task.CompletedTask;
        }
        public Task DeleteAsync(string id)
        {
            _items.Remove(id, out _);
            return Task.CompletedTask;
        }
    }
}
