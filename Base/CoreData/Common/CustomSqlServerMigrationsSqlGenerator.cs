using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace CoreData.Common
{
    internal class CustomSqlServerMigrationsSqlGenerator : SqlServerMigrationsSqlGenerator
    {
        public CustomSqlServerMigrationsSqlGenerator(
            MigrationsSqlGeneratorDependencies dependencies,
            IRelationalAnnotationProvider migrationsAnnotations)
            : base(dependencies, migrationsAnnotations)
        {
        }

        protected override void Generate(
            SqlServerCreateDatabaseOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            base.Generate(operation, model, builder);

            builder
                .Append("ALTER DATABASE ")
                .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
                .Append(" COLLATE ")
                .Append(Constants.MSSQL_DATABASE_COLLATION_NAME)
                .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator)
                .EndCommand(true);
        }
    }
}