using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vc
{
    namespace VehicleComponent
    {
        public class RevLimiter
        {
            public float ThrottleFactor { get; private set; }
            bool isRevLimiterActive = false;
            float RevLimiterActiveTime = 0f;
            float RevLimiterActiveTimeMax;

            public RevLimiter(float limterDuration = 0.02f)
            {
                RevLimiterActiveTimeMax = limterDuration;
                ThrottleFactor = 1f;
            }

            public void Step(float dt, IEngineRPM engine)
            {
                if (!isRevLimiterActive && engine.CurrentRPM >= engine.RedlineRPM)
                {
                    ToggleRevLimiter(true);

                }
                else if (isRevLimiterActive)
                {
                    RevLimiterActiveTime += dt;
                    if (RevLimiterActiveTime > RevLimiterActiveTimeMax)
                    {
                        ToggleRevLimiter(false);
                    }
                }
            }

            void ToggleRevLimiter(bool activate)
            {
                if (activate)
                {
                    isRevLimiterActive = true;
                    RevLimiterActiveTime = 0f;
                    ThrottleFactor = 0f;
                }
                else if (!activate)
                {
                    isRevLimiterActive = false;
                    RevLimiterActiveTime = 0f;
                    ThrottleFactor = 1f;
                }
            }

        }
    }
}