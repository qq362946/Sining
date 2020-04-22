using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Sining.Tools
{
    public static class LambdaHelper
    {
        public static Expression ReplaceParameters(Scene scene, Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            using (ParameterRebinderEntity entity = ComponentFactory.Create<ParameterRebinderEntity>(scene))
            {
                entity.ParameterRebinder.Map = map;

                return entity.ParameterRebinder.Visit(exp);
            }
        }

        public static Expression<T> Compose<T>(this Expression<T> first, Scene scene, Expression<T> second,
            Func<Expression, Expression, Expression> merge)
        {
            using var dic =
                ObjectPool<DictionaryComponent<ParameterExpression, ParameterExpression>>.Rent();

            for (var i = 0; i < first.Parameters.Count; i++)
            {
                dic.Dictionary[second.Parameters[i]] = first.Parameters[i];
            }

            var secondBody = ReplaceParameters(scene, dic.Dictionary, second.Body);

            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Scene scene, Expression<Func<T, bool>> second)
        {
            return first.Compose(scene, second, Expression.And);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Scene scene, Expression<Func<T, bool>> second)
        {
            return first.Compose(scene, second, Expression.Or);
        }

        public static Expression<Func<T, bool>> True<T>()
        {
            return f => true;
        }

        public static Expression<Func<T, bool>> False<T>()
        {
            return f => false;
        }
    }
}