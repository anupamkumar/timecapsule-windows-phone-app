﻿#pragma checksum "C:\Users\charlie0.o\Documents\Visual Studio 2013\Projects\timecapsule-test\timecapsule_test\AddImage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "052B6D5947DE0DC28BEC9157AD0C4928"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.33440
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace timecapsule_test {
    
    
    public partial class AddImage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.TextBox TodoInput;
        
        internal System.Windows.Controls.Button ButtonCaptureImage;
        
        internal System.Windows.Controls.Button ButtonSave;
        
        internal System.Windows.Controls.Button ButtonRefresh;
        
        internal Microsoft.Phone.Controls.LongListSelector ListItems;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/timecapsule_test;component/AddImage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.TodoInput = ((System.Windows.Controls.TextBox)(this.FindName("TodoInput")));
            this.ButtonCaptureImage = ((System.Windows.Controls.Button)(this.FindName("ButtonCaptureImage")));
            this.ButtonSave = ((System.Windows.Controls.Button)(this.FindName("ButtonSave")));
            this.ButtonRefresh = ((System.Windows.Controls.Button)(this.FindName("ButtonRefresh")));
            this.ListItems = ((Microsoft.Phone.Controls.LongListSelector)(this.FindName("ListItems")));
        }
    }
}

