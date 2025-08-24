using System;
using UnityEngine;
using Frame;
using UnityEngine.Serialization;

//这个脚本为动画更新和input读入的中间脚本
namespace Scene.InteractiveItems
{
    [Serializable]
    [RequireComponent(typeof(Animator))]
    public class AnimationItem : InteractiveItem
    {
        private Animator _animator;

        private float _speed = 0;
        
        public bool dragVertical;
        //动画初始状态
        [Range(0,1)][SerializeField] private float initAnimationState;
        //Xuan：保留接口，以防出现不依靠拖拉自行播放动画的事件
        [SerializeField] private bool isPlayBySelf;
        //动画正反及播放速度
        [SerializeField] private float dragSpeed = -10;
        //操作反向
        [SerializeField] private bool isReverse;
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            //初始化动画进度
            _animator.Play(Settings.Animations.INTERACTIVE_ITEM_ANIMATION, 0, initAnimationState);
        }

        public void AnimationMove(Vector3 moveVector)
        {
            if (!dragVertical) _speed = moveVector.x * Mathf.Abs(dragSpeed);
            else _speed = moveVector.y * Mathf.Abs(dragSpeed);
            
            if (isReverse) _speed = -_speed;
        }

        private void Update()
        {
            if (!canInteract) return;
            
            if ((itemState == ItemState.Interacted && _speed >= 0) ||
                (itemState == ItemState.Idle && _speed <= 0) )
            {
                _animator.SetFloat(Settings.AnimatorParameters.SPEED, 0);
            }
            else
            {
                if (itemState != ItemState.Interacting)
                {
                    Interacting();
                }

                if (dragSpeed > 0) _animator.SetFloat(Settings.AnimatorParameters.SPEED, _speed);
                else _animator.SetFloat(Settings.AnimatorParameters.SPEED, -_speed);
            }
        }
    }
}
