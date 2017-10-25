using System;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;

namespace SickDev.CommandSystem {
    public class MethodInfoCommand : Command {
        public MethodInfoCommand(MethodInfo method) : base(MakeDelegate(method)) { }

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
        public ActionCommand(Action method) : base(method) { }
    }

    public class ActionCommand<T1> : Command {
        public ActionCommand(Action<T1> method) : base(method) { }
    }

    public class ActionCommand<T1, T2> : Command {
        public ActionCommand(Action<T1, T2> method) : base(method) { }
    }

    public class ActionCommand<T1, T2, T3> : Command {
        public ActionCommand(Action<T1, T2, T3> method) : base(method) { }
    }

    public class ActionCommand<T1, T2, T3, T4> : Command {
        public ActionCommand(Action<T1, T2, T3, T4> method) : base(method) { }
    }

    public class FuncCommand<TResult> : Command {
        public FuncCommand(Func<TResult> method) : base(method) { }
    }

    public class FuncCommand<T1, TResult> : Command {
        public FuncCommand(Func<T1, TResult> method) : base(method) { }
    }

    public class FuncCommand<T1, T2, TResult> : Command {
        public FuncCommand(Func<T1, T2, TResult> method) : base(method) { }
    }

    public class FuncCommand<T1, T2, T3, TResult> : Command {
        public FuncCommand(Func<T1, T2, T3, TResult> method) : base(method) { }
    }

    public class FuncCommand<T1, T2, T3, T4, TResult> : Command {
        public FuncCommand(Func<T1, T2, T3, T4, TResult> method) : base(method) { }
    }
}