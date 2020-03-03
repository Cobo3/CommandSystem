using System;
using System.Collections.Generic;

namespace SickDev.CommandSystem
{
	//TODO ParedParameter?
	//TODO ArgumentParser?
	public class ParsedArgument
	{
		static CastInfo[] castInfo;
		static Dictionary<string, CachedCast> cachedCasts = new Dictionary<string, CachedCast>();

		public string raw { get; private set; }
		public string argument { get; private set; }
		public Type type { get; private set; }

		public bool isTypeSpecified => type != null;

		static ParsedArgument()
		{
			Type[] allTypes = ReflectionFinder.allTypes;
			List<CastInfo> castInfoList = new List<CastInfo>();
			for (int i = 0; i < allTypes.Length; i++)
			{
				Type type = allTypes[i];
				string name = type.Name;
				string fullName = type.FullName;
				if (SignatureBuilder.aliases.ContainsKey(type))
					name = fullName = SignatureBuilder.aliases[type];

				castInfoList.Add(new CastInfo(type, name, fullName));
				//Only make array CastInfo when the parameter is not byref
				if (type != typeof(TypedReference) && !type.IsByRef)
					castInfoList.Add(new CastInfo(type.MakeArrayType(), name + "[]", fullName + "[]"));
			}
			castInfo = castInfoList.ToArray();
		}

		public ParsedArgument(string raw)
		{
			this.raw = raw;

			if (!raw.StartsWith("("))
				ParseSimple();
			else
				ParseComplex();
		}

		void ParseSimple()
		{
			type = null;
			argument = raw;
		}

		void ParseComplex()
		{
			//TODO check if this influences a string parameter whose value contains parenthesis
			int index = raw.IndexOf(")");
			argument = raw.Substring(index + 1);
			string castType = raw.Substring(1, index - 1);

			//TODO try get value
			if (!cachedCasts.ContainsKey(castType))
				CreateCachedCast(castType);
			type = GetCastType(castType);
		}

		static void CreateCachedCast(string castType)
		{
			CachedCast cachedCast = new CachedCast(castType);
			for (int i = 0; i < castInfo.Length; i++)
				if (castInfo[i].name.Equals(castType, StringComparison.OrdinalIgnoreCase))
					cachedCast.nameMatches.Add(castInfo[i]);

			//If no name matches, resort to fullname matches
			//TODO both fors are the same...
			if (cachedCast.nameMatches.Count == 0)
				for (int i = 0; i < castInfo.Length; i++)
					if (castInfo[i].fullName.Equals(castType, StringComparison.OrdinalIgnoreCase))
						cachedCast.nameMatches.Add(castInfo[i]);
			cachedCasts.Add(castType, cachedCast);
		}

		static Type GetCastType(string cast)
		{
			CachedCast cachedCast = cachedCasts[cast];

			if (cachedCast.nameMatches.Count == 0)
				throw new ExplicitCastNotFound(cast);
			if (cachedCast.nameMatches.Count > 1)
				throw new AmbiguousExplicitCast(cast, cachedCast.nameMatches.ConvertAll(x => x.type).ToArray());
			return cachedCast.nameMatches[0].type;
		}

		struct CastInfo
		{
			public readonly Type type;
			public readonly string name;
			public readonly string fullName;

			public CastInfo(Type type, string name, string fullName)
			{
				this.type = type;
				this.name = name;
				this.fullName = fullName;
			}
		}

		struct CachedCast
		{
			public readonly string castType;
			public List<CastInfo> nameMatches;

			public CachedCast(string castType)
			{
				this.castType = castType;
				nameMatches = new List<CastInfo>();
			}
		}
	}
}