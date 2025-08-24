using System;
using System.Collections;
using System.Collections.Generic;
using SO;
using UnityEditor;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Frame
{
    public class SceneManager : MonoBehaviour
    {
        private Player.Player _player;
        
        public GameScene menuScene;
        public List<GameScene> gameScenes;
        [SerializeField] private GameScene _currentScene;
        [SerializeField] private GameScene _loadingScene;
        
        private AsyncOperationHandle<SceneInstance> _loadingSceneInstanceAsyncHandle;

        public bool temp;

        private void Update()
        {
            if (temp)
            {
                EventCenter.Broadcast(GameEvent.PreloadSceneDisplayEvent);
                temp = false;
            }
        }

        public void Init(Player.Player player, GameScene menuScene, List<GameScene> gameScenes)
        {
            _player = player;
            this.menuScene = menuScene;
            this.gameScenes = gameScenes;
            EventCenter.Broadcast(GameEvent.ScenePreloadEvent, menuScene);
        }

        private void OnEnable()
        {
            EventCenter.AddListener(GameEvent.SceneUnloadEvent, OnSceneUnload);
            EventCenter.AddListener<GameScene>(GameEvent.ScenePreloadEvent, OnScenePreload);
            EventCenter.AddListener(GameEvent.PreloadSceneDisplayEvent, OnPreloadSceneDisplay);
        }

        private void OnDisable()
        {
            EventCenter.RemoveListener(GameEvent.SceneUnloadEvent, OnSceneUnload);
            EventCenter.RemoveListener<GameScene>(GameEvent.ScenePreloadEvent, OnScenePreload);
            EventCenter.RemoveListener(GameEvent.PreloadSceneDisplayEvent, OnPreloadSceneDisplay);
        }

        private void OnPreloadSceneDisplay()
        {
            StartCoroutine(UnloadThenDisplayPreloadScene());
        }

        private void OnSceneUnload()
        {
            UnloadCurrentScene();
        }

        private void OnScenePreload(GameScene scene)
        {
            AsyncPreloadScene(scene);
        }

        private void UnloadCurrentScene()
        {
            if (!_currentScene) return;
            
            _currentScene.sceneReference.UnLoadScene();
            _currentScene = null;
        }
        
        private IEnumerator UnloadThenDisplayPreloadScene()
        {
            UnloadCurrentScene();

            yield return _loadingSceneInstanceAsyncHandle.Result.ActivateAsync();
            _currentScene = _loadingScene;
            EventCenter.Broadcast(GameEvent.SceneLoadCompletedEvent);
        }

        private void AsyncPreloadScene(GameScene scene)
        {
            _loadingScene = scene;
            _loadingSceneInstanceAsyncHandle = scene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, false);
            _loadingSceneInstanceAsyncHandle.Completed += OnSceneLoadCompleted;
        }

        // private IEnumerator UnloadThenLoadScene(GameScene scene)
        // {
        //     yield return _currentScene.sceneReference.UnLoadScene();
        //     
        //     _currentScene = null;
        //     
        //     AsyncPreloadScene(scene);
        // }
        
        private void OnSceneLoadCompleted(AsyncOperationHandle<SceneInstance> obj)
        {
            // _currentScene = _loadingScene;
            // EventCenter.Broadcast(GameEvent.SceneLoadCompletedEvent);
            if(_loadingScene.sceneType == SceneType.Menu)
                EventCenter.Broadcast(GameEvent.PreloadSceneDisplayEvent);
        }
    }
}