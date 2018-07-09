﻿using System;
using System.Reflection;
using UnityEditor;

namespace Framework
{
    public static class ReflectionExtensions
    {
        public static Type GetUnderlyingType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                default:
                    throw new ArgumentException
                    (
                        "Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"
                    );
            }
        }

        public delegate void PropertyCallback(SerializedProperty property);

        public static void ForeachProperty(this SerializedObject self, PropertyCallback callback)
        {
            SerializedProperty iterator = self.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                callback(iterator);
                enterChildren = false;
            }
        }
    }
}