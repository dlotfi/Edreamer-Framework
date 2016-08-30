using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Edreamer.Framework.Helpers
{
    public static class ExpressionExtensions
    {
        // Based on ReflectionHelper.FindProperty method in AutoMapper - http://automapper.org
        public static IEnumerable<MemberInfo> FindProperties(this Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Lambda:
                    foreach (var memberInfo in FindProperties(((LambdaExpression)expression).Body))
                    {
                        yield return memberInfo;
                    }
                    break;
                case ExpressionType.Convert:
                    foreach (var memberInfo in FindProperties(((UnaryExpression)expression).Operand))
                    {
                        yield return memberInfo;
                    }
                    break;
                case ExpressionType.New:
                    var newExpression = ((NewExpression)expression);
                    foreach (var memberInfo in newExpression.Arguments.SelectMany(FindProperties))
                    {
                        yield return memberInfo;
                    }
                    break;
                case ExpressionType.MemberAccess:
                    var memberExpression = ((MemberExpression)expression);
                    Throw.If(memberExpression.Expression.NodeType != ExpressionType.Parameter &&
                             memberExpression.Expression.NodeType != ExpressionType.Convert)
                        .AnArgumentException("Expression '{0}' must resolve to top-level property.".FormatWith(expression), "expression");
                    MemberInfo member = memberExpression.Member;
                    yield return member;
                    break;
                default:
                    Throw.Now.AnArgumentException(
                        "Expression must resolve to a single property or an anonymous type formed of top-level properties.",
                        "expression");
                    break;
            }
        }

        public static bool ExpressionEquals(this Expression source, Expression target)
        {
            Throw.IfArgumentNull(source, "source");
            Throw.IfArgumentNull(target, "target");
            Func<Expression, Expression, bool> equalityComparer = ExpressionEqualityComparer.Instance.Equals;
            return equalityComparer(source, target);
        }
    }

    // The following classes are all based on db4o project - https://source.db4o.com/db4o/trunk/db4o.net/Db4objects.Db4o.Linq/

    internal class ExpressionEqualityComparer : IEqualityComparer<Expression>
    {
        public static ExpressionEqualityComparer Instance = new ExpressionEqualityComparer();

        public bool Equals(Expression a, Expression b)
        {
            return new ExpressionComparison(a, b).AreEqual;
        }

        public int GetHashCode(Expression expression)
        {
            return new HashCodeCalculation(expression).HashCode;
        }
    }

    internal class ExpressionComparison : ExpressionVisitor
    {
        private bool _areEqual = true;

        private Queue<Expression> _candidates;
        private Expression _candidate;

        public bool AreEqual
        {
            get { return _areEqual; }
        }

        public ExpressionComparison(Expression a, Expression b)
        {
            _candidates = new Queue<Expression>(new ExpressionEnumeration(b));

            Visit(a);

            if (_candidates.Count > 0) Stop();
        }

        private Expression PeekCandidate()
        {
            if (_candidates.Count == 0) return null;
            return _candidates.Peek();
        }

        private Expression PopCandidate()
        {
            return _candidates.Dequeue();
        }

        private bool CheckAreOfSameType(Expression candidate, Expression expression)
        {
            if (!CheckEqual(expression.NodeType, candidate.NodeType)) return false;
            if (!CheckEqual(expression.Type, candidate.Type)) return false;

            return true;
        }

        private void Stop()
        {
            _areEqual = false;
        }

        private T CandidateFor<T>(T original) where T : Expression
        {
            return (T)_candidate;
        }

        protected override void Visit(Expression expression)
        {
            if (expression == null) return;
            if (!AreEqual) return;

            _candidate = PeekCandidate();
            if (!CheckNotNull(_candidate)) return;
            if (!CheckAreOfSameType(_candidate, expression)) return;

            PopCandidate();

            base.Visit(expression);
        }

        protected override void VisitConstant(ConstantExpression constant)
        {
            var candidate = CandidateFor(constant);
            if (!CheckEqual(constant.Value, candidate.Value)) return;
        }

        protected override void VisitMemberAccess(MemberExpression member)
        {
            var candidate = CandidateFor(member);
            if (!CheckEqual(member.Member, candidate.Member)) return;

            base.VisitMemberAccess(member);
        }

        protected override void VisitMethodCall(MethodCallExpression methodCall)
        {
            var candidate = CandidateFor(methodCall);
            if (!CheckEqual(methodCall.Method, candidate.Method)) return;

            base.VisitMethodCall(methodCall);
        }

        protected override void VisitParameter(ParameterExpression parameter)
        {
            var candidate = CandidateFor(parameter);
            if (!CheckEqual(parameter.Name, candidate.Name)) return;
        }

        protected override void VisitTypeIs(TypeBinaryExpression type)
        {
            var candidate = CandidateFor(type);
            if (!CheckEqual(type.TypeOperand, candidate.TypeOperand)) return;

            base.VisitTypeIs(type);
        }

        protected override void VisitBinary(BinaryExpression binary)
        {
            var candidate = CandidateFor(binary);
            if (!CheckEqual(binary.Method, candidate.Method)) return;
            if (!CheckEqual(binary.IsLifted, candidate.IsLifted)) return;
            if (!CheckEqual(binary.IsLiftedToNull, candidate.IsLiftedToNull)) return;

            base.VisitBinary(binary);
        }

        protected override void VisitUnary(UnaryExpression unary)
        {
            var candidate = CandidateFor(unary);
            if (!CheckEqual(unary.Method, candidate.Method)) return;
            if (!CheckEqual(unary.IsLifted, candidate.IsLifted)) return;
            if (!CheckEqual(unary.IsLiftedToNull, candidate.IsLiftedToNull)) return;

            base.VisitUnary(unary);
        }

        protected override void VisitNew(NewExpression nex)
        {
            var candidate = CandidateFor(nex);
            if (!CheckEqual(nex.Constructor, candidate.Constructor)) return;
            CompareList(nex.Members, candidate.Members);

            base.VisitNew(nex);
        }

        private void CompareList<T>(ReadOnlyCollection<T> collection, ReadOnlyCollection<T> candidates)
        {
            CompareList(collection, candidates, (item, candidate) => EqualityComparer<T>.Default.Equals(item, candidate));
        }

        private void CompareList<T>(ReadOnlyCollection<T> collection, ReadOnlyCollection<T> candidates, Func<T, T, bool> comparer)
        {
            if (!CheckAreOfSameSize(collection, candidates)) return;

            for (int i = 0; i < collection.Count; i++)
            {
                if (!comparer(collection[i], candidates[i]))
                {
                    Stop();
                    return;
                }
            }
        }

        private bool CheckAreOfSameSize<T>(ReadOnlyCollection<T> collection, ReadOnlyCollection<T> candidate)
        {
            return CheckEqual(collection.Count, candidate.Count);
        }

        private bool CheckNotNull<T>(T t) where T : class
        {
            if (t == null)
            {
                Stop();
                return false;
            }

            return true;
        }

        private bool CheckEqual<T>(T t, T candidate)
        {
            // Note: Added by me as a workaround for the mismatch between CustomTypes and RuntimeTypes
            if (typeof(Type).IsAssignableFrom(t.GetType()))
            {
                var type1 = t as Type;
                var type2 = candidate as Type;
                if (type1 != null && type2 != null &&
                    type1.FullName == type2.FullName &&
                    type1.AssemblyQualifiedName == type2.AssemblyQualifiedName)
                {
                    return true;
                }
                Stop();
                return false;
            }


            if (!EqualityComparer<T>.Default.Equals(t, candidate))
            {
                Stop();
                return false;
            }

            return true;
        }
    }

    internal abstract class ExpressionVisitor
    {
        protected virtual void Visit(Expression expression)
        {
            if (expression == null)
                return;

            switch (expression.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                case ExpressionType.UnaryPlus:
                    VisitUnary((UnaryExpression)expression);
                    break;
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Power:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    VisitBinary((BinaryExpression)expression);
                    break;
                case ExpressionType.TypeIs:
                    VisitTypeIs((TypeBinaryExpression)expression);
                    break;
                case ExpressionType.Conditional:
                    VisitConditional((ConditionalExpression)expression);
                    break;
                case ExpressionType.Constant:
                    VisitConstant((ConstantExpression)expression);
                    break;
                case ExpressionType.Parameter:
                    VisitParameter((ParameterExpression)expression);
                    break;
                case ExpressionType.MemberAccess:
                    VisitMemberAccess((MemberExpression)expression);
                    break;
                case ExpressionType.Call:
                    VisitMethodCall((MethodCallExpression)expression);
                    break;
                case ExpressionType.Lambda:
                    VisitLambda((LambdaExpression)expression);
                    break;
                case ExpressionType.New:
                    VisitNew((NewExpression)expression);
                    break;
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    VisitNewArray((NewArrayExpression)expression);
                    break;
                case ExpressionType.Invoke:
                    VisitInvocation((InvocationExpression)expression);
                    break;
                case ExpressionType.MemberInit:
                    VisitMemberInit((MemberInitExpression)expression);
                    break;
                case ExpressionType.ListInit:
                    VisitListInit((ListInitExpression)expression);
                    break;
                default:
                    throw new ArgumentException("Unhandled expression type: '{0}'".FormatWith(expression.NodeType));
            }
        }

        protected virtual void VisitBinding(MemberBinding binding)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    VisitMemberAssignment((MemberAssignment)binding);
                    break;
                case MemberBindingType.MemberBinding:
                    VisitMemberMemberBinding((MemberMemberBinding)binding);
                    break;
                case MemberBindingType.ListBinding:
                    VisitMemberListBinding((MemberListBinding)binding);
                    break;
                default:
                    throw new ArgumentException("Unhandled binding type '{0}'".FormatWith(binding.BindingType));
            }
        }

        protected virtual void VisitElementInitializer(ElementInit initializer)
        {
            VisitExpressionList(initializer.Arguments);
        }

        protected virtual void VisitUnary(UnaryExpression unary)
        {
            Visit(unary.Operand);
        }

        protected virtual void VisitBinary(BinaryExpression binary)
        {
            Visit(binary.Left);
            Visit(binary.Right);
            Visit(binary.Conversion);
        }

        protected virtual void VisitTypeIs(TypeBinaryExpression type)
        {
            Visit(type.Expression);
        }

        protected virtual void VisitConstant(ConstantExpression constant)
        {
        }

        protected virtual void VisitConditional(ConditionalExpression conditional)
        {
            Visit(conditional.Test);
            Visit(conditional.IfTrue);
            Visit(conditional.IfFalse);
        }

        protected virtual void VisitParameter(ParameterExpression parameter)
        {
        }

        protected virtual void VisitMemberAccess(MemberExpression member)
        {
            Visit(member.Expression);
        }

        protected virtual void VisitMethodCall(MethodCallExpression methodCall)
        {
            Visit(methodCall.Object);
            VisitExpressionList(methodCall.Arguments);
        }

        protected virtual void VisitList<T>(ReadOnlyCollection<T> list, Action<T> visitor)
        {
            foreach (T element in list)
            {
                visitor(element);
            }
        }

        protected virtual void VisitExpressionList<TExp>(ReadOnlyCollection<TExp> list) where TExp : Expression
        {
            VisitList(list, Visit);
        }

        protected virtual void VisitMemberAssignment(MemberAssignment assignment)
        {
            Visit(assignment.Expression);
        }

        protected virtual void VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            VisitBindingList(binding.Bindings);
        }

        protected virtual void VisitMemberListBinding(MemberListBinding binding)
        {
            VisitElementInitializerList(binding.Initializers);
        }

        protected virtual void VisitBindingList<TBinding>(ReadOnlyCollection<TBinding> list) where TBinding : MemberBinding
        {
            VisitList(list, VisitBinding);
        }

        protected virtual void VisitElementInitializerList(ReadOnlyCollection<ElementInit> list)
        {
            VisitList(list, VisitElementInitializer);
        }

        protected virtual void VisitLambda(LambdaExpression lambda)
        {
            Visit(lambda.Body);
        }

        protected virtual void VisitNew(NewExpression nex)
        {
            VisitExpressionList(nex.Arguments);
        }

        protected virtual void VisitMemberInit(MemberInitExpression init)
        {
            VisitNew(init.NewExpression);
            VisitBindingList(init.Bindings);
        }

        protected virtual void VisitListInit(ListInitExpression init)
        {
            VisitNew(init.NewExpression);
            VisitElementInitializerList(init.Initializers);
        }

        protected virtual void VisitNewArray(NewArrayExpression newArray)
        {
            VisitExpressionList(newArray.Expressions);
        }

        protected virtual void VisitInvocation(InvocationExpression invocation)
        {
            VisitExpressionList(invocation.Arguments);
            Visit(invocation.Expression);
        }
    }

    internal class ExpressionEnumeration : ExpressionVisitor, IEnumerable<Expression>
    {
        private List<Expression> _expressions = new List<Expression>();

        public ExpressionEnumeration(Expression expression)
        {
            Visit(expression);
        }

        protected override void Visit(Expression expression)
        {
            if (expression == null) return;

            _expressions.Add(expression);
            base.Visit(expression);
        }

        public IEnumerator<Expression> GetEnumerator()
        {
            return _expressions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal class HashCodeCalculation : ExpressionVisitor
    {
        private int _hashCode;

        public int HashCode
        {
            get { return _hashCode; }
        }

        public HashCodeCalculation(Expression expression)
        {
            Visit(expression);
        }

        private void Add(int i)
        {
            _hashCode *= 37;
            _hashCode ^= i;
        }

        protected override void Visit(Expression expression)
        {
            if (expression == null) return;

            Add((int)expression.NodeType);
            Add(expression.Type.GetHashCode());

            base.Visit(expression);
        }

        protected override void VisitList<T>(ReadOnlyCollection<T> list, Action<T> visitor)
        {
            Add(list.Count);

            base.VisitList<T>(list, visitor);
        }

        protected override void VisitConstant(ConstantExpression constant)
        {
            if (constant != null && constant.Value != null) Add(constant.Value.GetHashCode());
        }

        protected override void VisitMemberAccess(MemberExpression member)
        {
            Add(member.Member.GetHashCode());

            base.VisitMemberAccess(member);
        }

        protected override void VisitMethodCall(MethodCallExpression methodCall)
        {
            Add(methodCall.Method.GetHashCode());

            base.VisitMethodCall(methodCall);
        }

        protected override void VisitParameter(ParameterExpression parameter)
        {
            Add(parameter.Name.GetHashCode());
        }

        protected override void VisitTypeIs(TypeBinaryExpression type)
        {
            Add(type.TypeOperand.GetHashCode());

            base.VisitTypeIs(type);
        }

        protected override void VisitBinary(BinaryExpression binary)
        {
            if (binary.Method != null) Add(binary.Method.GetHashCode());
            if (binary.IsLifted) Add(1);
            if (binary.IsLiftedToNull) Add(1);

            base.VisitBinary(binary);
        }

        protected override void VisitUnary(UnaryExpression unary)
        {
            if (unary.Method != null) Add(unary.Method.GetHashCode());
            if (unary.IsLifted) Add(1);
            if (unary.IsLiftedToNull) Add(1);

            base.VisitUnary(unary);
        }

        protected override void VisitNew(NewExpression nex)
        {
            Add(nex.Constructor.GetHashCode());
            VisitList(nex.Members, member => Add(member.GetHashCode()));

            base.VisitNew(nex);
        }
    }
}
