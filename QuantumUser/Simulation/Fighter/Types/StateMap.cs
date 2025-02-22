using System;
using System.Collections.Generic;

namespace Quantum.Types
{
    public class StateMap<T>
    {
        public T DefaultValue;
        public Dictionary<int, T> Dictionary;
        public Dictionary<int, T> SuperDictionary;
        public Dictionary<int, Func<FrameParam, T>> FuncDictionary;
        public Dictionary<int, Func<FrameParam, T>> SuperFuncDictionary;
        

        public StateMap()
        {
            Dictionary = new Dictionary<int, T>();
            SuperDictionary = new Dictionary<int, T>();
            FuncDictionary = new Dictionary<int, Func<FrameParam, T>>();
            SuperFuncDictionary = new Dictionary<int, Func<FrameParam, T>>();
        }
        
        public T Get(PlayerFSM fsm, FrameParam frameParam = null)
        {
            int state = fsm.Fsm.State();
            return Lookup(state, fsm, frameParam);
        }

        public T Lookup(int state, PlayerFSM fsm, FrameParam frameParam = null)
        {
            
            if (FuncDictionary.ContainsKey(state))
            {
                return FuncDictionary[state](frameParam);
            }

            if (Dictionary.ContainsKey(state))
            {
                return Dictionary[state];
            }
            
            foreach (var (key, value) in SuperFuncDictionary)
            {
                if (fsm.Fsm.IsInState(key))
                {
                    return value(frameParam);
                }
            }
            
            foreach (var (key, value) in SuperDictionary)
            {
                if (fsm.Fsm.IsInState(key))
                {
                    return value;
                }
            }
            
            return DefaultValue;
        }
    }
}