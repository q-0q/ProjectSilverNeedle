using System;
using System.Collections.Generic;
using Photon.Deterministic;
using UnityEngine;

namespace Quantum.Types
{
    public class SectionGroup<T>
    {
        public bool Loop = false;
        public int LengthScalar = 1;
        public bool AutoFromAnimationPath = false;
        public List<Tuple<int, T>> Sections;


        public T GetCurrentItem(Frame f, PlayerFSM fsm)
        {
            int frames = fsm.FramesInCurrentState(f);
            return GetItemFromIndex(frames);
        }
        
        public T GetCurrentItem(Frame f, EntityRef entityRef)
        {
            int frames = PlayerFSM.FramesInCurrentState(f, entityRef);
            return GetItemFromIndex(frames);
        }
        
        public int GetCurrentItemDuration(Frame f, PlayerFSM fsm)
        {
            int frames = fsm.FramesInCurrentState(f);
            return GetItemDuration(frames);
        }

        public T GetItemFromPercentage(FP percentage)
        {
            int index = (int)(percentage * Duration());
            return GetItemFromIndex(index);
        }
        
        public T GetItemFromIndex(int index)
        {
            int duration = Duration();
            if (Loop) index %= duration;

            int counter = 0;
            for (int i = 0; i < Sections.Count; i++)
            {
                for (int j = counter; j < GetSectionDuration(i) + counter; j++)
                {
                    if (j == index)
                    {
                        return Sections[i].Item2;
                    }
                }

                counter += GetSectionDuration(i);
            }

            return Sections[^1].Item2;
        }
        
        public int GetItemDuration(int index)
        {
            int duration = Duration();
            if (Loop) index %= duration;

            int counter = 0;
            for (int i = 0; i < Sections.Count; i++)
            {
                for (int j = counter; j < GetSectionDuration(i) + counter; j++)
                {
                    if (j == index)
                    {
                        return Sections[i].Item1;
                    }
                }

                counter += GetSectionDuration(i);
            }

            return Sections[^1].Item1;
        }

        public int GetCurrentFirstFrame(Frame f, PlayerFSM fsm)
        {
            int frames = fsm.FramesInCurrentState(f);
            return GetFirstFrameFromIndex(frames);
        }
        
        public int GetFirstFrameFromIndex(int index)
        {
            int duration = Duration();
            if (Loop) index %= duration;

            int counter = 0;
            for (int i = 0; i < Sections.Count; i++)
            {
                for (int j = counter; j < GetSectionDuration(i) + counter; j++)
                { 
                    if (j == index)
                    {
                        return counter;
                    }
                }

                counter += GetSectionDuration(i);
            }

            return counter - (GetSectionDuration(Sections.Count - 1));
        }
        
        

        public int Duration()
        {
            int duration = 0;
            for (int i = 0; i < Sections.Count; i++)
            {
                duration += GetSectionDuration(i);
            }

            return duration;
        }

        private int GetSectionDuration(int i)
        {
            return Sections[i].Item1 * LengthScalar;
        }

        public bool IsOnFirstFrameOfSection(Frame f, PlayerFSM fsm)
        {
            int index = fsm.FramesInCurrentState(f);   
            int duration = Duration();
            if (Loop) index %= duration;

            int counter = 0;
            for (int i = 0; i < Sections.Count; i++)
            {
                for (int j = counter; j < GetSectionDuration(i) + counter; j++)
                {
                    if (j == index)
                    {
                        return j == counter;
                    }
                }

                counter += GetSectionDuration(i);
            }

            return false;
        }

        
    }
    
}