using System;
using System.Linq.Expressions;
using GalaSoft.MvvmLight;

namespace ZuneSocialTagger.GUI
{
    public class ViewModelBaseExtended : ViewModelBase
    {
        public void RaisePropertyChanged<TProperty>(Expression<Func<TProperty>> propertyExpression)
        {
            RaisePropertyChanged(GetMemberInfoName(propertyExpression));
        }

        /// <summary>
        /// Gets the member info represented by an expression.
        /// </summary>
        /// <param name="expression">The member expression.</param>
        /// <returns>The member name</returns>
        public static string GetMemberInfoName(Expression expression)
        {
            var lambda = (LambdaExpression)expression;

            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else memberExpression = (MemberExpression)lambda.Body;

            return memberExpression.Member.Name;
        }
    }
}