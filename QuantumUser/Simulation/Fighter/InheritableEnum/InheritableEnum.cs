using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Quantum.InheritableEnum
{
    public abstract class InheritableEnum
    {
        // Static dictionary to track the "next value" for each hierarchy
        private static readonly Dictionary<Type, int> hierarchyCounters = new();

        static InheritableEnum()
        {
            // Get all subclasses of BaseState (excluding abstract classes)
            var subclasses = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(InheritableEnum)) && !t.IsAbstract)
                .ToList();

            // Sort subclasses (optional, here just sorting alphabetically)
            subclasses.Sort((t1, t2) => string.Compare(t1.Name, t2.Name, StringComparison.Ordinal));

            foreach (var subclass in subclasses)
            {
                // Find the root type (the highest class in the hierarchy chain)
                var rootType = FindRootType(subclass);

                // Get the current "counter" for this hierarchy, or initialize it if it doesn't exist
                if (!hierarchyCounters.ContainsKey(rootType))
                {
                    hierarchyCounters[rootType] = 0;
                }

                // Set the integer value for this class (based on the hierarchy)
                var field = subclass.GetField("Value", BindingFlags.Public | BindingFlags.Static);
                if (field != null)
                {
                    field.SetValue(null, hierarchyCounters[rootType]);
                    hierarchyCounters[rootType]++;  // Increment for the next subclass in this hierarchy
                }
            }
        }

        // Static field instead of a property
        public static int Value;
    
        // Find the root class of the hierarchy (i.e., the class that isn't a subclass of anything else)
        private static Type FindRootType(Type subclass)
        {
            var baseType = subclass.BaseType;
            while (baseType != null && baseType != typeof(InheritableEnum))
            {
                subclass = baseType;
                baseType = subclass.BaseType;
            }
            return subclass; // The root type of the hierarchy
        }
    }

}