using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace ZuneSocialTagger.GUI
{
    public abstract class NotifyPropertyChangedImpl : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public virtual event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <summary>
        /// Notifies subscribers of the property change.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public virtual void NotifyOfPropertyChange(string propertyName)
        {
            RaisePropertyChangedEventImmediately(propertyName);
        }

        /// <summary>
        /// Notifies subscribers of the property change.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        public void NotifyOfPropertyChange<TProperty>(Expression<Func<TProperty>> propertyExpression)
        {
            NotifyOfPropertyChange(GetMemberInfo(propertyExpression).Name);
        }

        /// <summary>
        /// Raises the property changed event immediately.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public virtual void RaisePropertyChangedEventImmediately(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Gets the member info represented by an expression.
        /// </summary>
        /// <param name="expression">The member expression.</param>
        /// <returns>The member info represeted by the expression.</returns>
        public static MemberInfo GetMemberInfo(Expression expression)
        {
            var lambda = (LambdaExpression)expression;

            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else memberExpression = (MemberExpression)lambda.Body;

            return memberExpression.Member;
        }
    }
}