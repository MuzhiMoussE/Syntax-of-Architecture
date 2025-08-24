using System.Collections.Generic;
using Scene;
using UnityEditor;
using UnityEngine;
using Utility;

namespace Frame
{
    public class GlobalInstance : SingletonMonoBase<GlobalInstance>
    {
        private Player.Player _player;
        private InputManager _inputManager;
        private SceneManager _sceneManager;
        private NavigationManager _navigationManager;
        private AudioManager _audioManager;
        //TODO:render
        protected override void Awake()
        {
            base.Awake();

            var settings = GetComponent<Settings>();
            if (settings == null)
                return;
            
            InitPlayer(settings);
            CreateAndInitInputManager(settings);
            CreateAndInitSceneManager(settings);
            CreateAndInitNavigationManager(settings);
            CreateAndInitAudioManager(settings);
            //TODO:more settings
            
            Destroy(settings);
        }


        private void InitPlayer(Settings settings)
        {
            _player = settings.playerGameObject.GetComponent<Player.Player>();
            _player.Init(settings.playerGameObject, settings.playerIdleTime);
        }
        
        private void CreateAndInitInputManager(Settings settings)
        {
            _inputManager = gameObject.AddComponent<InputManager>();
            _inputManager.Init(_player);
        }
        
        private void CreateAndInitSceneManager(Settings settings)
        {
            _sceneManager = gameObject.AddComponent<SceneManager>();
            _sceneManager.Init(_player, settings.menuScene, settings.gameScenes);
        }
        
        private void CreateAndInitNavigationManager(Settings settings)
        {
            _navigationManager = gameObject.AddComponent<NavigationManager>();
            _navigationManager.Init(settings.staticSurface, settings.dynamicSurface);
        }
        
        private void CreateAndInitAudioManager(Settings settings)
        {
            _audioManager = gameObject.AddComponent<AudioManager>();
            _audioManager.Init(settings.bgmAudioSource, settings.sfxAudioSource, settings.bgmFadeDuration);
        }

        public static Player.Player Player()
        {
            return Instance ? Instance._player : null;
        }
        
        public static InputManager InputManager()
        {
            return Instance ? Instance._inputManager : null;
        }
        
        public static SceneManager SceneManager()
        {
            return Instance ? Instance._sceneManager : null;
        }

        public static NavigationManager NavigationManager()
        {
            return Instance ? Instance._navigationManager : null;
        }
        
        public static AudioManager AudioManager()
        {
            return Instance ? Instance._audioManager : null;
        }
    }
}