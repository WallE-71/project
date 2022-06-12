using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ShoppingStore.Infrastructure.Contracts
{
    public interface IBrandDbInitializer
    {
        /// <summary>
        /// Applies any pending migrations for the context to the database.
        /// Will create the database if it does not already exist.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Adds some default values to the IdentityDb
        /// </summary>
        void SeedData();

        Task<IdentityResult> SeedDatabaseAsync();
    }
}
