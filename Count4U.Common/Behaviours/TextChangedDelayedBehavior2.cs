using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Threading;

namespace Count4U.Common.Behaviours
{
    public class TextChangedDelayedBehavior2 : Behavior<TextBox>
    {
        private readonly DispatcherTimer _timer;

        private bool _skipUpdate;

        public static readonly DependencyProperty BehaviorTextProperty =
            DependencyProperty.Register("BehaviorText", typeof(string), typeof(TextChangedDelayedBehavior2), new PropertyMetadata(OnTextPropertyChanged));

        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var b = (TextChangedDelayedBehavior2)d;

            b.OnTextChanged(e.OldValue as string, e.NewValue as string);
        }

        public string BehaviorText
        {
            get { return (string)GetValue(BehaviorTextProperty); }
            set { SetValue(BehaviorTextProperty, value); }
        }

        public TextChangedDelayedBehavior2()
        {
            this._timer = new DispatcherTimer();
            this._timer.Tick += Timer_Tick;
            this._timer.Interval = TimeSpan.FromMilliseconds(200);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

			if (this.AssociatedObject != null) {
				AssociatedObject.TextChanged += TextBox_TextChanged;
				AssociatedObject.Unloaded += AssociatedObject_Unloaded;
			}
        }

        void AssociatedObject_Unloaded(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            Clear();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_skipUpdate)
                return;

            if (_timer.IsEnabled)
            {
                _timer.Stop();
            }

            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();

            _skipUpdate = true;
            BehaviorText = AssociatedObject.Text;
            _skipUpdate = false;
        }


        private void OnTextChanged(string oldValue, string newValue)
        {
            if (_skipUpdate)
                return;

            _skipUpdate = true;
            AssociatedObject.Text = newValue;
            _skipUpdate = false;
        }

        private void Clear()
        {
            _timer.Stop();
			if (this.AssociatedObject != null) {
				AssociatedObject.TextChanged -= TextBox_TextChanged;
				AssociatedObject.Unloaded -= AssociatedObject_Unloaded;
			}
        }
    }
}