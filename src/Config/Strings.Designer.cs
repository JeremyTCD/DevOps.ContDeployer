﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace JeremyTCD.PipelinesCE.Config {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("JeremyTCD.PipelinesCE.ConfigHost.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multiple deps.json files found in directory &quot;{0}&quot;.
        /// </summary>
        public static string Exception_MultipleDepsFiles {
            get {
                return ResourceManager.GetString("Exception_MultipleDepsFiles", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multiple PipelineFactorys found, please specify one:
        ///{0}.
        /// </summary>
        public static string Exception_MultiplePipelineFactories {
            get {
                return ResourceManager.GetString("Exception_MultiplePipelineFactories", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multiple PipelineFactorys build a Pipeline with name &quot;{0}&quot;:
        ///{1}.
        /// </summary>
        public static string Exception_MultiplePipelineFactoriesWithSameName {
            get {
                return ResourceManager.GetString("Exception_MultiplePipelineFactoriesWithSameName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No deps.json files found in directory &quot;{0}&quot;.
        /// </summary>
        public static string Exception_NoDepsFiles {
            get {
                return ResourceManager.GetString("Exception_NoDepsFiles", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No PipelineFactorys.
        /// </summary>
        public static string Exception_NoPipelineFactories {
            get {
                return ResourceManager.GetString("Exception_NoPipelineFactories", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No PipelineFactory builds a Pipeline with name &quot;{0}&quot;.
        /// </summary>
        public static string Exception_NoPipelineFactory {
            get {
                return ResourceManager.GetString("Exception_NoPipelineFactory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Configuring ServiceCollection for IPlugin implementation &quot;{0}&quot; using IPluginStartup implementation &quot;{1}&quot;.
        /// </summary>
        public static string Log_ConfiguringPluginServices {
            get {
                return ResourceManager.GetString("Log_ConfiguringPluginServices", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to == Creating Pipeline &quot;{0}&quot; ==.
        /// </summary>
        public static string Log_CreatingPipeline {
            get {
                return ResourceManager.GetString("Log_CreatingPipeline", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Creating PipelineFactory for Pipeline &quot;{0}&quot;.
        /// </summary>
        public static string Log_CreatingPipelineFactory {
            get {
                return ResourceManager.GetString("Log_CreatingPipelineFactory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Creating IoC container for IPlugin implementation &quot;{0}&quot;.
        /// </summary>
        public static string Log_CreatingPluginIoCContainer {
            get {
                return ResourceManager.GetString("Log_CreatingPluginIoCContainer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to == Creating Plugin IoC Containers ==.
        /// </summary>
        public static string Log_CreatingPluginIoCContainers {
            get {
                return ResourceManager.GetString("Log_CreatingPluginIoCContainers", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to PipelinesCE executable and config project versions do not match. The following options will be ignored:{0}
        ///The following options will be assigned their default values:{1}
        ///.
        /// </summary>
        public static string Log_ExecutableAndProjectVersionsDoNotMatch {
            get {
                return ResourceManager.GetString("Log_ExecutableAndProjectVersionsDoNotMatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to === Finished running Pipeline &quot;{0}&quot; ===.
        /// </summary>
        public static string Log_FinishedRunningPipeline {
            get {
                return ResourceManager.GetString("Log_FinishedRunningPipeline", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to == Finished running Plugin &quot;{0}&quot; ==.
        /// </summary>
        public static string Log_FinishedRunningPlugin {
            get {
                return ResourceManager.GetString("Log_FinishedRunningPlugin", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to == Loading Assemblies ==.
        /// </summary>
        public static string Log_LoadingAssemblies {
            get {
                return ResourceManager.GetString("Log_LoadingAssemblies", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to === Loading Pipeline ===.
        /// </summary>
        public static string Log_LoadingPipeline {
            get {
                return ResourceManager.GetString("Log_LoadingPipeline", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Resolved Default Pipeline to &quot;{0}&quot;.
        /// </summary>
        public static string Log_ResolvedDefaultPipeline {
            get {
                return ResourceManager.GetString("Log_ResolvedDefaultPipeline", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to === Running Pipeline &quot;{0}&quot; ===.
        /// </summary>
        public static string Log_RunningPipeline {
            get {
                return ResourceManager.GetString("Log_RunningPipeline", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to == Running Plugin &quot;{0}&quot; ==.
        /// </summary>
        public static string Log_RunningPlugin {
            get {
                return ResourceManager.GetString("Log_RunningPlugin", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to == Successfully created Pipeline &quot;{0}&quot; ==.
        /// </summary>
        public static string Log_SuccessfullyCreatedPipeline {
            get {
                return ResourceManager.GetString("Log_SuccessfullyCreatedPipeline", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to == Successfully Created Plugin IoC Containers ==.
        /// </summary>
        public static string Log_SuccessfullyCreatedPluginContainers {
            get {
                return ResourceManager.GetString("Log_SuccessfullyCreatedPluginContainers", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to == Successfully loaded Assemblies ==.
        /// </summary>
        public static string Log_SuccessfullyLoadedAssemblies {
            get {
                return ResourceManager.GetString("Log_SuccessfullyLoadedAssemblies", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to === Successfully loaded Pipeline ===.
        /// </summary>
        public static string Log_SuccessfullyLoadedPipeline {
            get {
                return ResourceManager.GetString("Log_SuccessfullyLoadedPipeline", resourceCulture);
            }
        }
    }
}
