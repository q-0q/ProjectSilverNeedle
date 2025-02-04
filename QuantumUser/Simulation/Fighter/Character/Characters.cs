using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using Photon.Deterministic;
using Debug = UnityEngine.Debug;

namespace Quantum
{
    public static unsafe class Characters
    {
        public enum CharacterEnum
        {
            Stick,
            StickTwo,
            Victor,
            Blenderman
            
            // add characters here
        }

        private static Dictionary<CharacterEnum, Character> _dictionary = new()
        {
            
            { CharacterEnum.StickTwo, new StickTwo() },
            
            // add characters here
        };

        public static Character Get(CharacterEnum e)
        {
            return _dictionary[e];
        } 
        
        public static Character GetPlayerCharacter(Frame f, EntityRef entityRef)
        {
            f.Unsafe.TryGetPointer<PlayerLink>(entityRef, out var playerLink);
            var e = (CharacterEnum)playerLink->characterId;
            return Get(e);
        }

        static Characters()
        {
            
        }
    }
    
}