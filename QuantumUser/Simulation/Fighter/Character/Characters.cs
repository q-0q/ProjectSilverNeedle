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
            
            // add characters here
        }

        private static Dictionary<CharacterEnum, Character> _dictionary = new()
        {
            { CharacterEnum.Stick, new Stick() },
            { CharacterEnum.StickTwo, new StickTwo() },
            { CharacterEnum.Victor, new Victor() },
            
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
    }
    
}