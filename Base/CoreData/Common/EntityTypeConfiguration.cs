using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreData.Common
{
    public class EntityTypeConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : class
    {
        private readonly Action<EntityTypeBuilder<TEntity>> _entityTypeBuilderFunc;

        public EntityTypeConfiguration(Action<EntityTypeBuilder<TEntity>> entityTypeBuilderFunc)
        {
            _entityTypeBuilderFunc = entityTypeBuilderFunc;
        }

        public void Configure(EntityTypeBuilder<TEntity> builder)
        {
            _entityTypeBuilderFunc(builder);
        }
    }
}