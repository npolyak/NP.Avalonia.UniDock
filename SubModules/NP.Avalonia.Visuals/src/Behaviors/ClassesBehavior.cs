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


        #region InsertClasses Attached Avalonia Property
        public static string GetInsertClasses(AvaloniaObject obj)
        {
            return obj.GetValue(InsertClassesProperty);
        }

        public static void SetInsertClasses(AvaloniaObject obj, string value)
        {
            obj.SetValue(InsertClassesProperty, value);
        }

        public static readonly AttachedProperty<string> InsertClassesProperty =
            AvaloniaProperty.RegisterAttached<object, Control, string>
            (
                "InsertClasses"
            );
        #endregion InsertClasses Attached Avalonia Property


        static ClassesBehavior()
        {
            TheClassesProperty.Changed.Subscribe(OnClassesChanged);

            InsertClassesProperty.Changed.Subscribe(OnInsertClassesChanged);
        }

        private static string[] GetClasses(this string classesStr)
        {
            return classesStr.Split(StrUtils.WHITESPACE_CHARS, StringSplitOptions.RemoveEmptyEntries);
        }

        private static void OnInsertClassesChanged(AvaloniaPropertyChangedEventArgs<string> change)
        {
            string oldClassesStr = change.OldValue.Value;

            IStyledElement sender = change.Sender as IStyledElement;

            if (oldClassesStr != null)
            {
                var oldClasses = oldClassesStr.GetClasses();
                sender.Classes.RemoveAll(oldClasses);
            }

            string newClassesStr = change.NewValue.Value;

            if (newClassesStr != null)
            {
                var newClasses = newClassesStr.GetClasses();
                sender.Classes.InsertRange(0, newClasses);
            }
        }

        private static void OnClassesChanged(AvaloniaPropertyChangedEventArgs<string> change)
        {
            IStyledElement sender = change.Sender as IStyledElement;

            string classesStr = change.NewValue.Value;

            if (classesStr != null)
            {
                var classes = classesStr.GetClasses();

                if (classes != null)
                {
                    sender.Classes = new Classes(classes);
                    return;
                }
            }

            sender.Classes = new Classes();
        }
    }
}
