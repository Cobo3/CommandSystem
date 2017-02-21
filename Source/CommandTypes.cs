﻿using System;

namespace SickDev.CommandSystem {
    public class ActionCommand : CommandBase {
        public ActionCommand(Action method, string alias = null, string description = null) : base(method, alias, description) { }
    }

    public class ActionCommand<T1> : CommandBase {
        public ActionCommand(Action<T1> method, string alias = null, string description = null) : base(method, alias, description) { }
    }

    public class ActionCommand<T1, T2> : CommandBase {
        public ActionCommand(Action<T1, T2> method, string alias = null, string description = null) : base(method, alias, description) { }
    }

    public class ActionCommand<T1, T2, T3> : CommandBase {
        public ActionCommand(Action<T1, T2, T3> method, string alias = null, string description = null) : base(method, alias, description) { }
    }

    public class ActionCommand<T1, T2, T3, T4> : CommandBase {
        public ActionCommand(Action<T1, T2, T3, T4> method, string alias = null, string description = null) : base(method, alias, description) { }
    }

    public class FuncCommand<TResult> : CommandBase {
        public FuncCommand(Func<TResult> method, string alias = null, string description = null) : base(method, alias, description) { }
    }

    public class FuncCommand<T1, TResult> : CommandBase {
        public FuncCommand(Func<T1, TResult> method, string alias = null, string description = null) : base(method, alias, description) { }
    }

    public class FuncCommand<T1, T2, TResult> : CommandBase {
        public FuncCommand(Func<T1, T2, TResult> method, string alias = null, string description = null) : base(method, alias, description) { }
    }

    public class FuncCommand<T1, T2, T3, TResult> : CommandBase {
        public FuncCommand(Func<T1, T2, T3, TResult> method, string alias = null, string description = null) : base(method, alias, description) { }
    }

    public class FuncCommand<T1, T2, T3, T4, TResult> : CommandBase {
        public FuncCommand(Func<T1, T2, T3, T4, TResult> method, string alias = null, string description = null) : base(method, alias, description) { }
    }
}