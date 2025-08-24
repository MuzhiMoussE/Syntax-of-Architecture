using System;
using UnityEngine;

namespace Utility
{
    [Serializable]
    public class LightIntensityPair
    {
        public Light light;
        public float intensity;

        public LightIntensityPair(Light light, float intensity)
        {
            this.light = light;
            this.intensity = intensity;
        }
    }
}