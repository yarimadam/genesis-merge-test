using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CoreData.Infrastructure
{
    //TODO Rename and move methods below
    public static class TestHelpers
    {
        public static TRepository CreateRepository<TRepository>(DbContext context = null) where TRepository : IGenericRepository
        {
            if (context == null)
                return Activator.CreateInstance<TRepository>();

            var constructors = typeof(TRepository).GetConstructors();
            var contextType = context.GetType();

            if (constructors
                .Any(x => x.GetParameters()
                    .Any(p => p.ParameterType.IsAssignableFrom(contextType))))
                return (TRepository) Activator.CreateInstance(typeof(TRepository), context);

            if (constructors
                .Any(x => x.GetParameters()
                    .All(p => p.IsOptional)))
            {
                Log.Warning($"Constructor overloads of '{typeof(TRepository)}' does not accept '{contextType.Name}' as parameter, this will result as another instance of context.");
                return Activator.CreateInstance<TRepository>();
            }

            throw new ArgumentException($"Constructor overloads of '{typeof(TRepository)}' does not accept '{contextType.Name}' as parameter.");
        }

        public static void DetachAllEntities(this DbContext context)
        {
            foreach (var entry in context.ChangeTracker.Entries().Where(x => x.State != EntityState.Unchanged))
                entry.State = EntityState.Detached;
        }
    }
}