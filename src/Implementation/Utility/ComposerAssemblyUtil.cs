using System;
using System.Linq;
using System.Reflection;

namespace Appson.Composer.Utility
{
	public static class ComposerAssemblyUtil
	{
		#region Public utility methods

		public static void RegisterAssembly(this ComponentContext context, Assembly assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException("assembly");

			if (context == null)
				throw new ArgumentNullException("context");

			ExtractAllComponents(assembly, context);
		}

		public static void RegisterAssembly(this ComponentContext context, string name)
		{
			var assembly = Assembly.Load(name);
			context.RegisterAssembly(assembly);
		}

		public static void RegisterAssemblyFile(this ComponentContext context, string path)
		{
			var assembly = Assembly.LoadFile(path);
			context.RegisterAssembly(assembly);
		}

		#endregion

		#region Private helper methods

		private static void ExtractAllComponents(Assembly assembly, ComponentContext context)
		{
			Type[] types;

			try
			{
				types = assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException rtle)
			{
				var message = "Could not load types of assembly '" + assembly.FullName + "', with the following messages: \n";

				message = rtle.LoaderExceptions.Aggregate(message, (current, exception) => current + (exception.Message + "\n"));

				throw new CompositionException(message);
			}

			var candidateTypes = types
				.Where(ComponentContextUtils.HasComponentAttribute)
				.Where(componentType => !ComponentContextUtils.HasIgnoredOnAssemblyRegistrationAttribute(componentType));

			foreach (var componentType in candidateTypes)
			{
				context.Register(componentType);
			}
		}

		#endregion
	}
}