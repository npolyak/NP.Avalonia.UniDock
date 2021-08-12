// (c) Nick Polyak 2021 - http://awebpros.com/
// License: MIT License (https://opensource.org/licenses/MIT)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.
//
using Avalonia;
using Avalonia.Controls;
using System.Collections.Generic;
using System;
using NP.Utilities;

namespace NP.Avalonia.Visuals.Behaviors
{
    public static class ClassesBehavior
    {
        #region TheClasses Attached Avalonia Property
        public static string GetTheClasses(AvaloniaObject obj)
        {
            return obj.GetValue(TheClassesProperty);
        }

        public static void SetTheClasses(AvaloniaObject obj, string value)
        {
            obj.SetValue(TheClassesProperty, value);
        }

        public static readonly AttachedProperty<string> TheClassesProperty =
            AvaloniaProperty.RegisterAttached<object, StyledElement, string>
            (
                "TheClasses"
            );
        #endregion TheClasses Attached Avalonia Property

        static ClassesBehavior()
        {
            TheClassesProperty.Changed.Subscribe(OnClassesChanged);
        }

        private static void OnClassesChanged(AvaloniaPropertyChangedEventArgs<string> change)
        {
            IStyledElement sender = change.Sender as IStyledElement;

            string classesStr = change.NewValue.Value;

            if (classesStr != null)
            {
                var classes = classesStr.Split(StrUtils.WHITESPACE_CHARS, StringSplitOptions.RemoveEmptyEntries);
                sender.Classes = new Classes(classes);
            }
            else
            {
                sender.Classes = new Classes();
            }
        }
    }
}
