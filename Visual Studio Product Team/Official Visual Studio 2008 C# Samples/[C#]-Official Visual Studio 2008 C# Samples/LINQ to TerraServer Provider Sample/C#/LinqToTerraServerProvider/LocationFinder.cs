using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToTerraServerProvider
{
    internal class LocationFinder : ExpressionVisitor
    {
        private Expression expression;
        private List<string> locations;

        public LocationFinder(Expression exp)
        {
            this.expression = exp;
        }

        public List<string> Locations
        {
            get
            {
                if (locations == null)
                {
                    locations = new List<string>();
                    this.Visit(this.expression);
                }
                return this.locations;
            }
        }

        protected override Expression VisitBinary(BinaryExpression be)
        {
            if (be.NodeType == ExpressionType.Equal)
            {
                if (ExpressionTreeHelpers.IsMemberEqualsValueExpression(be, typeof(Place), "Name"))
                {
                    locations.Add(ExpressionTreeHelpers.GetValueFromEqualsExpression(be, typeof(Place), "Name"));
                    return be;
                }
                else if (ExpressionTreeHelpers.IsMemberEqualsValueExpression(be, typeof(Place), "State"))
                {
                    locations.Add(ExpressionTreeHelpers.GetValueFromEqualsExpression(be, typeof(Place), "State"));
                    return be;
                }
                else
                    return base.VisitBinary(be);
            }
            else
                return base.VisitBinary(be);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(String) && m.Method.Name == "StartsWith")
            {
                if (ExpressionTreeHelpers.IsSpecificMemberExpression(m.Object, typeof(Place), "Name") ||
                ExpressionTreeHelpers.IsSpecificMemberExpression(m.Object, typeof(Place), "State"))
                {
                    locations.Add(ExpressionTreeHelpers.GetValueFromExpression(m.Arguments[0]));
                    return m;
                }

            }
            else if (m.Method.Name == "Contains")
            {
                Expression valuesExpression = null;

                if (m.Method.DeclaringType == typeof(Enumerable))
                {
                    if (ExpressionTreeHelpers.IsSpecificMemberExpression(m.Arguments[1], typeof(Place), "Name") ||
                    ExpressionTreeHelpers.IsSpecificMemberExpression(m.Arguments[1], typeof(Place), "State"))
                    {
                        valuesExpression = m.Arguments[0];
                    }
                }
                else if (m.Method.DeclaringType == typeof(List<string>))
                {
                    if (ExpressionTreeHelpers.IsSpecificMemberExpression(m.Arguments[0], typeof(Place), "Name") ||
                    ExpressionTreeHelpers.IsSpecificMemberExpression(m.Arguments[0], typeof(Place), "State"))
                    {
                        valuesExpression = m.Object;
                    }
                }

                if (valuesExpression == null || valuesExpression.NodeType != ExpressionType.Constant)
                    throw new InvalidQueryException("Could not find the location values.");

                ConstantExpression ce = (ConstantExpression)valuesExpression;

                IEnumerable<string> placeStrings = (IEnumerable<string>)ce.Value;
                // Add each string in the collection to the list of locations to obtain data about.
                foreach (string place in placeStrings)
                    locations.Add(place);

                return m;
            }

            return base.VisitMethodCall(m);
        }
    }
}