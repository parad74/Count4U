using System;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Threading;
using System.Windows.Data;
using Xceed.Wpf.Toolkit;

namespace Count4U.Common.Behaviours
{
    public class MaskedTextChangedDelayedBehavior : Behavior<MaskedTextBox>
    {
        private DispatcherTimer _timer;

        protected override void OnAttached()
        {
            base.OnAttached();

            MaskedTextBox textBox = this.AssociatedObject as MaskedTextBox;            
            this._timer = new DispatcherTimer();
            this._timer.Tick += timer_Tick;
            this._timer.Interval = TimeSpan.FromSeconds(1);
            textBox.TextChanged += textBox_TextChanged;
            textBox.Unloaded += textBox_Unloaded;
        }

        void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_timer.IsEnabled)
                this._timer.Start();
        }
     

        void textBox_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
         //   Cleanup();
        }        

        void timer_Tick(object sender, System.EventArgs e)
        {
            this._timer.Stop();

            MaskedTextBox textBox = this.AssociatedObject as MaskedTextBox;
            BindingExpression expression = textBox.GetBindingExpression(MaskedTextBox.ValueProperty);
            expression.UpdateSource();
        }

        private bool _isCleanedUp;

        private void Cleanup()
        {
            if (!_isCleanedUp)
            {
                this._isCleanedUp = true;

                MaskedTextBox textBox = this.AssociatedObject as MaskedTextBox;
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