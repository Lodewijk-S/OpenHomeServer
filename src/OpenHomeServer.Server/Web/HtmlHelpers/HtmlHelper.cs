using System.ComponentModel;
using Castle.Core.Internal;
using Nancy.ViewEngines.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeServer.Server.Web.HtmlHelpers
{
    /// <summary>
    /// Contains extension methods for the <see cref="Expression"/> type.
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Retrieves the member that an expression is defined for.
        /// </summary>
        /// <param name="expression">The expression to retreive the member from.</param>
        /// <returns>A <see cref="MemberInfo"/> instance if the member could be found; otherwise <see langword="null"/>.</returns>
        public static MemberInfo GetTargetMemberInfo(this Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Convert:
                    return GetTargetMemberInfo(((UnaryExpression)expression).Operand);
                case ExpressionType.Lambda:
                    return GetTargetMemberInfo(((LambdaExpression)expression).Body);
                case ExpressionType.Call:
                    return ((MethodCallExpression)expression).Method;
                case ExpressionType.MemberAccess:
                    return ((MemberExpression)expression).Member;
                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// Contains the extension methods on HtmlHelper<T>
    /// </summary>
    public static class HtmlHelperExtensions
    {
        public static IHtmlString TextBoxFor<T>(this HtmlHelpers<T> helpers, Expression<Func<T, object>> expression)
        {
            var name = expression.GetTargetMemberInfo().Name;
            var value = expression.Compile().Invoke(helpers.Model);

            var markup = string.Concat("<input type='textbox' class='form-control' name='", name, "' id='", name, "' value='", value,"' />");

            return new NonEncodedHtmlString(markup);
        }

        public static IHtmlString LabelFor<T>(this HtmlHelpers<T> helpers, Expression<Func<T, object>> expression, string labelText = null)
        {
            var property = expression.GetTargetMemberInfo();
            var name = property.Name;
            var displayNameAttribute = property.GetAttribute<DisplayNameAttribute>();
            var text = displayNameAttribute == null ? labelText : displayNameAttribute.DisplayName;

            var markup = string.Concat("<label for='", name,"' class='col-sm-2 control-label'>", text ?? name,"</label>");

            return new NonEncodedHtmlString(markup);
        }

        public static IHtmlString FormGroupFor<T>(this HtmlHelpers<T> helpers, Expression<Func<T, object>> expression)
        {
            var label = helpers.LabelFor(expression);
            var textBox = helpers.TextBoxFor(expression);

            var markup = string.Concat(@"<div class='form-group'>", label.ToHtmlString(), "<div class='col-sm-10'>", textBox.ToHtmlString(), "</div></div>");

            return new NonEncodedHtmlString(markup);
        }

        public static IHtmlString SubmitButton<T>(this HtmlHelpers<T> helpers)
        {
            var markup = "<div class='form-group'><div class='col-sm-offset-2 col-sm-10'><button type='submit' class='btn btn-defaul'>Save Changes</button></div></div>";
            return new NonEncodedHtmlString(markup);
        }

        //public static IHtmlString FormFor<T>(this HtmlHelpers<T> helpers, Expression<Func<T, object>> expression)
        //{
        //    var type = typeof (T);
        //    foreach (var property in type.GetProperties())
        //    {
                
        //    }
        //}
    }
}
