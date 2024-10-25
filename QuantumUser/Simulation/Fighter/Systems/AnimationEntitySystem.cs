using System.Collections.Generic;
using Photon.Deterministic;
using Quantum.Types;
using UnityEngine;

namespace Quantum
{
    public unsafe class AnimationEntitySystem : SystemMainThreadFilter<AnimationEntitySystem.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public AnimationEntityData* AnimationEntityData;

        }

        public static void Create(Frame f, AnimationEntities.AnimationEntityEnum type, FPVector2 location, FP angle, bool flip)
        {

            var prototype = AnimationEntities.Get(type).UI
                ? f.FindAsset<EntityPrototype>("QuantumUser/Resources/UIAnimationEntityEntityPrototype")
                : f.FindAsset<EntityPrototype>("QuantumUser/Resources/AnimationEntityEntityPrototype");
            
            EntityRef entity = f.Create(prototype);
            
            f.Unsafe.TryGetPointer<AnimationEntityData>(entity, out var animationEntityData);
            animationEntityData->type = (int)type;
            animationEntityData->framesAlive = 0;
            animationEntityData->spriteId = AnimationEntities.Get(type).SpriteSheetOffset;
            animationEntityData->flip = flip;
            animationEntityData->angle = angle;
            animationEntityData->layer = AnimationEntities.Get(type).Layer;
            
            f.Unsafe.TryGetPointer<Transform3D>(entity, out var transform);
            transform->Position = location.XYO;
            transform->Position.Z = 0;
        }
        
        public override void Update(Frame f, ref Filter filter)
        {
            
            // if (HitstopSystem.IsHitstopActive(f)) return;
            
            var animationEntity = AnimationEntities.Get((AnimationEntities.AnimationEntityEnum)filter.AnimationEntityData->type);
            filter.AnimationEntityData->spriteId =
                animationEntity.SectionGroup.GetItemFromIndex(filter.AnimationEntityData->framesAlive)
                + animationEntity.SpriteSheetOffset;

            filter.AnimationEntityData->framesAlive++;
            if (filter.AnimationEntityData->framesAlive > animationEntity.SectionGroup.Duration())
            {
                f.Destroy(filter.Entity);
            }
        }
    }
}