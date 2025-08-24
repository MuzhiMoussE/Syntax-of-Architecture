using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Frame;
using UnityEngine;
using UnityEngine.Serialization;
//关卡类，保存关卡信息
namespace Scene
{
    public class Level : MonoBehaviour
    {
        public string levelName;
        //存储可交互物品
        public List<InteractiveItem> interactiveItems;
        //大关卡中有多少个小关卡，对应的关卡事件
        public List<LevelEvent> levelEvents;
        //初始化目的地
        public List<Vector3> initPlayerDestinations;
        //下一个事件索引
        private int _nextEventIndex;
        
        [SerializeField] private List<AudioClip> bgmClips;
        [SerializeField] private float initBGMVolume = 1f;
        private int _nextBGMIndex;

        public void PlayNextBGM(float targetVolume)
        {
            if (_nextBGMIndex >= bgmClips.Count) return;
            
            GlobalInstance.AudioManager().PlayBGM(bgmClips[_nextBGMIndex++], targetVolume);
        }
        
        private void Awake()
        {
            _nextEventIndex = 0;
            _nextBGMIndex = 0;
            PlayNextBGM(initBGMVolume);
        }

        //接受广播
        private void OnEnable()
        {
            EventCenter.AddListener(GameEvent.ItemStateChangeEvent, OnItemStateChanged);
        }
        
        private void OnDisable()
        {
            EventCenter.RemoveListener(GameEvent.ItemStateChangeEvent, OnItemStateChanged);
        }
        //项目状态更改
        private void OnItemStateChanged()
        {
            //判断事件是否启动
            if (levelEvents[_nextEventIndex].CheckEvent())
            {
                //执行事件后果
                EventCenter.Broadcast(GameEvent.TriggerLevelEvent, levelEvents[_nextEventIndex]);
                levelEvents[_nextEventIndex].onEventTriggered?.Invoke();
                _nextEventIndex++;
                EventCenter.Broadcast(GameEvent.ItemStateChangeEvent);
            }
        }

        public void ChangePlayerMoveState(bool isRun)
        {
            GlobalInstance.Player().ChangeMoveState(isRun);
        }
    }
}
