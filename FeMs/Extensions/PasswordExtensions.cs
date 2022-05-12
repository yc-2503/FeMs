using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FeMs.Extensions
{
    public class PasswordExtensions
    {
        public static string GetPassword(DependencyObject obj)
        {
            return (string)obj.GetValue(PasswordProperty);
        }

        public static void SetPassword(DependencyObject obj, string value)
        {
            obj.SetValue(PasswordProperty, value);
        }
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached("Password",typeof(string),typeof(PasswordExtensions),new PropertyMetadata(string.Empty, OnPasswordProperityChanged));
        static void OnPasswordProperityChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var passWord = sender as PasswordBox;
            string password = (string)e.NewValue;

            if (passWord != null && passWord.Password != password)
                passWord.Password = password;
        }
    }
    public class PasswordBehavior : Behavior<PasswordBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PasswordChanged += AssociatedObject_PasswordChanged;
        }

        private void AssociatedObject_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox? passwordBox = sender as PasswordBox;
            if (passwordBox != null)
            {

                string password = PasswordExtensions.GetPassword(passwordBox);

                if (passwordBox != null && passwordBox.Password != password)
                    PasswordExtensions.SetPassword(passwordBox, passwordBox.Password);
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PasswordChanged -= AssociatedObject_PasswordChanged;
        }
    }
}
