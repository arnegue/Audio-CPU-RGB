﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Threading;


namespace AudioCPURGB {
    /// <summary>
    /// Interaction logic for MyRangeSlider.xaml
    /// </summary>
    public sealed partial class MyRangeSlider : UserControl {

        public double Minimum {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public double MinDifference {
            get { return (double)GetValue(MinDifferenceProperty); }
            set { SetValue(MinDifferenceProperty, value); }
        }

        public double Maximum {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public double RangeMin {
            get { return (double)GetValue(RangeMinProperty); }
            set { SetValue(RangeMinProperty, value); }
        }

        public double RangeMax {
            get { return (double)GetValue(RangeMaxProperty); }
            set { SetValue(RangeMaxProperty, value); }
        }


        public static readonly DependencyProperty MinDifferenceProperty = DependencyProperty.Register("MinDifference", typeof(double), typeof(MyRangeSlider), new PropertyMetadata(0.0));

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(MyRangeSlider), new PropertyMetadata(0.0));

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(MyRangeSlider), new PropertyMetadata(1.0));

        public static readonly DependencyProperty RangeMinProperty = DependencyProperty.Register("RangeMin", typeof(double), typeof(MyRangeSlider), new PropertyMetadata(0.0, OnRangeMinPropertyChanged));

        public static readonly DependencyProperty RangeMaxProperty = DependencyProperty.Register("RangeMax", typeof(double), typeof(MyRangeSlider), new PropertyMetadata(1.0, OnRangeMaxPropertyChanged));

        public MyRangeSlider() {
            this.InitializeComponent();
        }

        private static void OnRangeMinPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var slider = (MyRangeSlider)d;
            double newValue = (double)e.NewValue;

            if(newValue >= (slider.RangeMax - slider.MinDifference+1)) {
                newValue = slider.RangeMax - slider.MinDifference;
            }
            
            if (newValue < slider.Minimum) {
                slider.RangeMin = slider.Minimum;
            }
            else if (newValue > slider.Maximum) {
                slider.RangeMin = slider.Maximum;
            }
            else {
                slider.RangeMin = newValue;
            }

            if (slider.RangeMin > slider.RangeMax) {
                slider.RangeMax = slider.RangeMin;
            }
            

            slider.UpdateMinThumb(slider.RangeMin);
        }

        private static void OnRangeMaxPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var slider = (MyRangeSlider)d;
            double newValue = (double)e.NewValue;

            if (newValue <= (slider.RangeMin + slider.MinDifference)) {
                newValue = slider.RangeMin + slider.MinDifference;
            }

            if (newValue < slider.Minimum) {
                slider.RangeMax = slider.Minimum;
            }
            else if (newValue > slider.Maximum) {
                slider.RangeMax = slider.Maximum;
            }
            else {
                slider.RangeMax = newValue;
            }

            if (slider.RangeMax < slider.RangeMin) {
                slider.RangeMin = slider.RangeMax;
            }

            slider.UpdateMaxThumb(slider.RangeMax);
        }

        private void UpdateMinThumb(double min, bool update = false) {
            if (ContainerCanvas != null) {
                if (update || !MinThumb.IsDragging) {
                    var relativeLeft = ((min - Minimum) / (Maximum - Minimum)) * ContainerCanvas.ActualWidth;

                    Canvas.SetLeft(MinThumb, relativeLeft);
                    Canvas.SetLeft(ActiveRectangle, relativeLeft);

                    ActiveRectangle.Width = (RangeMax - min) / (Maximum - Minimum) * ContainerCanvas.ActualWidth;
                }
            }
        }

        private void UpdateMaxThumb(double max, bool update = false) {
            if (ContainerCanvas != null) {
                if (update || !MaxThumb.IsDragging) {
                    var relativeRight = (max - Minimum) / (Maximum - Minimum) * ContainerCanvas.ActualWidth;

                    Canvas.SetLeft(MaxThumb, relativeRight);

                    ActiveRectangle.Width = (max - RangeMin) / (Maximum - Minimum) * ContainerCanvas.ActualWidth;
                }
            }
        }

        private void ContainerCanvas_SizeChanged(object sender, SizeChangedEventArgs e) {
            var relativeLeft = ((RangeMin - Minimum) / (Maximum - Minimum)) * ContainerCanvas.ActualWidth;
            var relativeRight = (RangeMax - Minimum) / (Maximum - Minimum) * ContainerCanvas.ActualWidth;

            Canvas.SetLeft(MinThumb, relativeLeft);
            Canvas.SetLeft(ActiveRectangle, relativeLeft);
            Canvas.SetLeft(MaxThumb, relativeRight);

            ActiveRectangle.Width = (RangeMax - RangeMin) / (Maximum - Minimum) * ContainerCanvas.ActualWidth;
        }

        private void MinThumb_DragDelta(object sender, DragDeltaEventArgs e) {
            var min = DragThumb(MinThumb, 0, Canvas.GetLeft(MaxThumb), e.HorizontalChange);
            UpdateMinThumb(min, true);
            RangeMin = Math.Round(min);
        }

        private void MaxThumb_DragDelta(object sender, DragDeltaEventArgs e) {
            var max = DragThumb(MaxThumb, Canvas.GetLeft(MinThumb), ContainerCanvas.ActualWidth, e.HorizontalChange);
            UpdateMaxThumb(max, true);
            RangeMax = Math.Round(max);
        }

        private double DragThumb(Thumb thumb, double min, double max, double offset) {
            var currentPos = Canvas.GetLeft(thumb);
            var nextPos = currentPos + offset;

            nextPos = Math.Max(min, nextPos);
            nextPos = Math.Min(max, nextPos);

            return (Minimum + (nextPos / ContainerCanvas.ActualWidth) * (Maximum - Minimum));
        }

        private void MinThumb_DragCompleted(object sender, DragCompletedEventArgs e) {
            UpdateMinThumb(RangeMin);
            Canvas.SetZIndex(MinThumb, 10);
            Canvas.SetZIndex(MaxThumb, 0);
        }

        private void MaxThumb_DragCompleted(object sender, DragCompletedEventArgs e) {
            UpdateMaxThumb(RangeMax);
            Canvas.SetZIndex(MinThumb, 0);
            Canvas.SetZIndex(MaxThumb, 10);
        }
    }
}