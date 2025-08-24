using System.Collections.Generic;
using SO;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Serialization;

namespace Frame
{
    public class Settings : MonoBehaviour
    {
        [Header("Player")]
        public GameObject playerGameObject;
        public float playerIdleTime;
        [Header("Scene")]
        public GameScene menuScene;
        public List<GameScene> gameScenes;
        [Header("Navigation")]
        public NavMeshSurface staticSurface;
        public NavMeshSurface dynamicSurface;
        [Header("Audio")] 
        public AudioSource bgmAudioSource;
        public AudioSource sfxAudioSource;
        public float bgmFadeDuration;
        //TODO:More settings
        
        public static class Time //may use in future
        {
            public const float DAY_TIME = 90;
            public const float DUSK_TIME = 30;
            public const float NIGHT_TIME = 80;
            public const float WHOLE_DAY_TIME = DAY_TIME + DUSK_TIME + NIGHT_TIME;
        }
        
        public static class TagName
        {
            public const string UNTAGGED = "Untagged";
            public const string PLAYER = "Player";
            public const string LEVEL = "Level";
            public const string INTERACTIVE_ITEM = "InteractiveItem";
            public const string LANTERN = "Lantern";
        }
        
        public static class AnimatorParameters //some animations I guess
        {
            public const string IS_WALK = "isWalk";
            public const string IS_READ = "isRead";
            public const string SPEED = "Speed";
            public const string IS_WIND = "IsWind";
            public const string ON_FADE = "OnFade";
            public const string RANDOM_VALUE = "RandomValue";
        }

        public static class Animations
        {
            public const string INTERACTIVE_ITEM_ANIMATION = "InteractiveItemAnimation";
            public const string BOAT_DROP = "BoatDrop";
        }
        
        
        public static class SceneName
        {
            public const string MENU = "Menu";
            public const string SEEK_ROAD = "SeekRoad";
        }
    }
}
