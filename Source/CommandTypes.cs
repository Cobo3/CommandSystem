using System;

namespace SickDev.CommandSystem {
    public class ActionCommand : CommandBase {
        public ActionCommand(Action method, string description, params string[] aliases) : base(method, description, aliases) { }
    }

    public class ActionCommand<T1> : CommandBase {
        public ActionCommand(Action<T1> method, string description, params string[] aliases) : base(method, description, aliases) { }
    }

    public class ActionCommand<T1, T2> : CommandBase {
        public ActionCommand(Action<T1, T2> method, string description, params string[] aliases) : base(method, description, aliases) { }
    }

    public class ActionCommand<T1, T2, T3> : CommandBase {
        public ActionCommand(Action<T1, T2, T3> method, string description, params string[] aliases) : base(method, description, aliases) { }
    }

    public class ActionCommand<T1, T2, T3, T4> : CommandBase {
        public ActionCommand(Action<T1, T2, T3, T4> method, string description, params string[] aliases) : base(method, description, aliases) { }
    }

    public class FuncCommand<TResult> : CommandBase {
        public FuncCommand(Func<TResult> method, string description, params string[] aliases) : base(method, description, aliases) { }
    }

    public class FuncCommand<T1, TResult> : CommandBase {
        public FuncCommand(Func<T1, TResult> method, string description, params string[] aliases) : base(method, description, aliases) { }
    }

    public class FuncCommand<T1, T2, TResult> : CommandBase {
        public FuncCommand(Func<T1, T2, TResult> method, string description, params string[] aliases) : base(method, description, aliases) { }
    }

    public class FuncCommand<T1, T2, T3, TResult> : CommandBase {
        public FuncCommand(Func<T1, T2, T3, TResult> method, string description, params string[] aliases) : base(method, description, aliases) { }
    }

    public class FuncCommand<T1, T2, T3, T4, TResult> : CommandBase {
        public FuncCommand(Func<T1, T2, T3, T4, TResult> method, string description, params string[] aliases) : base(method, description, aliases) { }
    }
}