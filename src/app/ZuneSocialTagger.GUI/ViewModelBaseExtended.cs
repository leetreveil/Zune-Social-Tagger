using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace ZuneSocialTagger.GUI
{
    [Serializable]
    public class ViewModelBaseExtended : INotifyPropertyChanged
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

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}