using System;

namespace SickDev.CommandSystem {
    public class DelegateCommand : Command {
        public DelegateCommand(Delegate _delegate, string alias = null, string description = null) : base(_delegate.Method, alias, description) { }
    }

    public class ActionCommand : DelegateCommand {
        public ActionCommand(Action method, string alias = null, string description = null) : base(method, alias, description) { }
    }

    public class ActionCommand<T1> : DelegateCommand {
        public ActionCommand(Action<T1> method, string alias = null, string description = null) : base(method, alias, description) { }
    }

    public class ActionCommand<T1, T2> : DelegateCommand {
        public ActionCommand(Action<T1, T2> method, string alias = null, string description = null) : base(method, alias, description) { }
    }

    public class ActionCommand<T1, T2, T3> : DelegateCommand {
        public ActionCommand(Action<T1, T2, T3> method, string alias = null, string description = null) : base(method, alias, description) { }
    }

    public class ActionCommand<T1, T2, T3, T4> : DelegateCommand {
        public ActionCommand(Action<T1, T2, T3, T4> method, string alias = null, string description = null) : base(method, alias, description) { }
    }

    public class FuncCommand<TResult> : DelegateCommand {
        public FuncCommand(Func<TResult> method, string alias = null, string description = null) : base(method, alias, description) { }
    }

    public class FuncCommand<T1, TResult> : DelegateCommand {
        public FuncCommand(Func<T1, TResult> method, string alias = null, string description = null) : base(method, alias, description) { }
    }

    public class FuncCommand<T1, T2, TResult> : DelegateCommand {
        public FuncCommand(Func<T1, T2, TResult> method, string alias = null, string description = null) : base(method, alias, description) { }
    }

    public class FuncCommand<T1, T2, T3, TResult> : DelegateCommand {
        public FuncCommand(Func<T1, T2, T3, TResult> method, string alias = null, string description = null) : base(method, alias, description) { }
    }

    public class FuncCommand<T1, T2, T3, T4, TResult> : DelegateCommand {
        public FuncCommand(Func<T1, T2, T3, T4, TResult> method, string alias = null, string description = null) : base(method, alias, description) { }
    }
}