using System;
using System.Collections.Generic;
using Utility;

namespace Scene
{
    [Serializable]
    public class LightEvent
    {
        public string eventName;
        public List<LightIntensityPair> targetLightIntensityPairs;
        public float fadeTime = 5f;
    }
}