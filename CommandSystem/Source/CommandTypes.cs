using System;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;

namespace SickDev.CommandSystem {
    public class MethodInfoCommand : Command {
        public MethodInfoCommand(MethodInfo method, string alias = null, string description = null) : base(MakeDelegate(method), alias, description) { }

        static Delegate MakeDelegate(MethodInfo method) {
            Type delegateType;
            Type[] typeArgs = method.GetParameters().ToList().ConvertAll(x => x.ParameterType).ToArray();
            if(method.ReturnType == typeof(void))
                delegateType = Expression.GetActionType(typeArgs);
            else {
                typeArgs = typeArgs.Concat(new[] { method.ReturnType }).ToArray();
                delegateType = Expression.GetFuncType(typeArgs);
            }
            Delegate deleg = Delegate.CreateDelegate(delegateType, method);
            return deleg;
        }
    }

    public class ActionCommand : Command {
        public ActionCommand(Action method, string alias = null, string description = null) : base(method, alias, description) { }
    }

    public class ActionCommand<T1> : Command {
        public ActionCommand(Action<T1> method, string alias = null, string description = null) : base(method, alias, description) { }
    }

    public class ActionCommand<T1, T2> : Command {
        public ActionCommand(Action<T1, T2> method, string alias = null, string description = null) : base(method, alias, description) { }
    }

    public class ActionCommand<T1, T2, T3> : Command {
        public ActionCommand(Action<T1, T2, T3> method, string alias = null, string description = null) : base(method, alias, description) { }
    }

    public class ActionCommand<T1, T2, T3, T4> : Command {
        public ActionCommand(Action<T1, T2, T3, T4> method, string alias = null, string description = null) : base(method, alias, description) { }
    }

    public class FuncCommand<TResult> : Command {
        public FuncCommand(Func<TResult> method, string alias = null, string description = null) : base(method, alias, description) { }
    }

    public class FuncCommand<T1, TResult> : Command {
        public FuncCommand(Func<T1, TResult> method, string alias = null, string description = null) : base(method, alias, description) { }
    }

    public class FuncCommand<T1, T2, TResult> : Command {
        public FuncCommand(Func<T1, T2, TResult> method, string alias = null, string description = null) : base(method, alias, description) { }
    }

    public class FuncCommand<T1, T2, T3, TResult> : Command {
        public FuncCommand(Func<T1, T2, T3, TResult> method, string alias = null, string description = null) : base(method, alias, description) { }
    }

    public class FuncCommand<T1, T2, T3, T4, TResult> : Command {
        public FuncCommand(Func<T1, T2, T3, T4, TResult> method, string alias = null, string description = null) : base(method, alias, description) { }
    }
}