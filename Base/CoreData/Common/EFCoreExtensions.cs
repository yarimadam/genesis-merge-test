using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using CoreData.Infrastructure;
using CoreData.Repositories;
using CoreType.Attributes;
using CoreType.DBModels;
using CoreType.Types;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Serilog;

namespace CoreData.Common
{
    public static class EFCoreExtensions
    {
        public static readonly Func<string, ValueConverter<string, string>> EncryptionValueConverter =
            symmetricKey => new ValueConverter<string, string>(
                value => EncryptionManager.Encrypt(value, symmetricKey, true),
                value => EncryptionManager.Decrypt(value, symmetricKey, false));

        public static readonly ValueConverter<string, string> HashValueConverter = new ValueConverter<string, string>(
            value => Helper.GetHashedString(value, true),
            value => value);

        private static MethodInfo LikeMethodInfo { get; } = typeof(DbFunctionsExtensions)
            .GetMethod(nameof(DbFunctionsExtensions.Like), new[] { typeof(DbFunctions), typeof(string), typeof(string) });

        private static MethodInfo ILikeMethodInfo { get; } = typeof(NpgsqlDbFunctionsExtensions)
            .GetMethod(nameof(NpgsqlDbFunctionsExtensions.ILike), new[] { typeof(DbFunctions), typeof(string), typeof(string) });

        private static readonly PropertyInfo EFPropertyInfo = typeof(EF).GetProperty(nameof(EF.Functions));

        public static IEnumerable<PropertyEntry> GetKeyPropertyEntries(this EntityEntry entry)
        {
            return entry.Metadata.FindPrimaryKey()
                .Properties
                .Select(p => entry.Property(p.Name));
        }

        public static object GetPrimaryIdVal(this EntityEntry entry) => GetPrimaryIdVal<object>(entry);

        public static TKey GetPrimaryIdVal<TKey>(this EntityEntry entry)
        {
            var property = GetKeyPropertyEntries(entry).FirstOrDefault();

            if (property != null)
            {
                var clrType = property.Metadata.ClrType;
                return (TKey) Convert.ChangeType(property.CurrentValue, Nullable.GetUnderlyingType(clrType) ?? clrType);
            }

            return default;
        }

        public static void SetPrimaryIdVal(this EntityEntry entry, object value)
        {
            var property = GetKeyPropertyEntries(entry).FirstOrDefault();

            if (property != null)
            {
                var clrType = property.Metadata.ClrType;
                property.CurrentValue = Convert.ChangeType(value, Nullable.GetUnderlyingType(clrType) ?? clrType);
            }
        }

        public static void ClearPrimaryIds(this EntityEntry entry)
        {
            entry.State = EntityState.Detached;

            var properties = GetKeyPropertyEntries(entry).ToList();

            foreach (var property in properties)
            {
                var clrType = property.Metadata.ClrType;
                var defaultValue = clrType.IsValueType
                    ? Activator.CreateInstance(clrType)
                    : null;

                property.CurrentValue = Convert.ChangeType(defaultValue, clrType);
            }
        }

        public static void SetPropertyVal(this EntityEntry entry, string propertyName, object value)
        {
            var property = entry.Property(propertyName);

            if (property != null)
            {
                var clrType = property.Metadata.ClrType;
                property.CurrentValue = Convert.ChangeType(value, Nullable.GetUnderlyingType(clrType) ?? clrType);
            }
        }

        public static IQueryable<T> AddFiltersAndPagination<T, R>(this IQueryable<T> source, RequestWithPagination<R> entity, bool ignorePagination = false)
            where T : class, new()
            where R : T, new()
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (entity.Criteria == null)
                throw new ArgumentNullException(nameof(entity.Criteria));
            if (entity.Pagination == null)
                throw new ArgumentNullException(nameof(entity.Pagination));

            source = source.AddFilters(entity).AddSortings(entity);

            if (!ignorePagination)
                return AddPagination(source, entity);

            return source;
        }

        public static IQueryable<T> AddFilters<T, R>(this IQueryable<T> source, R entity)
            where T : class, new()
            where R : T, new()
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return AddFilters(source, new RequestWithPagination<R>
            {
                Criteria = entity
            });
        }

        public static IQueryable<T> AddFilters<T, R>(this IQueryable<T> source, RequestWithPagination<R> entity)
            where T : class, new()
            where R : T, new()
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (entity.Criteria == null)
                throw new ArgumentNullException(nameof(entity.Criteria));

            try
            {
                DatabaseType? dbType = null;
                var properties = typeof(T).GetProperties();
                var parameterExpression = Expression.Parameter(typeof(T), "p");

                foreach (var property in properties)
                    try
                    {
                        // If NotMapped Attribute is not defined for the property, continue processing
                        if (!Attribute.IsDefined(property, typeof(NotMappedAttribute)))
                        {
                            MemberExpression propertyExpression = null;
                            var value = property.GetValue(entity.Criteria);

                            if (value != null)
                            {
                                var type = property.PropertyType;
                                var defaultValue = type.IsValueType ? Activator.CreateInstance(type) : null;

                                if (!value.Equals(defaultValue))
                                {
                                    var isString = type == typeof(string);
                                    // Ignore empty strings
                                    if ((isString && string.IsNullOrEmpty(value.ToString()))
                                        //Ignore IEnumerable properties
                                        || (!isString && type.GetInterface(nameof(IEnumerable)) != null))
                                        continue;

                                    propertyExpression ??= Expression.Property(parameterExpression, property.Name);
                                    source = source.AddFilter(parameterExpression, propertyExpression, value, ref dbType);
                                }
                            }

                            var filterModel = entity.GridCriterias?.FilterModel?.FirstOrDefault(x => x.PropertyName.Equals(property.Name, StringComparison.OrdinalIgnoreCase));
                            if (filterModel != null)
                            {
                                propertyExpression ??= Expression.Property(parameterExpression, property.Name);

                                source = source.AddFilterByConditions(parameterExpression, propertyExpression, filterModel, ref dbType);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "AddFilters-Loop");
                    }

                return source;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "AddFilters");
                return source;
            }
        }

        private static IQueryable<T> AddFilterByConditions<T>(this IQueryable<T> source, ParameterExpression parameterExpression, MemberExpression propertyExpression, FilterModel filterModel,
            ref DatabaseType? dbType)
            where T : class, new()
        {
            if (filterModel?.Conditions == null || !filterModel.Conditions.Any())
                return source;

            var context = GetContextFromQuery(source);
            dbType ??= context.GetDatabaseType();

            Expression finalExpression = null;
            foreach (var condition in filterModel.Conditions)
            {
                var currentExpression = GetFilterExpression(condition.Type, null, propertyExpression, ref dbType, condition.Values);

                if (finalExpression != null)
                    finalExpression = filterModel.Operator == "AND"
                        ? Expression.And(finalExpression, currentExpression)
                        : Expression.Or(finalExpression, currentExpression);
                else
                    finalExpression = currentExpression;
            }

            if (finalExpression != null)
            {
                var lambdaExpression = Expression.Lambda<Func<T, bool>>(finalExpression, parameterExpression);

                source = source.Where(lambdaExpression);
            }

            return source;
        }

        private static IQueryable<T> AddFilter<T>(this IQueryable<T> source, ParameterExpression parameterExpression, MemberExpression propertyExpression, object value, ref DatabaseType? dbType)
            where T : class, new()
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (propertyExpression == null)
                throw new ArgumentNullException(nameof(propertyExpression));

            try
            {
                var type = propertyExpression.Type;
                var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
                var context = GetContextFromQuery(source);
                var entityType = context.Model.FindEntityType(typeof(T));
                var property = entityType.FindProperty(propertyExpression.Member);

                dbType ??= context.GetDatabaseType();

                Expression expression;
                if (underlyingType == typeof(string))
                {
                    var valueConverter = property.GetValueConverter();

                    expression = GetFilterExpression(valueConverter != null ? ConditionType.Equals : ConditionType.Contains, property, propertyExpression, ref dbType, value);
                }
                else if (underlyingType == typeof(DateTime) || underlyingType == typeof(DateTimeOffset))
                {
                    var dateValue = ((DateTime) value).Date;

                    expression = GetFilterExpression(ConditionType.Equals, property, propertyExpression, ref dbType, dateValue);
                    //source = source.Where(p => EF.Property<DateTime>(p, propertyExpression.Name) >= minDate && EF.Property<DateTime>(p, propertyExpression.Name) < maxDate);
                }
                else if (underlyingType.IsNumericType())
                {
                    expression = GetFilterExpression(ConditionType.Equals, property, propertyExpression, ref dbType, value);
                    //source = source.Where(p => EF.Property<object>(p, propertyExpression.Name) == values);
                }
                else
                {
                    expression = GetFilterExpression(ConditionType.Equals, property, propertyExpression, ref dbType, value.ToString());
                    // source = source.Where(p =>
                    //     EF.Functions.Like(EF.Property<object>(p, propertyExpression.Name).ToString(),
                    //         values.ToString()));
                }

                return source.Where(Expression.Lambda<Func<T, bool>>(expression, parameterExpression));
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "AddFilters");
                return source;
            }
        }

        private static Expression<Func<TEntity, bool>> GetFilterExpression<TEntity>(ConditionType conditionType, IProperty property, ref DatabaseType? dbType, params object[] values)
        {
            var parameterExpression = Expression.Parameter(typeof(TEntity), "p");
            var propertyExpression = Expression.Property(parameterExpression, property.PropertyInfo);

            return Expression.Lambda<Func<TEntity, bool>>(GetFilterExpression(conditionType, property, propertyExpression, ref dbType, values), parameterExpression);
        }

        private static Expression GetFilterExpression(ConditionType conditionType, IProperty property, MemberExpression propertyExpression, ref DatabaseType? dbType, params object[] values)
        {
            var type = propertyExpression.Type;
            var underlyingType = Nullable.GetUnderlyingType(type);

            if (underlyingType != null)
            {
                type = underlyingType;
                propertyExpression = Expression.Property(propertyExpression, "Value");
            }

            if (type == typeof(DateTime))
                propertyExpression = Expression.Property(propertyExpression, "Date");

            var convertedValues = Enumerable
                .Range(0, values.Length)
                .Where(i => i < values.Length && values[i] != null)
                .Select(i => Convert.ChangeType(values[i], type))
                .ToArray();

            var valueExpressions = Enumerable
                .Range(0, convertedValues.Length)
                .Select(i =>
                {
                    var value = convertedValues[i];
                    if (conditionType == ConditionType.Contains || conditionType == ConditionType.NotContains)
                        value = $"%{value}%";
                    else if (conditionType == ConditionType.StartsWith)
                        value = $"{value}%";
                    else if (conditionType == ConditionType.EndsWith)
                        value = $"%{value}";
                    return Expression.Constant(value);
                })
                .ToArray();

            switch (conditionType)
            {
                case ConditionType.Equals:
                    var containsMethodInfo = type.GetMethod("Equals", new[] { type });

                    if (type == typeof(double) || type == typeof(float) || type == typeof(decimal))
                    {
                        // var precision = property.GetPrecision();
                        var scale = property?.GetScale();
                        var decimalPrecision = scale ?? 2;
                        var roundMethodInfo = typeof(Math).GetMethod("Round", new[] { type, typeof(int) });

                        return Expression.Call(
                            propertyExpression,
                            containsMethodInfo,
                            Expression.Call(
                                null,
                                roundMethodInfo,
                                valueExpressions[0],
                                Expression.Constant(decimalPrecision)
                            )
                        );
                    }

                    return Expression.Call(propertyExpression, containsMethodInfo, valueExpressions[0]);
                //return Expression.Equal(propertyExpression, valueExpression);
                case ConditionType.NotEquals:
                    return Expression.NotEqual(propertyExpression, valueExpressions[0]);
                case ConditionType.StartsWith:
                case ConditionType.EndsWith:
                case ConditionType.NotContains:
                case ConditionType.Contains:
                    MethodCallExpression expression;
                    //'ILIKE' doesn't work on MSSQL and MySQL
                    expression = Expression.Call(dbType == DatabaseType.PostgreSQL ? ILikeMethodInfo : LikeMethodInfo
                        , Expression.Property(null, EFPropertyInfo)
                        , propertyExpression, valueExpressions[0]);
                    return conditionType == ConditionType.NotContains
                        ? (Expression) Expression.Not(expression)
                        : expression;
                case ConditionType.LessThan:
                    return Expression.LessThan(propertyExpression, valueExpressions[0]);
                case ConditionType.LessThanOrEqual:
                    return Expression.LessThanOrEqual(propertyExpression, valueExpressions[0]);
                case ConditionType.GreaterThan:
                    return Expression.GreaterThan(propertyExpression, valueExpressions[0]);
                case ConditionType.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(propertyExpression, valueExpressions[0]);
                case ConditionType.InRange:
                    return Expression.And(
                        Expression.GreaterThanOrEqual(propertyExpression, valueExpressions[0])
                        , Expression.LessThanOrEqual(propertyExpression, valueExpressions[1]));
                default:
                    return Expression.Equal(propertyExpression, valueExpressions[0]);
            }
        }

        private static IEnumerable<INavigation> GetNavigations<T>(this IQueryable<T> source) where T : class, new()
        {
            try
            {
                var entityType = GetContextFromQuery(source).Model
                    .FindEntityType(typeof(T));

                if (entityType != null)
                    return entityType.GetDerivedTypesInclusive()
                        .SelectMany(type => type.GetNavigations());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetNavigations");
            }

            return Enumerable.Empty<INavigation>();
        }

        public static IOrderedQueryable<T> AddSortings<T, R>(this IQueryable<T> source, RequestWithPagination<R> entity)
            where T : class, new()
            where R : T, new()
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (entity.Pagination == null)
                throw new ArgumentNullException(nameof(entity.Pagination));

            try
            {
                var sortModels = entity.GridCriterias?.SortModel;
                if (sortModels != null && sortModels.Any())
                {
                    foreach (var sortModel in sortModels)
                        source = source.OrderBy(sortModel.PropertyName, sortModel.Order == "asc");

                    if (source.IsOrderedQueryable())
                        return (IOrderedQueryable<T>) source;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "AddPagination");
            }

            return (IOrderedQueryable<T>) source.OrderBy(null, true);
        }

        public static IQueryable<T> AddPagination<T, R>(this IQueryable<T> source, RequestWithPagination<R> entity)
            where T : class
            where R : new()
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (entity.Pagination == null)
                throw new ArgumentNullException(nameof(entity.Pagination));

            try
            {
                return source.Skip(entity.Pagination.RowOffset).Take(entity.Pagination.MaxRowsPerPage);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "AddPagination");
                return source;
            }
        }

        public static IEnumerable<T> AddPagination<T, R>(this IEnumerable<T> source, RequestWithPagination<R> entity)
            where T : class
            where R : new()
        {
            return source.AsQueryable().AddPagination(entity);
        }

        public static PaginationWrapper<T> ToPaginatedList<T, R>(this IQueryable<T> source, RequestWithPagination<R> entity)
            where T : class
            where R : new()
        {
            return source.ToPaginatedListAsync(entity).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static async Task<PaginationWrapper<T>> ToPaginatedListAsync<T, R>(this IQueryable<T> source, RequestWithPagination<R> entity)
            where T : class
            where R : new()
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (entity.Pagination == null)
                throw new ArgumentNullException(nameof(entity.Pagination));

            var paginationWrapper = new PaginationWrapper<T>();

            try
            {
                var isAsyncEnumerable = source is IAsyncEnumerable<T>;

                int totalRowCount;
                if (isAsyncEnumerable)
                    totalRowCount = await source.CountAsync();
                else
                    totalRowCount = source.Count();

                var paginatedQuery = AddPagination(source, entity);
                if (isAsyncEnumerable)
                    paginationWrapper.List = await paginatedQuery.ToListAsync();
                else
                    paginationWrapper.List = paginatedQuery.ToList();

                paginationWrapper.Pagination = entity.Pagination;
                paginationWrapper.Pagination.ResultRowCount = paginationWrapper.List.Count;
                paginationWrapper.Pagination.TotalRowCount = totalRowCount;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ToPaginatedList");
                throw;
            }

            return paginationWrapper;
        }

        public static PaginationWrapper<T> ToPaginatedList<T, R>(this IEnumerable<T> source, RequestWithPagination<R> entity)
            where T : class
            where R : new()
        {
            return source.AsQueryable().ToPaginatedList(entity);
        }

        // TODO FIX SelectExclusively does not work properly with filtered Include, omits Where expression in Include method.
        public static IQueryable<T> SelectExclusively<T>(this IQueryable<T> source, params Expression<Func<T, object>>[] expressions)
            where T : class, new()
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (expressions == null)
                throw new ArgumentNullException(nameof(expressions));
            try
            {
                var excludedColumns = NameReaderExtensions.GetMemberNames(expressions);

                var navigationMembers = GetNavigations(source)
                    .Select(x => x.Name)
                    .ToList();

                var selectExpression = LinqExtensions.DynamicFields<T>(excludedColumns, navigationMembers);

                return source.Select(selectExpression);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "SelectExclusively");
                return source;
            }
        }

        public static IEnumerable<T> SelectExclusively<T>(this IEnumerable<T> source, params Expression<Func<T, object>>[] expressions)
            where T : class, new()
        {
            return source.AsQueryable().SelectExclusively(expressions);
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName, bool isAscending = true)
        {
            var type = typeof(T);
            var parameter = Expression.Parameter(type, "p");

            Expression expression;
            if (!string.IsNullOrEmpty(propertyName))
            {
                var memberExpression = Expression.Property(parameter, propertyName);
                expression = memberExpression;
                var propertyType = expression.Type;
                var hasNotMappedAttribute = memberExpression.Member.GetCustomAttribute<NotMappedAttribute>(true) != null;

                if (hasNotMappedAttribute
                    || (!propertyType.IsValueType
                        && !propertyType.IsPrimitive
                        && propertyType != typeof(string) && propertyType != typeof(object)))
                    return source;
            }
            else
                expression = Expression.Constant(1);

            var isOrderedQueryable = source.IsOrderedQueryable();
            var command = isAscending
                ? (!isOrderedQueryable ? "OrderBy" : "ThenBy")
                : (!isOrderedQueryable ? "OrderByDescending" : "ThenByDescending");
            var orderByExpression = Expression.Lambda(expression, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new[] { type, expression.Type },
                source.Expression, Expression.Quote(orderByExpression));

            return (IOrderedQueryable<T>) source.Provider.CreateQuery<T>(resultExpression);
        }

        public static bool IsOrderedQueryable<T>(this IQueryable<T> source) => typeof(IOrderedQueryable<T>).IsAssignableFrom(source.Expression.Type);

        private static DbContext GetContextFromQuery(IQueryable query)
        {
            DbContext context = null;

            try
            {
                var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
                var queryCompiler = typeof(EntityQueryProvider).GetField("_queryCompiler", bindingFlags)?.GetValue(query.Provider);
                var queryContextFactory = queryCompiler?.GetType().GetField("_queryContextFactory", bindingFlags)?.GetValue(queryCompiler);

                var dependencies = typeof(RelationalQueryContextFactory).GetField("_dependencies", bindingFlags)?.GetValue(queryContextFactory);
                var queryContextDependencies = typeof(DbContext).Assembly.GetType(typeof(QueryContextDependencies).FullName ?? throw new ArgumentNullException(nameof(QueryContextDependencies)));
                var stateManagerProperty = queryContextDependencies.GetProperty("StateManager", bindingFlags | BindingFlags.Public)?.GetValue(dependencies);
                var stateManager = (IStateManager) stateManagerProperty;

                context = stateManager?.Context;
            }
            catch (Exception e)
            {
                Log.Fatal(e, "GetContextFromQuery");
            }

            return context;
        }

        public static IEnumerable<TSource> PrioritizeByTenant<TSource, TKey>(this IQueryable<TSource> source, Func<TSource, TKey> groupSelector,
            Func<IOrderedEnumerable<TSource>, IEnumerable<TSource>> subQuery = null)
            where TSource : class, ITenantInfo
        {
            subQuery ??= query => query;

            var context = GetContextFromQuery(source);
            var tenants = context.Set<Tenant>()
                .AsNoTracking()
                .IgnoreQueryFilters()
                .Select(x => new Tenant
                {
                    TenantId = x.TenantId,
                    TenantType = x.TenantType,
                    ParentTenantId = x.ParentTenantId
                })
                .OrderByDescending(x => x.TenantId)
                .Cacheable(CacheExpirationMode.Absolute, TimeSpan.FromMinutes(1))
                .ToList();

            var tenantId = SessionAccessor.GetSession().GetTenantId();

            var parentTenants = tenantId != 0
                ? TenantRepository
                    .RecurseParentTenants(tenants, tenantId)
                    .Select(x => x.TenantId)
                    .ToList()
                : tenants
                    .Select(x => x.TenantId)
                    .ToList();

            parentTenants.Add(0);

            // TODO Fix ordering problem
            // Server-side attempt-1 method based linq (throws error on ordering by tenant index)
            // var serverSide = context.Set<CoreParameters>()
            //     .AsNoTracking()
            //     .IgnoreQueryFilters()
            //     .Where(x => x.TenantId == 0 || parentTenants.Contains(x.TenantId))
            //     .GroupBy(x => new { x.KeyCode, x.ParentValue, x.Value, x.Status })
            //     .Select(x => new { x.Key.KeyCode, x.Key.ParentValue, x.Key.Value, x.Key.Status })
            //     .Select(g => context.Set<CoreParameters>()
            //         .AsNoTracking()
            //         .IgnoreQueryFilters()
            //         .Where(t => t.KeyCode == g.KeyCode && t.Value == g.Value && t.ParentValue == g.ParentValue && t.Status == g.Status)
            //         // Throws error here
            //         .OrderBy(p => parentTenants.IndexOf(p.TenantId))
            //         .FirstOrDefault())
            //     .ToList();

            // TODO Fix ordering problem
            // Server-side attempt-2 query based linq (throws error on ordering by tenant index)
            // var serverSide = from p in context.Set<CoreParameters>().AsNoTracking().IgnoreQueryFilters()
            //     where p.TenantId == 0 || parentTenants.Contains(p.TenantId)
            //     group p by new { p.KeyCode, p.Value, p.ParentValue, p.Status }
            //     into g
            //     select new { g.Key.KeyCode, g.Key.ParentValue, g.Key.Value, g.Key.Status }
            //     into p3
            //     select context.Set<CoreParameters>()
            //         .AsNoTracking()
            //         .IgnoreQueryFilters()
            //         .Where(p2 => p2.KeyCode == p3.KeyCode && p2.Value == p3.Value && p2.ParentValue == p3.ParentValue && p2.Status == p3.Status)
            //         // Throws error here
            //         .OrderBy(p => parentTenants.IndexOf(p.TenantId))
            //         .FirstOrDefault();

            return source
                .IgnoreQueryFilters()
                .Where(x => x.TenantId == 0 || parentTenants.Contains(x.TenantId))
                .AsEnumerable()
                .GroupBy(groupSelector)
                .Select(g =>
                    subQuery(
                            g.OrderBy(p => parentTenants.IndexOf(p.TenantId)
                            ))
                        .FirstOrDefault());
        }

        public static IEnumerable<TSource> PrioritizeByTenant<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> groupSelector,
            Func<IOrderedEnumerable<TSource>, IEnumerable<TSource>> subQuery = null)
            where TSource : class, ITenantInfo
        {
            return source.AsQueryable().PrioritizeByTenant(groupSelector, subQuery);
        }

        public static IEnumerable<CoreParameters> PrioritizeByTenant(this IQueryable<CoreParameters> source,
            Func<IOrderedEnumerable<CoreParameters>, IEnumerable<CoreParameters>> subQuery = null)
        {
            return source.PrioritizeByTenant(p => new { p.KeyCode, p.ParentValue, p.Value, p.Status }, subQuery);
        }

        public static IEnumerable<CoreParameters> PrioritizeByTenant(this IEnumerable<CoreParameters> source,
            Func<IOrderedEnumerable<CoreParameters>, IEnumerable<CoreParameters>> subQuery = null)
        {
            return source.AsQueryable().PrioritizeByTenant(subQuery);
        }

        public static IQueryable<TEntity> GetByIdAsQueryable<TEntity, TKey>(this IQueryable<TEntity> source, TKey primaryId)
            where TEntity : class, new()
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (primaryId == null)
                throw new ArgumentNullException(nameof(primaryId));

            try
            {
                var entity = new TEntity();
                var context = GetContextFromQuery(source);
                var entry = context.Entry(entity);
                DatabaseType? dbType = context.GetDatabaseType();

                SetPrimaryIdVal(entry, primaryId);

                var property = entry.Metadata.FindPrimaryKey().Properties.First();
                var expression = GetFilterExpression<TEntity>(ConditionType.Equals, property, ref dbType, primaryId);

                source = source.Where(expression);

                return source;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetByIdAsQueryable");
                return source;
            }
        }

        public static Expression<Func<T, T>> CreateSoftDeleteExpression<T>(SoftDeleteAttribute softDeleteAttribute)
            where T : class, new()
        {
            if (softDeleteAttribute == null)
                throw new ArgumentNullException(nameof(SoftDeleteAttribute));

            try
            {
                var type = typeof(T);
                var propertyInfo = type.GetProperty(softDeleteAttribute.PropertyName);
                if (propertyInfo == null)
                    throw new ArgumentNullException(nameof(propertyInfo));

                var newExpression = Expression.New(type);
                var constantExpression = Expression.Constant(softDeleteAttribute.ValueToBeAssigned, propertyInfo.PropertyType);
                var parameterExpression = Expression.Parameter(type, "e");
                var memberAssignment = Expression.Bind(propertyInfo, constantExpression);
                var memberInit = Expression.MemberInit(newExpression, memberAssignment);

                return Expression.Lambda<Func<T, T>>(memberInit, parameterExpression);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "CreateSoftDeleteExpression");
                throw;
            }
        }

        public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> propertyBuilder, JsonSerializerSettings jsonSerializerSettings = null)
            where T : class, new()
        {
            jsonSerializerSettings ??= CustomJsonSerializerSettings.Default;

            var converter = new ValueConverter<T, string>(
                v => JsonConvert.SerializeObject(v, jsonSerializerSettings),
                v => JsonConvert.DeserializeObject<T>(v, jsonSerializerSettings));

            var comparer = new ValueComparer<T>(
                (l, r) => JsonConvert.SerializeObject(l) == JsonConvert.SerializeObject(r),
                v => v == null ? 0 : JsonConvert.SerializeObject(v).GetHashCode(),
                v => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(v, jsonSerializerSettings), jsonSerializerSettings));

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.Metadata.SetValueComparer(comparer);

            return propertyBuilder;
        }

        public static ValueConverter ToValueConverter(this PersistenceConverterAttribute converterAttribute)
        {
            return converterAttribute.PersistenceOptions switch
            {
                PersistenceOptions.Encrypt => EncryptionValueConverter(
                    converterAttribute is EncryptedPersistenceAttribute encryptedPersistenceAttribute
                        ? encryptedPersistenceAttribute.SymmetricKey
                        : null),
                PersistenceOptions.Hash => HashValueConverter,
                _ => new ValueConverter<string, string>(converterAttribute.ConvertToProviderExpression, converterAttribute.ConvertFromProviderExpression)
            };
        }

        public static void ApplyConfiguration<TEntity>(this ModelBuilder modelBuilder, Action<EntityTypeBuilder<TEntity>> action)
            where TEntity : class
        {
            var configuration = (IEntityTypeConfiguration<TEntity>) Activator.CreateInstance(typeof(EntityTypeConfiguration<TEntity>), action);

            modelBuilder.ApplyConfiguration(configuration);
        }
    }
}