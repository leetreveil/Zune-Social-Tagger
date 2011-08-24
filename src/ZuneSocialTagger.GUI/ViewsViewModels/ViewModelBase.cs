using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Threading;

namespace ZuneSocialTagger.GUI.ViewsViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        protected PropertyChangedEventHandler _propertyChanged;
        public virtual event PropertyChangedEventHandler PropertyChanged
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add
            {
                _propertyChanged = (PropertyChangedEventHandler)Delegate.Combine(_propertyChanged, value);
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove
            {
                _propertyChanged = (PropertyChangedEventHandler)Delegate.Remove(_propertyChanged, value);
            }
        }

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = _propertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RaisePropertyChanged<TProperty>(Expression<Func<TProperty>> propertyExpression)
        {
            RaisePropertyChanged(GetMemberInfoName(propertyExpression));
        }

        private static string GetMemberInfoName(Expression expression)
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

        /// <summary>
        /// Calls the method on the UI thread
        /// </summary>
        /// <param name="action"></param>
        public void Dispatch(Action action) {
            DispatcherHelper.CheckBeginInvokeOnUI(action);
        }
    }
}