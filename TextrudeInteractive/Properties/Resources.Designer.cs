﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TextrudeInteractive.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("TextrudeInteractive.Properties.Resources", typeof(Resources).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!DOCTYPE html&gt;
        ///&lt;html&gt;
        ///&lt;head&gt;
        ///    &lt;meta http-equiv=&quot;Content-Type&quot; content=&quot;text/html;charset=utf-8&quot;&gt;
        ///
        ///    &lt;style type=&quot;text/css&quot;&gt;
        ///        html,
        ///        body {
        ///            height: 100%;
        ///            margin: 0;
        ///            overflow: hidden;
        ///        }
        ///
        ///        #container {
        ///            width: 100%;
        ///            height: 100%;
        ///        }
        ///    &lt;/style&gt;
        ///&lt;/head&gt;
        ///&lt;body&gt;
        ///    &lt;div id=&quot;container&quot; /&gt;
        ///
        ///    &lt;script src=&quot;vs/loader.js&quot;&gt;&lt;/script&gt;
        ///    &lt;script&gt;
        ///        require.config({ paths: { &apos;vs&apos;: &apos;vs&apos; }  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string monaco {
            get {
                return ResourceManager.GetString("monaco", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Byte[].
        /// </summary>
        internal static byte[] monaco_editor_0_22_3 {
            get {
                object obj = ResourceManager.GetObject("monaco_editor_0_22_3", resourceCulture);
                return ((byte[])(obj));
            }
        }
    }
}