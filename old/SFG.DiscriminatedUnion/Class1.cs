using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace BrightSword.CSharpExtensions.DiscriminatedUnion
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class ProvideGeneratorAttribute : RegistrationAttribute
    {
        private string _name;

        public ProvideGeneratorAttribute(Type generatorType, string languageServiceGuid)
        {
            if (generatorType == null)
                throw new ArgumentNullException(nameof(generatorType));
            if (languageServiceGuid == null)
                throw new ArgumentNullException(nameof(languageServiceGuid));
            if (string.IsNullOrEmpty(languageServiceGuid))
                throw new ArgumentException("languageServiceGuid cannot be empty");

            GeneratorType = generatorType;
            LanguageServiceGuid = new Guid(languageServiceGuid);
            _name = GeneratorType.Name;
        }

        public Type GeneratorType { get; }
        public Guid LanguageServiceGuid { get; }

        public string Name
        {
            get { return _name; }

            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("value cannot be empty");

                _name = value;
            }
        }

        public string Description { get; set; }
        public bool GeneratesDesignTimeSource { get; set; }

        private string RegistrationKey => $@"Generators\{LanguageServiceGuid.ToString("B")}\{Name}";

        public override void Register(RegistrationContext context)
        {
            using (var key = context.CreateKey(RegistrationKey))
            {
                if (!string.IsNullOrEmpty(Description))
                    key.SetValue(string.Empty, Description);
                key.SetValue("CLSID", GeneratorType.GUID.ToString("B"));
                key.SetValue("GeneratesDesignTimeSource", GeneratesDesignTimeSource ? 1 : 0);
            }
        }

        public override void Unregister(RegistrationContext context)
        {
            context.RemoveKey(RegistrationKey);
        }
    }

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class ProvideAssemblyObjectAttribute : RegistrationAttribute
    {
        public ProvideAssemblyObjectAttribute(Type objectType)
        {
            if (objectType == null)
                throw new ArgumentNullException(nameof(objectType));

            ObjectType = objectType;
        }

        public Type ObjectType { get; }
        public RegistrationMethod RegistrationMethod { get; set; }

        private string ClsidRegKey => $@"CLSID\{ObjectType.GUID.ToString("B")}";

        public override void Register(RegistrationContext context)
        {
            using (var key = context.CreateKey(ClsidRegKey))
            {
                key.SetValue(string.Empty, ObjectType.FullName);
                key.SetValue("InprocServer32", context.InprocServerPath);
                key.SetValue("Class", ObjectType.FullName);
                if (context.RegistrationMethod != RegistrationMethod.Default)
                    RegistrationMethod = context.RegistrationMethod;

                switch (RegistrationMethod)
                {
                    case RegistrationMethod.Default:
                    case RegistrationMethod.Assembly:
                        key.SetValue("Assembly", ObjectType.Assembly.FullName);
                        break;

                    case RegistrationMethod.CodeBase:
                        key.SetValue("CodeBase", context.CodeBase);
                        break;

                    default:
                        throw new InvalidOperationException();
                }

                key.SetValue("ThreadingModel", "Both");
            }
        }

        public override void Unregister(RegistrationContext context)
        {
            context.RemoveKey(ClsidRegKey);
        }
    }

    [Guid("90FFA01D-CFAF-4811-8D2E-677A9097F74A")]
    [ComVisible(true)]
    public class DiscriminatedUnionFileGenerator : IVsSingleFileGenerator
    {
        public int DefaultExtension(out string pbstrDefaultExtension)
        {
            pbstrDefaultExtension = ".union";
            return VSConstants.S_OK;
        }

        public int Generate(
            string wszInputFilePath,
            string bstrInputFileContents,
            string wszDefaultNamespace,
            IntPtr[] rgbOutputFileContents,
            out uint pcbOutput,
            IVsGeneratorProgress pGenerateProgress)
        {
            pcbOutput = 0;
            return VSConstants.S_FALSE;
        }
    }
}