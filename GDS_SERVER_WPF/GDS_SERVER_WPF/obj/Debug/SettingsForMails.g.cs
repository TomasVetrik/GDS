﻿#pragma checksum "..\..\SettingsForMails.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "B7C1B55C06554B66DC4C4698CB7EEC616BE680793FEC71D24C0165F2F87297A8"
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
    /// SettingsForMails
    /// </summary>
    public partial class SettingsForMails : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 24 "..\..\SettingsForMails.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox textBoxMail;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\SettingsForMails.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button buttonNew;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\SettingsForMails.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView listViewMails;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\SettingsForMails.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button buttonToActive;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\SettingsForMails.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button buttonToInActive;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\SettingsForMails.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView listViewMailsInActive;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\SettingsForMails.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button buttonOK;
        
        #line default
        #line hidden
        
        
        #line 49 "..\..\SettingsForMails.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelError;
        
        #line default
        #line hidden
        
        
        #line 50 "..\..\SettingsForMails.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button buttonCancel;
        
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
            System.Uri resourceLocater = new System.Uri("/GDS_SERVER_WPF;component/settingsformails.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\SettingsForMails.xaml"
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
            
            #line 8 "..\..\SettingsForMails.xaml"
            ((GDS_SERVER_WPF.SettingsForMails)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.textBoxMail = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.buttonNew = ((System.Windows.Controls.Button)(target));
            
            #line 25 "..\..\SettingsForMails.xaml"
            this.buttonNew.Click += new System.Windows.RoutedEventHandler(this.buttonNew_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.listViewMails = ((System.Windows.Controls.ListView)(target));
            
            #line 28 "..\..\SettingsForMails.xaml"
            this.listViewMails.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.ListViewMails_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 32 "..\..\SettingsForMails.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.MenuItemRemove_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.buttonToActive = ((System.Windows.Controls.Button)(target));
            
            #line 36 "..\..\SettingsForMails.xaml"
            this.buttonToActive.Click += new System.Windows.RoutedEventHandler(this.ButtonToActive_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.buttonToInActive = ((System.Windows.Controls.Button)(target));
            
            #line 37 "..\..\SettingsForMails.xaml"
            this.buttonToInActive.Click += new System.Windows.RoutedEventHandler(this.ButtonToInActive_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.listViewMailsInActive = ((System.Windows.Controls.ListView)(target));
            
            #line 39 "..\..\SettingsForMails.xaml"
            this.listViewMailsInActive.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.ListViewMailsInActive_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 43 "..\..\SettingsForMails.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.MenuItemRemoveInActive_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.buttonOK = ((System.Windows.Controls.Button)(target));
            
            #line 48 "..\..\SettingsForMails.xaml"
            this.buttonOK.Click += new System.Windows.RoutedEventHandler(this.buttonOK_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            this.labelError = ((System.Windows.Controls.Label)(target));
            return;
            case 12:
            this.buttonCancel = ((System.Windows.Controls.Button)(target));
            
            #line 50 "..\..\SettingsForMails.xaml"
            this.buttonCancel.Click += new System.Windows.RoutedEventHandler(this.buttonCancel_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

