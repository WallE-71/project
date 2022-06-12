using System;
using Microsoft.Extensions.Options;

namespace ShoppingStore.Infrastructure.Contracts
{
    public interface IWritableOptions<out T> : IOptions<T> where T : class, new()
    {
        T Value { get; }
        void Update(Action<T> applyChanges);
    }
}
