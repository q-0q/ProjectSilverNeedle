using System;
using System.Collections.Generic;

namespace Quantum.Types
{
    public class StateMap<T>
    {
        public T DefaultValue;
        public Dictionary<int, T> Dictionary;
        public Dictionary<int, T> SuperDictionary;
        public Dictionary<int, Func<T>> FuncDictionary;

        public StateMap()
        {
            Dictionary = new Dictionary<int, T>();
            SuperDictionary = new Dictionary<int, T>();
            FuncDictionary = new Dictionary<int, Func<T>>();
        }
        
        public T Get(PlayerFSM fsm)
        {
            int state = fsm.Fsm.State();
            
            if (FuncDictionary.ContainsKey(state))
            {
                return FuncDictionary[state]();
            }

            foreach (var (key, value) in SuperDictionary)
            {
                if (fsm.Fsm.IsInState(key))
                {
                    return value;
                }
            }
            
            if (Dictionary.ContainsKey(state))
            {
                return Dictionary[state];
            }

            return DefaultValue;
        }
    }
}