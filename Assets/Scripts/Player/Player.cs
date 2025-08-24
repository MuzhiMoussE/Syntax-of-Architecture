using System.Collections.Generic;
using Scene;
using Frame;
using SO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Player
{
    public enum PlayerState
    {
        Idle,
        Moving,
        Interacting
    }
    
    [RequireComponent(typeof(NavMeshAgent))]
    public class Player : MonoBehaviour
    {
        [SerializeField] private GameObject wind;
        
        private NavMeshAgent _agent;
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;

        public PlayerState playerState;
        
        public List<Vector3> destinations;
        private float _idleTime;
        private float _stopCounter;

        public bool isPlaying;
        private bool _hasTriggeredWind;
        private bool _hasTriggeredTitle;

        public float runSpeed = 6f;
        public float walkSpeed = 3f;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _agent = GetComponent<NavMeshAgent>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _agent.destination = transform.position;
            _agent.updateRotation = false;
            playerState = PlayerState.Idle;
            _stopCounter = 0;
            isPlaying = false;
            _hasTriggeredTitle = false;
            _hasTriggeredWind = false;
            _agent.speed = walkSpeed;
        }

        public void Init(GameObject settingsPlayerGameObject, float settingsIdleTime)
        {
            settingsPlayerGameObject.tag = Settings.TagName.PLAYER;
            _idleTime = settingsIdleTime;
        }
        
        private void OnEnable()
        {
            //当收到广播，触发函数
            EventCenter.AddListener(GameEvent.SceneLoadCompletedEvent, OnSceneLoadCompleted);
            EventCenter.AddListener<LevelEvent>(GameEvent.TriggerLevelEvent, OnLevelEventTriggered);
            EventCenter.AddListener(GameEvent.AfterTitleEvent, OnAfterTitle);
            EventCenter.AddListener<GameScene>(GameEvent.ScenePreloadEvent, OnScenePreload);
        }
        
        private void OnDisable()
        {
            EventCenter.RemoveListener(GameEvent.SceneLoadCompletedEvent, OnSceneLoadCompleted);
            EventCenter.RemoveListener<LevelEvent>(GameEvent.TriggerLevelEvent, OnLevelEventTriggered);
            EventCenter.RemoveListener(GameEvent.AfterTitleEvent, OnAfterTitle);
            EventCenter.RemoveListener<GameScene>(GameEvent.ScenePreloadEvent, OnScenePreload);
        }

        //temp
        private void OnScenePreload(GameScene preloadScene)
        {
            if (preloadScene.sceneType == SceneType.Level)
                _spriteRenderer.enabled = true;
        }

        private void OnAfterTitle()
        {
            _animator.SetBool(Settings.AnimatorParameters.IS_WIND, false);
        }
        
        private void OnSceneLoadCompleted()
        {
            var levels = GameObject.FindGameObjectsWithTag(Settings.TagName.LEVEL);

            if (levels.Length <= 0) return;
            
            destinations = levels[0].GetComponent<Level>().initPlayerDestinations;
            if (levels[0].GetComponent<Level>().levelName == Settings.SceneName.SEEK_ROAD)
            {
                isPlaying = false;
                _animator.SetBool(Settings.AnimatorParameters.IS_WIND, true);
            }
            else isPlaying = true;
        }

        private void OnLevelEventTriggered(LevelEvent levelEvent)
        {
            destinations = levelEvent.playerDestinations;
            
            playerState = PlayerState.Moving;
            _stopCounter = 0;
            SetDestination(destinations[Random.Range(0, destinations.Count)]);
        }

        private void Update()
        {
            if (!isPlaying) return;
            
            _animator.SetFloat(Settings.AnimatorParameters.SPEED, _agent.velocity.magnitude);
            // Debug.Log(_agent.velocity.magnitude);
            var dir = _agent.velocity.normalized;
            if(dir.x>0) transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            else if(dir.x<0) transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            if (playerState != PlayerState.Idle && Vector3.Distance(_agent.destination, transform.position) < 2f)
            {
                playerState = PlayerState.Idle;
            }
            
            if (playerState == PlayerState.Idle)
            {
                _stopCounter += Time.deltaTime;
                
                if (_stopCounter > _idleTime)
                {
                    playerState = PlayerState.Moving;
                    _stopCounter -= _idleTime;
                    SetDestination(destinations[Random.Range(0, destinations.Count)]);
                }
            }
        }
        
        public void SetDestination(Vector3 destination)
        {
            _agent.destination = destination;
        }

        public void OnWindBegin()
        {
            if (_hasTriggeredWind) return;
            
            _hasTriggeredWind = true;
            GlobalInstance.InputManager().canInput = false;
        }
        
        public void AfterOnWind()
        {
            if (_hasTriggeredTitle) return;
            
            EventCenter.Broadcast(GameEvent.TitleFadeEvent);
            _hasTriggeredTitle = true;
        }

        public void WindEndBegin()
        {
            wind.SetActive(true);
        }
        
        public void AfterWindEnd()
        {
            isPlaying = true;
        }

        public void ChangeMoveState(bool isRun)
        {
            if (isRun) _agent.speed = runSpeed;
            else _agent.speed = walkSpeed;
        }
    }
}
