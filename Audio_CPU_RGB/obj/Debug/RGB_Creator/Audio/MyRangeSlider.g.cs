﻿#pragma checksum "..\..\..\..\RGB_Creator\Audio\MyRangeSlider.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "724E286AA44B8C891F97638720AB9648"
//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

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


namespace AudioCPURGB {
    
    
    /// <summary>
    /// MyRangeSlider
    /// </summary>
    public partial class MyRangeSlider : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 37 "..\..\..\..\RGB_Creator\Audio\MyRangeSlider.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas ContainerCanvas;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\..\RGB_Creator\Audio\MyRangeSlider.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.Thumb MinThumb;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\..\..\RGB_Creator\Audio\MyRangeSlider.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.Thumb MaxThumb;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\..\..\RGB_Creator\Audio\MyRangeSlider.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle ActiveRectangle;
        
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
            System.Uri resourceLocater = new System.Uri("/Audio-CPU-RGB;component/rgb_creator/audio/myrangeslider.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\RGB_Creator\Audio\MyRangeSlider.xaml"
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
            this.ContainerCanvas = ((System.Windows.Controls.Canvas)(target));
            
            #line 37 "..\..\..\..\RGB_Creator\Audio\MyRangeSlider.xaml"
            this.ContainerCanvas.SizeChanged += new System.Windows.SizeChangedEventHandler(this.ContainerCanvas_SizeChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            this.MinThumb = ((System.Windows.Controls.Primitives.Thumb)(target));
            
            #line 38 "..\..\..\..\RGB_Creator\Audio\MyRangeSlider.xaml"
            this.MinThumb.DragCompleted += new System.Windows.Controls.Primitives.DragCompletedEventHandler(this.MinThumb_DragCompleted);
            
            #line default
            #line hidden
            
            #line 38 "..\..\..\..\RGB_Creator\Audio\MyRangeSlider.xaml"
            this.MinThumb.DragDelta += new System.Windows.Controls.Primitives.DragDeltaEventHandler(this.MinThumb_DragDelta);
            
            #line default
            #line hidden
            return;
            case 3:
            this.MaxThumb = ((System.Windows.Controls.Primitives.Thumb)(target));
            
            #line 39 "..\..\..\..\RGB_Creator\Audio\MyRangeSlider.xaml"
            this.MaxThumb.DragCompleted += new System.Windows.Controls.Primitives.DragCompletedEventHandler(this.MaxThumb_DragCompleted);
            
            #line default
            #line hidden
            
            #line 39 "..\..\..\..\RGB_Creator\Audio\MyRangeSlider.xaml"
            this.MaxThumb.DragDelta += new System.Windows.Controls.Primitives.DragDeltaEventHandler(this.MaxThumb_DragDelta);
            
            #line default
            #line hidden
            return;
            case 4:
            this.ActiveRectangle = ((System.Windows.Shapes.Rectangle)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

