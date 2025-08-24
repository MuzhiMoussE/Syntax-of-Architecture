using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Scene
{
    public class LightManager : MonoBehaviour
    {
        private List<LightIntensityPair> _allLightPairs;
        public float initialFadeInTime = 2f;
        
        public List<LightEvent> lightEvents;
        private int _nextLightEventIndex = 0;
        
        private void Start()
        {
            _allLightPairs = new List<LightIntensityPair>();
            
            var allLights = GetComponentsInChildren<Light>();
            
            foreach (var light in allLights)
            {
                _allLightPairs.Add(new LightIntensityPair(light, light.intensity));
            }

            foreach (var light in allLights)
            {
                light.intensity = 0;
            }
            StartCoroutine(AllLightsFadeIn());
        }

        private IEnumerator AllLightsFadeIn()
        {
            var time = 0f;
            while(time < initialFadeInTime)
            {
                time += Time.deltaTime;
                foreach (var lightIntensityPair in _allLightPairs)
                {
                    lightIntensityPair.light.intensity = Mathf.Lerp(0, lightIntensityPair.intensity, time / initialFadeInTime);
                }
                yield return null;
            }
        }

        public void NextLightEvent()
        {
            foreach (var lightIntensityPairs in lightEvents[_nextLightEventIndex].targetLightIntensityPairs)
            {
                StartCoroutine(LightFade(lightIntensityPairs.light, lightIntensityPairs.light.intensity,
                    lightIntensityPairs.intensity, lightEvents[_nextLightEventIndex].fadeTime));
            }
            ++_nextLightEventIndex;
        }
        
        private static IEnumerator LightFade(Light light, float startIntensity, float targetIntensity, float time)
        {
            float timeCount = 0;
            while (timeCount < time)
            {
                timeCount += Time.deltaTime;
                light.intensity = Mathf.Lerp(startIntensity, targetIntensity, timeCount / time);
                yield return null;
            }
        }
    }
}