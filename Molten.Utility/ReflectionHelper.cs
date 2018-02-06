﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Molten
{
    public static class ReflectionHelper
    {
        /// <summary>Finds all types that derive from the provided class type.</summary>
        /// <typeparam name="T">The base type of other classes to search for.</typeparam>
        /// <returns></returns>
        public static List<Type> FindType<T>()
        {
            Type bType = typeof(T);
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            List<Type> result = new List<Type>();
            foreach (Assembly assembly in assemblies)
            {
                IEnumerable<Type> types = assembly.GetTypes().Where(t => t.IsSubclassOf(bType));
                result.AddRange(types);
            }

            return result;
        }

        public static IEnumerable<Type> FindType<T>(Assembly assembly)
        {
            Type bType = typeof(T);

            return assembly.GetTypes().Where(t => t.IsSubclassOf(bType));
        }

        /// <summary>Gets the name of a type. Includes its namespace and name only.</summary>
        /// <param name="type">The type of which to retrieve the name.</param>
        /// <returns></returns>
        public static string GetTypeName(Type type)
        {
            string typeName = "";

            typeName = type.Namespace + "." + type.Name;
            if (type.IsGenericType)
            {
                typeName += "<";
                Type[] generics = type.GetGenericArguments();
                for (int i = 0; i < generics.Length; i++)
                {
                    if (i > 0)
                        typeName += ", " + GetTypeName(generics[i]);
                    else
                        typeName += GetTypeName(generics[i]);
                }

                typeName += ">";
            }

            return typeName;
        }

        /// <summary>Produces a short string that can be used to retrieve the type via Type.GetType.</summary>
        /// <param name="type">The type to create the string for.</param>
        /// <returns></returns>
        public static string GetQualifiedTypeName(Type type)
        {
            string[] delims = new string[1] { ", " };

            return type + ", " + type.Assembly.FullName.Split(delims, StringSplitOptions.RemoveEmptyEntries)[0];
        }
    }
}