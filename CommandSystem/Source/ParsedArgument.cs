using System;
using System.Collections.Generic;

namespace SickDev.CommandSystem {
    public class ParsedArgument {
        static CastInfo[] _castInfo;
        static CastInfo[] castInfo {
            get {
                if(_castInfo == null)
                    _castInfo = CreateCastInfo();
                return _castInfo;
            }
        }

        static Dictionary<string, CachedCast> cachedCasts = new Dictionary<string, CachedCast>();

        public string raw { get; private set; }
        public string argument { get; private set; }
        public Type type { get; private set; }

        public bool isTypeSpecified { get { return type != null; } }

        public ParsedArgument(string raw) {
            this.raw = raw;

            if(raw.StartsWith("("))
                ParseComplex();
            else
                ParseSimple();
        }

        void ParseComplex() {
            int index = raw.IndexOf(")");
            argument = raw.Substring(index + 1);
            string cast = raw.Substring(1, index-1);
            if(!cachedCasts.ContainsKey(cast))
                CreateCachedCast(cast);
            type = GetCastType(cast);
        }

        Type GetCastType(string cast) {
            CachedCast cachedCast = cachedCasts[cast];

            if(cachedCast.nameMatches.Count == 0)
                throw new ExplicitCastNotFoundException(cast);
            if(cachedCast.nameMatches.Count > 1)
                throw new AmbiguousExplicitCastException(cast, cachedCast.nameMatches.ConvertAll(x => x.type).ToArray());
            return cachedCast.nameMatches[0].type;
        }

        void CreateCachedCast(string cast) {
            CachedCast cachedCast = new CachedCast(cast);
            for(int i = 0; i < castInfo.Length; i++)
                if(castInfo[i].name.Equals(cast, StringComparison.OrdinalIgnoreCase))
                    cachedCast.nameMatches.Add(castInfo[i]);

            if(cachedCast.nameMatches.Count == 0) {
                for(int i = 0; i < castInfo.Length; i++) {
                    if(castInfo[i].fullName.Equals(cast, StringComparison.OrdinalIgnoreCase)) {
                        cachedCast.nameMatches.Add(castInfo[i]);
                        break;
                    }
                }
            }
            cachedCasts.Add(cast, cachedCast);
        }

        void ParseSimple() {
            type = null;
            argument = raw;
        }

        static CastInfo[] CreateCastInfo() {
            Type[] allTypes = ReflectionFinder.allTypes;
            List<CastInfo> typesInfo = new List<CastInfo>();
            for(int i = 0; i < allTypes.Length; i++) {
                Type type = allTypes[i];
                if(SignatureBuilder.aliases.ContainsKey(type)) {
                    string alias = SignatureBuilder.aliases[type];
                    typesInfo.Add(new CastInfo(type, alias, alias));
                }
                else
                    typesInfo.Add(new CastInfo(type, type.Name, type.FullName));
            }
            return typesInfo.ToArray();
        }

        struct CastInfo {
            public readonly Type type;
            public readonly string name;
            public readonly string fullName;

            public CastInfo(Type type, string name, string fullName) {
                this.type = type;
                this.name = name;
                this.fullName = fullName;
            }
        }

        struct CachedCast {
            public readonly string cast;
            public List<CastInfo> nameMatches;

            public CachedCast(string cast) {
                this.cast = cast;
                nameMatches = new List<CastInfo>();
            }
        }
    }
}