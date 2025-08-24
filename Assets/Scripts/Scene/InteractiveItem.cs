using System;
using Frame;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scene
{
    [Serializable]
    public enum ItemState
    {
        //闲置
        Idle,
        //互动中
        Interacting,
        //互动完成
        Interacted,
        //特殊
        SpecialState,
        /***For multi-state items***/
        State1,//4
        
        State2,//5
        
        State3,//6
        /***For multi-state items***/
    }
    [Serializable]
    public abstract class InteractiveItem : MonoBehaviour
    {
        public static int MultiStateOffset = 3;
        public string itemName;
        
        public ItemState itemState = ItemState.Idle;

        public bool canInteract = true;
        [Header("Universal Audio")]
        public AudioClip idleAudioClip;
        [Header("Single-State Audio")]
        public AudioClip interactedAudioClip;
        public AudioClip interactingAudioClip;
        public AudioClip specialStateAudioClip;
        [Header("Multi-State Audio")]
        public AudioClip state1AudioClip;
        public AudioClip state2AudioClip;
        public AudioClip state3AudioClip;
        
        //Change interactable
        public void ChangeInteractable(bool interactable)
        {
            canInteract = interactable;
        }
        
        //For All Items
        public void Idle()
        {
            if (itemState == ItemState.Idle) return;
            
            itemState = ItemState.Idle;
            //广播
            EventCenter.Broadcast(GameEvent.ItemStateChangeEvent);

            if (idleAudioClip) GlobalInstance.AudioManager().PlaySFX(idleAudioClip);
        }
        
        /***For Single-State Items***/
        public void Interacting()
        {
            if (interactingAudioClip && !GlobalInstance.AudioManager().IsPlayingSFX(interactingAudioClip))
            {
                GlobalInstance.AudioManager().PlaySFX(interactingAudioClip);
            }
            
            if (itemState == ItemState.Interacting) return;
            
            itemState = ItemState.Interacting;
            
            EventCenter.Broadcast(GameEvent.ItemStateChangeEvent);
        }
        //互动状态
        public void Interacted()
        {
            if (itemState == ItemState.Interacted) return;
            
            itemState = ItemState.Interacted;
            //广播
            EventCenter.Broadcast(GameEvent.ItemStateChangeEvent);

            if (interactedAudioClip) GlobalInstance.AudioManager().PlaySFX(interactedAudioClip);
        }

        //特殊状态触发的时候
        public void SpecialState()
        {
            if (itemState == ItemState.SpecialState) return;
            
            itemState = ItemState.SpecialState;
            //广播
            EventCenter.Broadcast(GameEvent.ItemStateChangeEvent);
            
            if(specialStateAudioClip) GlobalInstance.AudioManager().PlaySFX(specialStateAudioClip);
        }
        /***For Single-State Items***/
        
        /***For Multi-State Items***/
        public void State1()
        {
            if (itemState == ItemState.State1) return;
            
            itemState = ItemState.State1;
            //广播
            EventCenter.Broadcast(GameEvent.ItemStateChangeEvent);
            
            if(state1AudioClip) GlobalInstance.AudioManager().PlaySFX(state1AudioClip);
        }
        
        public void State2()
        {
            if (itemState == ItemState.State2) return;
            
            itemState = ItemState.State2;
            //广播
            EventCenter.Broadcast(GameEvent.ItemStateChangeEvent);
            
            if(state2AudioClip) GlobalInstance.AudioManager().PlaySFX(state2AudioClip);
        }
        
        public void State3()
        {
            if (itemState == ItemState.State3) return;
            
            itemState = ItemState.State3;
            //广播
            EventCenter.Broadcast(GameEvent.ItemStateChangeEvent);
            
            if(state3AudioClip) GlobalInstance.AudioManager().PlaySFX(state3AudioClip);
        }
        /***For Multi-State Items***/
    }
}