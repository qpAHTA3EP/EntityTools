using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace HarmonyLib
{
	/// <summary>A PatchProcessor handles patches on a method/constructor</summary>
	/// 
	public class PatchProcessor
	{
		readonly Harmony instance;
		readonly MethodBase original;

		HarmonyMethod prefix;
		HarmonyMethod postfix;
		HarmonyMethod transpiler;
		HarmonyMethod finalizer;

		internal static readonly object locker = new object();

		/// <summary>Creates an empty patch processor</summary>
		/// <param name="instance">The Harmony instance</param>
		/// <param name="original">The original method/constructor</param>
		///
		public PatchProcessor(Harmony instance, MethodBase original)
		{
			this.instance = instance;
			this.original = original;
		}

		/// <summary>Adds a prefix</summary>
		/// <param name="prefix">The prefix as a <see cref="HarmonyMethod"/></param>
		/// <returns>A <see cref="PatchProcessor"/> for chaining calls</returns>
		///
		public PatchProcessor AddPrefix(HarmonyMethod prefix)
		{
			this.prefix = prefix;
			return this;
		}

		/// <summary>Adds a prefix</summary>
		/// <param name="fixMethod">The prefix method</param>
		/// <returns>A <see cref="PatchProcessor"/> for chaining calls</returns>
		///
		public PatchProcessor AddPrefix(MethodInfo fixMethod)
		{
			prefix = new HarmonyMethod(fixMethod);
			return this;
		}

		/// <summary>Adds a postfix</summary>
		/// <param name="postfix">The postfix as a <see cref="HarmonyMethod"/></param>
		/// <returns>A <see cref="PatchProcessor"/> for chaining calls</returns>
		///
		public PatchProcessor AddPostfix(HarmonyMethod postfix)
		{
			this.postfix = postfix;
			return this;
		}

		/// <summary>Adds a postfix</summary>
		/// <param name="fixMethod">The postfix method</param>
		/// <returns>A <see cref="PatchProcessor"/> for chaining calls</returns>
		///
		public PatchProcessor AddPostfix(MethodInfo fixMethod)
		{
			postfix = new HarmonyMethod(fixMethod);
			return this;
		}

		/// <summary>Adds a transpiler</summary>
		/// <param name="transpiler">The transpiler as a <see cref="HarmonyMethod"/></param>
		/// <returns>A <see cref="PatchProcessor"/> for chaining calls</returns>
		///
		public PatchProcessor AddTranspiler(HarmonyMethod transpiler)
		{
			this.transpiler = transpiler;
			return this;
		}

		/// <summary>Adds a transpiler</summary>
		/// <param name="fixMethod">The transpiler method</param>
		/// <returns>A <see cref="PatchProcessor"/> for chaining calls</returns>
		///
		public PatchProcessor AddTranspiler(MethodInfo fixMethod)
		{
			transpiler = new HarmonyMethod(fixMethod);
			return this;
		}

		/// <summary>Adds a finalizer</summary>
		/// <param name="finalizer">The finalizer as a <see cref="HarmonyMethod"/></param>
		/// <returns>A <see cref="PatchProcessor"/> for chaining calls</returns>
		///
		public PatchProcessor AddFinalizer(HarmonyMethod finalizer)
		{
			this.finalizer = finalizer;
			return this;
		}

		/// <summary>Adds a finalizer</summary>
		/// <param name="fixMethod">The finalizer method</param>
		/// <returns>A <see cref="PatchProcessor"/> for chaining calls</returns>
		///
		public PatchProcessor AddFinalizer(MethodInfo fixMethod)
		{
			finalizer = new HarmonyMethod(fixMethod);
			return this;
		}

		/// <summary>Gets all patched original methods in the appdomain</summary>
		/// <returns>An enumeration of patched method/constructor</returns>
		///
		public static IEnumerable<MethodBase> GetAllPatchedMethods()
		{
			lock (locker)
			{
				return HarmonySharedState.GetPatchedMethods();
			}
		}

		/// <summary>Applies all registered patches</summary>
		/// <returns>The generated replacement method</returns>
		///
		public MethodInfo Patch()
		{
			if (original == null)
				throw new NullReferenceException($"Null method for {instance.Id}");

			if (original.IsDeclaredMember() == false)
			{
				var declaredMember = original.GetDeclaredMember();
				throw new ArgumentException($"You can only patch implemented methods/constructors. Path the declared method {declaredMember.FullDescription()} instead.");
			}

			lock (locker)
			{
				var patchInfo = HarmonySharedState.GetPatchInfo(original);
				if (patchInfo == null) patchInfo = new PatchInfo();

				PatchFunctions.AddPrefix(patchInfo, instance.Id, prefix);
				PatchFunctions.AddPostfix(patchInfo, instance.Id, postfix);
				PatchFunctions.AddTranspiler(patchInfo, instance.Id, transpiler);
				PatchFunctions.AddFinalizer(patchInfo, instance.Id, finalizer);
				var replacement = PatchFunctions.UpdateWrapper(original, patchInfo);

				HarmonySharedState.UpdatePatchInfo(original, patchInfo);
				return replacement;
			}
		}

		/// <summary>Unpatches patches of a given type and/or Harmony ID</summary>
		/// <param name="type">The <see cref="HarmonyPatchType"/> patch type</param>
		/// <param name="harmonyID">Harmony ID or <c>*</c> for any</param>
		/// <returns>A <see cref="PatchProcessor"/> for chaining calls</returns>
		///
		public PatchProcessor Unpatch(HarmonyPatchType type, string harmonyID)
		{
			lock (locker)
			{
				var patchInfo = HarmonySharedState.GetPatchInfo(original);
				if (patchInfo == null) patchInfo = new PatchInfo();

				if (type == HarmonyPatchType.All || type == HarmonyPatchType.Prefix)
					PatchFunctions.RemovePrefix(patchInfo, harmonyID);
				if (type == HarmonyPatchType.All || type == HarmonyPatchType.Postfix)
					PatchFunctions.RemovePostfix(patchInfo, harmonyID);
				if (type == HarmonyPatchType.All || type == HarmonyPatchType.Transpiler)
					PatchFunctions.RemoveTranspiler(patchInfo, harmonyID);
				if (type == HarmonyPatchType.All || type == HarmonyPatchType.Finalizer)
					PatchFunctions.RemoveFinalizer(patchInfo, harmonyID);
				_ = PatchFunctions.UpdateWrapper(original, patchInfo);

				HarmonySharedState.UpdatePatchInfo(original, patchInfo);
				return this;
			}
		}

		/// <summary>Unpatches a specific patch</summary>
		/// <param name="patch">The method of the patch</param>
		/// <returns>A <see cref="PatchProcessor"/> for chaining calls</returns>
		///
		public PatchProcessor Unpatch(MethodInfo patch)
		{
			lock (locker)
			{
				var patchInfo = HarmonySharedState.GetPatchInfo(original);
				if (patchInfo == null) patchInfo = new PatchInfo();

				PatchFunctions.RemovePatch(patchInfo, patch);
				_ = PatchFunctions.UpdateWrapper(original, patchInfo);

				HarmonySharedState.UpdatePatchInfo(original, patchInfo);
				return this;
			}
		}

		/// <summary>Gets patch information on an original</summary>
		/// <param name="method">The original method/constructor</param>
		/// <returns>The patch information as <see cref="Patches"/></returns>
		///
		public static Patches GetPatchInfo(MethodBase method)
		{
			PatchInfo patchInfo;
			lock (locker) { patchInfo = HarmonySharedState.GetPatchInfo(method); }
			if (patchInfo == null) return null;
			return new Patches(patchInfo.prefixes, patchInfo.postfixes, patchInfo.transpilers, patchInfo.finalizers);
		}

		/// <summary>Gets Harmony version for all active Harmony instances</summary>
		/// <param name="currentVersion">[out] The current Harmony version</param>
		/// <returns>A dictionary containing assembly version keyed by Harmony ID</returns>
		///
		public static Dictionary<string, Version> VersionInfo(out Version currentVersion)
		{
			currentVersion = typeof(Harmony).Assembly.GetName().Version;
			var assemblies = new Dictionary<string, Assembly>();
			GetAllPatchedMethods().Do(method =>
			{
				PatchInfo info;
				lock (locker) { info = HarmonySharedState.GetPatchInfo(method); }
				info.prefixes.Do(fix => assemblies[fix.owner] = fix.PatchMethod.DeclaringType.Assembly);
				info.postfixes.Do(fix => assemblies[fix.owner] = fix.PatchMethod.DeclaringType.Assembly);
				info.transpilers.Do(fix => assemblies[fix.owner] = fix.PatchMethod.DeclaringType.Assembly);
				info.finalizers.Do(fix => assemblies[fix.owner] = fix.PatchMethod.DeclaringType.Assembly);
			});

			var result = new Dictionary<string, Version>();
			assemblies.Do(info =>
			{
				var assemblyName = info.Value.GetReferencedAssemblies().FirstOrDefault(a => a.FullName.StartsWith("0Harmony, Version", StringComparison.Ordinal));
				if (assemblyName != null)
					result[info.Key] = assemblyName.Version;
			});
			return result;
		}

		/// <summary>Creates a new <see cref="ILGenerator">generator</see> to use when reading method bodies</summary>
		/// <returns>A new <see cref="ILGenerator"/></returns>
		/// 
		public static ILGenerator CreateILGenerator()
		{
			var method = new DynamicMethodDefinition($"ILGenerator_{Guid.NewGuid()}", typeof(void), new Type[0]);
			return method.GetILGenerator();
		}

		/// <summary>Returns the methods unmodified list of code instructions</summary>
		/// <param name="original">The original method/constructor</param>
		/// <param name="generator">Optionally an existing generator that will be used to create all local variables and labels contained in the result (if not specified, an internal generator is used)</param>
		/// <returns>A list containing all the original <see cref="CodeInstruction"/></returns>
		/// 
		public static List<CodeInstruction> GetOriginalInstructions(MethodBase original, ILGenerator generator = null)
		{
			var reader = MethodBodyReader.GetInstructions(generator ?? CreateILGenerator(), original);
			return reader.Select(ins => ins.GetCodeInstruction()).ToList();
		}

		/// <summary>Returns the methods unmodified list of code instructions</summary>
		/// <param name="original">The original method/constructor</param>
		/// <param name="generator">A new generator that now contains all local variables and labels contained in the result</param>
		/// <returns>A list containing all the original <see cref="CodeInstruction"/></returns>
		/// 
		public static List<CodeInstruction> GetOriginalInstructions(MethodBase original, out ILGenerator generator)
		{
			generator = CreateILGenerator();
			var reader = MethodBodyReader.GetInstructions(generator, original);
			return reader.Select(ins => ins.GetCodeInstruction()).ToList();
		}

		/// <summary>A low level way to read the body of a method. Used for quick searching in methods</summary>
		/// <param name="method">The original method</param>
		/// <returns>All instructions as opcode/operand pairs</returns>
		///
		public static IEnumerable<KeyValuePair<OpCode, object>> ReadMethodBody(MethodBase method)
		{
			return MethodBodyReader.GetInstructions(CreateILGenerator(), method)
				.Select(instr => new KeyValuePair<OpCode, object>(instr.opcode, instr.operand));
		}

		/// <summary>A low level way to read the body of a method. Used for quick searching in methods</summary>
		/// <param name="method">The original method</param>
		/// <param name="generator">An existing generator that will be used to create all local variables and labels contained in the result</param>
		/// <returns>All instructions as opcode/operand pairs</returns>
		///
		public static IEnumerable<KeyValuePair<OpCode, object>> ReadMethodBody(MethodBase method, ILGenerator generator)
		{
			return MethodBodyReader.GetInstructions(generator, method)
				.Select(instr => new KeyValuePair<OpCode, object>(instr.opcode, instr.operand));
		}
	}
}