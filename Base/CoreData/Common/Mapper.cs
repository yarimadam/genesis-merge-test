using System;
using System.Linq;
using Mapster;

namespace CoreData.Common
{
    public static class Mapper
    {
        public static TDestination Map<TDestination>(this object source) => source.Adapt<TDestination>();
        public static TDestination Map<TDestination>(this object source, TypeAdapterConfig config) => source.Adapt<TDestination>(config);

        public static TDestination Map<TSource, TDestination>(this TSource source) => source.Adapt<TSource, TDestination>();
        public static TDestination Map<TSource, TDestination>(this TSource source, TypeAdapterConfig config) => source.Adapt<TSource, TDestination>(config);

        public static object Map(this object source, Type sourceType, Type destinationType) => source.Adapt(sourceType, destinationType);
        public static object Map(this object source, Type sourceType, Type destinationType, TypeAdapterConfig config) => source.Adapt(sourceType, destinationType, config);

        public static IQueryable<TDestination> ProjectTo<TDestination>(this IQueryable source, TypeAdapterConfig config = null) => source.ProjectToType<TDestination>(config);

        public static IQueryable ProjectTo(this IQueryable source, Type destinationType, TypeAdapterConfig config = null) => source.ProjectToType(destinationType, config);
    }
}