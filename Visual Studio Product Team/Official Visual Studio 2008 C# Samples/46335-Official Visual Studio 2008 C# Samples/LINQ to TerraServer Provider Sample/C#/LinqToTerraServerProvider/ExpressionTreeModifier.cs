using System;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToTerraServerProvider
{
    internal class ExpressionTreeModifier : ExpressionVisitor
    {
        private IQueryable<Place> queryablePlaces;

        internal ExpressionTreeModifier(IQueryable<Place> places)
        {
            this.queryablePlaces = places;
        }

        internal Expression CopyAndModify(Expression expression)
        {
            return this.Visit(expression);
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            // Replace the constant QueryableTerraServerData arg with the queryable Place collection.
            if (c.Type == typeof(QueryableTerraServerData<Place>))
                return Expression.Constant(this.queryablePlaces);
            else
                return c;
        }
    }
}
