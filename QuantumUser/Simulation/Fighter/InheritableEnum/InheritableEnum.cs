using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace Quantum.InheritableEnum
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Collections.Generic;

    public abstract class InheritableEnum
    {
        // Static dictionary to track the next available value for the entire hierarchy
        private static int currentValue = 0; // Tracks the overall count for all states in the hierarchy

        static InheritableEnum()
        {
            Debug.Log("Hello from InheritableEnum ctor");
            
            // Get all subclasses of InheritableEnum (excluding abstract classes)
            var subclasses = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(InheritableEnum)) && !t.IsAbstract)
                .ToList();

            // Sort subclasses (optional, here just sorting alphabetically)
            subclasses.Sort((t1, t2) => string.Compare(t1.Name, t2.Name, StringComparison.Ordinal));

            foreach (var subclass in subclasses)
            {
                // Find the fields (states) in the current subclass
                var fields = subclass.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(f => f.FieldType == typeof(int))
                    .ToList();

                foreach (var field in fields)
                {
                    // Assign the current value to each state field and increment the global counter
                    field.SetValue(null, currentValue);
                    Debug.Log(field.Name + ": " + field);
                    currentValue++;
                }
            }
        }
        
        // Static method to ensure initialization is triggered
        public static void Initialize()
        {
            // This will force the static constructor of BaseState to run
            var _ = PlayerFSM.State.GroundActionable;  // Trigger initialization by accessing a static member
        }
        
        public static string GetFieldNameByValue(int value, Type subclassType)
        {
            // Get all the fields of the given subclass type
            var fields = subclassType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(f => f.FieldType == typeof(int))
                .ToList();

            // Search through fields to find the one that matches the value
            foreach (var field in fields)
            {
                int fieldValue = (int)field.GetValue(null);
                if (fieldValue == value)
                {
                    return field.Name;
                }
            }

            return null;  // Return null if no field matches the value
        }
    }


}