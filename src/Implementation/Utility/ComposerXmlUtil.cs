using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Compositional.Composer.CompositionXml;
using Compositional.Composer.CompositionXml.Info;


namespace Compositional.Composer.Utility
{
	public static class ComposerXmlUtil
	{
		#region Public methods

		public static void ProcessCompositionXml(this ComponentContext context, Stream configurationStream)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			var xmlProcessingContext = new XmlProcessingContext(context);
			ProcessCompositionXml(configurationStream, xmlProcessingContext);
			xmlProcessingContext.ThrowIfErrors();
		}

		public static void ProcessCompositionXml(this ComponentContext context, string configurationPath)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			var xmlProcessingContext = new XmlProcessingContext(context);
			ProcessCompositionXml(configurationPath, xmlProcessingContext);
			xmlProcessingContext.ThrowIfErrors();
		}

		public static void ProcessCompositionXmlFromResource(this ComponentContext context, string configurationResourceName)
		{
			context.ProcessCompositionXmlFromResource(Assembly.GetCallingAssembly(), configurationResourceName);
		}

		public static void ProcessCompositionXmlFromResource(this ComponentContext context, Assembly assembly, string configurationResourceName)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			var xmlProcessingContext = new XmlProcessingContext(context);
			ProcessCompositionXmlFromResource(assembly, configurationResourceName, xmlProcessingContext);
			xmlProcessingContext.ThrowIfErrors();
		}

		#endregion

		#region Internal methods

		internal static void ProcessCompositionXml(Stream configurationStream, XmlProcessingContext xmlProcessingContext)
		{
			if (configurationStream == null)
				throw new ArgumentNullException("configurationStream");

			var xsdStream =
				Assembly.GetExecutingAssembly().GetManifestResourceStream(
					"Compositional.Composer.CompositionXml.Schema.compositionXml.1.0.xsd");

			if (xsdStream == null)
				throw new NullReferenceException("Could not load XSD resource from DLL.");
			
			var schema =
				XmlSchema.Read(
					xsdStream,
					delegate(object sender, ValidationEventArgs e)
					{
						throw new CompositionXmlValidationException(
							"Could not load XSD for Composition XML. Message: " + e.Message + "; Sender: " +
							sender, e.Exception);
					});

			var schemaSet = new XmlSchemaSet();
			schemaSet.Add(schema);

			var settings = new XmlReaderSettings {ValidationType = ValidationType.Schema, Schemas = schemaSet};
			settings.ValidationEventHandler += delegate(object sender, ValidationEventArgs e)
			                                   	{
			                                   		var errorMessage = "Composition XML Schema Validation error: ";

			                                   		var validationException =
			                                   			e.Exception as XmlSchemaValidationException;

			                                   		if (validationException != null)
			                                   		{
			                                   			errorMessage += "Line: " + validationException.LineNumber + ", Position: " +
			                                   			                validationException.LinePosition +
			                                   			                "; " + validationException.Message;
			                                   		}
			                                   		else
			                                   		{
			                                   			errorMessage += "Message: " + e.Message + "; Sender: " + sender +
			                                   			                "; Inner exception: " + e.Exception.Message;
			                                   		}

			                                   		xmlProcessingContext.ReportError(errorMessage);
			                                   	};

			var serializer = new XmlSerializer(typeof (CompositionInfo));

			var reader = XmlReader.Create(configurationStream, settings);
			var info = (CompositionInfo) serializer.Deserialize(reader);
			info.Validate();

			CompositionInfoApplicator.ApplyCompositionInfo(info, xmlProcessingContext);
		}

		internal static void ProcessCompositionXml(string configurationPath, XmlProcessingContext xmlProcessingContext)
		{
			if (configurationPath == null)
				throw new ArgumentNullException("configurationPath");

			if (!File.Exists(configurationPath))
			{
				xmlProcessingContext.ReportError(
					string.Format("Specified configuration file '{0}' does not exist.", configurationPath));
				return;
			}

			using (Stream configurationStream = File.Open(configurationPath, FileMode.Open, FileAccess.Read))
			{
				ProcessCompositionXml(configurationStream, xmlProcessingContext);
			}
		}

		internal static void ProcessCompositionXmlFromResource(Assembly assembly, string configurationResourceName,
		                                                       XmlProcessingContext xmlProcessingContext)
		{
			var stream = assembly.GetManifestResourceStream(configurationResourceName);

			if (stream == null)
			{
				xmlProcessingContext.ReportError(
					string.Format("Resource name '{0}' could not be loaded from assembly '{1}'.", configurationResourceName,
					              assembly.FullName));
				return;
			}

			ProcessCompositionXml(stream, xmlProcessingContext);
		}

		#endregion
	}
}