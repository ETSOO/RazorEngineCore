using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis;

namespace RazorEngineCore
{
    public class RazorEngineCompilationOptionsBuilder : IRazorEngineCompilationOptionsBuilder
    {
        public RazorEngineCompilationOptions Options { get; set; }

        public RazorEngineCompilationOptionsBuilder(RazorEngineCompilationOptions? options = null)
        {
            Options = options ?? new RazorEngineCompilationOptions();
        }

        /// <summary>
        /// Loads the Assembly by name and adds it to the Engine's Assembly Reference list
        /// </summary>
        /// <param name="assemblyName">Full Name of the Assembly to add to the assembly list</param>
        public void AddAssemblyReferenceByName(string assemblyName)
        {
            Assembly assembly = Assembly.Load(new AssemblyName(assemblyName));
            AddAssemblyReference(assembly);
        }

        /// <summary>
        /// Adds a loaded assembly to the Engine's Assembly Reference list
        /// </summary>
        /// <param name="assembly">Assembly to add to the assembly list</param>
        public void AddAssemblyReference(Assembly assembly)
        {
            Options.ReferencedAssemblies.Add(assembly);
        }

        /// <summary>
        /// <para>Adds a type's assembly to the Engine's Assembly Reference list</para>
        /// <para>Also adds the type's GenericTypeArguments to the Reference list as well</para>
        /// </summary>
        /// <param name="type">The type who's assembly should be added to the assembly list</param>
        public void AddAssemblyReference(Type type)
        {
            AddAssemblyReference(type.Assembly);

            foreach (Type argumentType in type.GenericTypeArguments)
            {
                AddAssemblyReference(argumentType);
            }
        }

        /// <summary>
        /// Adds a MetadataReference for use in the Engine's Assembly Reference generation
        /// </summary>
        /// <param name="reference">Metadata Reference to add to the Engine's Referenced Assemblies</param>
        public void AddMetadataReference(MetadataReference reference)
        {
            Options.MetadataReferences.Add(reference);
        }

        /// <summary>
            /// <para>Adds a default <c>using</c> to the compiled view. This is equivalent to adding <c>@using [NAMESPACE]</c> to the template rendered by the engine'</para>
            /// Current Defaults: 
            /// <list type="bullet">
            ///     <listheader></listheader>
            ///     <item>System.Linq</item>
            ///     <item>System.Collections</item>
            ///     <item>System.Collections.Generic</item>
            /// </list>
        /// </summary>
        /// <param name="namespaceName">Namespace to add to default usings</param>
        public void AddUsing(string namespaceName)
        {
            Options.DefaultUsings.Add(namespaceName);
        }

        /// <summary>
        /// Adds <c>@inherits</c> directive to the compiled template
        /// </summary>
        /// <param name="type">Type to <c>@inherits</c> from</param>
        public void Inherits(Type type)
        {
            Options.Inherits = RenderTypeName(type);
            AddAssemblyReference(type);
        }

        private string RenderTypeName(Type type)
        {
            IList<string?> elements =
            [
                type.Namespace,
                type.DeclaringType == null ? null : RenderDeclaringType(type.DeclaringType),
                type.Name
            ];

            var result = string.Join(".", elements.Where(e => !string.IsNullOrEmpty(e)));

            var tildeLocation = result.IndexOf('`');
            if (tildeLocation > -1)
            {
                result = result[..tildeLocation];
            }

            if (type.GenericTypeArguments.Length == 0)
            {
                return result;
            }

            return result + "<" + string.Join(",", type.GenericTypeArguments.Select(RenderTypeName)) + ">";
        }

        private string RenderDeclaringType(Type type)
        {
            if (type.DeclaringType != null)
            {
                var parent = RenderDeclaringType(type.DeclaringType);

                if (!string.IsNullOrEmpty(parent))
                {
                    return parent + "." + type.Name;
                }
            }

            return type.Name;
        }

        /// <summary>
        /// Enables debug info
        /// </summary>
        public void IncludeDebuggingInfo()
        {
            Options.IncludeDebuggingInfo = true;
        }

        /// <summary>
        /// Enables debug info
        /// </summary>
        public void ConfigureRazorEngineProject(Action<RazorProjectEngineBuilder> configure)
        {
            Options.ProjectEngineBuilder = configure;
        }

    }
}
