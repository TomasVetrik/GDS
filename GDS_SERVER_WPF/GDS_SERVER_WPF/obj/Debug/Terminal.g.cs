﻿#pragma checksum "..\..\Terminal.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "97418BE2CFF3C318A48C24436A3541D2D5B95231132C1EC8DE4B8EA4F7EFF357"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using GDS_SERVER_WPF;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace GDS_SERVER_WPF {
    
    
    /// <summary>
    /// Terminal
    /// </summary>
    public partial class Terminal : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        /// <summary>
        /// txtBlockTerminal Name Field
        /// </summary>
        
        #line 27 "..\..\Terminal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public System.Windows.Controls.TextBox txtBlockTerminal;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\Terminal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_Run;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\Terminal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_Browse;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\Terminal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_Save;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\Terminal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_Load;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\Terminal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox listBoxComputers;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\Terminal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelUserName;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\Terminal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelPassword;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\Terminal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelStaticIP;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\Terminal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtBoxUserName;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\Terminal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtBoxPassword;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\Terminal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtBoxStaticIP;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\Terminal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_Done;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/GDS_SERVER_WPF;component/terminal.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\Terminal.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 8 "..\..\Terminal.xaml"
            ((GDS_SERVER_WPF.Terminal)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.txtBlockTerminal = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.button_Run = ((System.Windows.Controls.Button)(target));
            
            #line 30 "..\..\Terminal.xaml"
            this.button_Run.Click += new System.Windows.RoutedEventHandler(this.Button_Run_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.button_Browse = ((System.Windows.Controls.Button)(target));
            
            #line 31 "..\..\Terminal.xaml"
            this.button_Browse.Click += new System.Windows.RoutedEventHandler(this.Button_Browse_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.button_Save = ((System.Windows.Controls.Button)(target));
            
            #line 32 "..\..\Terminal.xaml"
            this.button_Save.Click += new System.Windows.RoutedEventHandler(this.Button_Save_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.button_Load = ((System.Windows.Controls.Button)(target));
            
            #line 33 "..\..\Terminal.xaml"
            this.button_Load.Click += new System.Windows.RoutedEventHandler(this.Button_Load_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.listBoxComputers = ((System.Windows.Controls.ListBox)(target));
            
            #line 34 "..\..\Terminal.xaml"
            this.listBoxComputers.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.ListBoxComputers_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 8:
            this.labelUserName = ((System.Windows.Controls.Label)(target));
            return;
            case 9:
            this.labelPassword = ((System.Windows.Controls.Label)(target));
            return;
            case 10:
            this.labelStaticIP = ((System.Windows.Controls.Label)(target));
            return;
            case 11:
            this.txtBoxUserName = ((System.Windows.Controls.TextBox)(target));
            return;
            case 12:
            this.txtBoxPassword = ((System.Windows.Controls.TextBox)(target));
            return;
            case 13:
            this.txtBoxStaticIP = ((System.Windows.Controls.TextBox)(target));
            return;
            case 14:
            this.button_Done = ((System.Windows.Controls.Button)(target));
            
            #line 47 "..\..\Terminal.xaml"
            this.button_Done.Click += new System.Windows.RoutedEventHandler(this.Button_Done_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
