using System;
using System.Collections.Generic;
using System.Linq;
using Frame;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Utility;

//关卡事件 记录起因和结果 触发条件和后期结果
namespace Scene
{
    // [Serializable]
    // public enum LevelEventType
    // {
    //     //前往目的地
    //     NewDestination,
    //     //播放动画后前往目的地
    //     PlayAnimationThenNewDestination
    // }
    [Serializable]
    public class LevelEvent
    {
        //名称
        public string eventName;
        //当前关卡类型
        // public LevelEventType eventType;
        //项目处于可交互状态才会交互
        public bool isTriggered;
        
        //item状态对
        public List<ItemStatePair> itemStatePairs;
        
        public UnityEvent onEventTriggered;
        //事件类型NewDestination时才使用
        public List<Vector3> playerDestinations;
        
        //用来判断是否交互，如果交互成功返回true
        public bool CheckEvent()
        {
            if (isTriggered)
            {
                Debug.LogError("Event " + eventName + " is already triggered!");
                return false;
            }
            //判断条件是否达成 若干个相关物体是否在正确的状态
            if (itemStatePairs.Any(itemStatePair => itemStatePair.item.itemState != itemStatePair.state))
            {
                return false;
            }
            //交互过的物体不能互动
            foreach (var itemStatePair in itemStatePairs)
            {
                itemStatePair.item.canInteract = false;
            }
            //这个事件
            isTriggered = true;
            return true;
        }
    }
}
