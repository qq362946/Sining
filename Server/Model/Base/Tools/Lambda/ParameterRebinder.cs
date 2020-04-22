using System.Collections.Generic;
using System.Linq.Expressions;
using Sining.Event;

namespace Sining.Tools
{
    [ComponentSystem]
    public class ParameterRebinderEntityDestroySystem: DestroySystem<ParameterRebinderEntity>
    {
        protected override void Destroy(ParameterRebinderEntity self)
        {
            self.ParameterRebinder.Map = null;
        }
    }

    public class ParameterRebinderEntity: Component
    {
        public readonly ParameterRebinder ParameterRebinder = new ParameterRebinder();
    }

    public class ParameterRebinder: ExpressionVisitor
    {
        public Dictionary<ParameterExpression, ParameterExpression> Map;

        protected override Expression VisitParameter(ParameterExpression p)
        {
            if (Map.TryGetValue(p, out var replacement))
            {
                p = replacement;
            }

            return base.VisitParameter(p);
        }
    }
}