using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Threading;
using System.Windows.Data;

namespace Count4U.Common.Behaviours
{
    public class TextChangedDelayedBehavior : Behavior<TextBox>
    {
        private DispatcherTimer _timer;

        public static readonly DependencyProperty IsTimerEnabledProperty =
            DependencyProperty.Register("IsTimerEnabled", typeof(bool), typeof(TextChangedDelayedBehavior), new PropertyMetadata(false));

        public bool IsTimerEnabled
        {
            get { return (bool)GetValue(IsTimerEnabledProperty); }
            set { SetValue(IsTimerEnabledProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            TextBox textBox = this.AssociatedObject as TextBox;
            this._timer = new DispatcherTimer();
            this._timer.Tick += timer_Tick;
            this._timer.Interval = TimeSpan.FromMilliseconds(200);
            textBox.TextChanged += textBox_TextChanged;
            textBox.Unloaded += textBox_Unloaded;
        }

        void textBox_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
         //   Cleanup();
        }

        void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsTimerEnabled) return;

            if (_timer.IsEnabled)
                _timer.Stop();

            this._timer.Start();
        }
        
        void timer_Tick(object sender, System.EventArgs e)
        {
            this._timer.Stop();

            TextBox textBox = this.AssociatedObject as TextBox;            
            BindingExpression expression = textBox.GetBindingExpression(TextBox.TextProperty);                         
            expression.UpdateSource();            
        }

        private bool _isCleanedUp;

        private void Cleanup()
        {
            if (!_isCleanedUp)
            {
                this._isCleanedUp = true;

                TextBox textBox = this.AssociatedObject as TextBox;
                textBox.TextChanged -= textBox_TextChanged;
                textBox.Unloaded -= textBox_Unloaded;

                this._timer.Stop();
                this._timer.Tick -= timer_Tick;
                this._timer = null;
            }
        }

        protected override void OnDetaching()
        {
            Cleanup();

            base.OnDetaching();
        }
    }
}