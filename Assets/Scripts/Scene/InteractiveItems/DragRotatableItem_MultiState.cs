using System.Collections;
using UnityEngine;

//设计之初没有设计非动画的交互模式，但对于需求中的旋转行为，直接使用代码控制是最方便的，所以单独开一个脚本用来控制转盘旋转
//PS：绝对不是PM的问题 PM我滴超人😭
namespace Scene.InteractiveItems
{
    public class DragRotatableItem_MultiState : InteractiveItem
    {
        [SerializeField] private int stateNum = 3;
        //旋转速度
        [SerializeField] private float moveSpeed;
        //校准范围 优化玩家体验
        [SerializeField] private float calibrationRange;
        //真实旋转值
        [SerializeField] private float realRotationValue;
        //表现旋转值
        [SerializeField] private float showRotationValue;
        //佛像的偏移
        [SerializeField] private float rotationOffset;

        private void Start()
        {
            realRotationValue = transform.rotation.eulerAngles.y;
            StartCoroutine(RotateObject());
        }

        public void Rotate(Vector2 dragVector2)
        {
            if (!canInteract) return;
            
            var angle = dragVector2.x * moveSpeed;
            
            realRotationValue += angle;
            while (realRotationValue >= 360) realRotationValue -= 360;
            while (realRotationValue < 0) realRotationValue += 360;
            
            //更新表现出来的旋转角
            GetShowRotationValue();
            transform.GetComponent<AudioSource>();
            //设置rotation
            transform.rotation = Quaternion.Euler(0, showRotationValue, 0);
        }

        //获取真的旋转角 顺便记录佛像
        private void GetShowRotationValue()
        {
            var judgeNum = rotationOffset;
            var stateInterval = 360f / stateNum;

            for (var i = 1; i <= stateNum; i++)
            {
                judgeNum += stateInterval;
                while (judgeNum > 360) judgeNum -= 360;
                if (Mathf.Abs(realRotationValue - judgeNum) <= calibrationRange)
                {
                    // itemState = (ItemState)(MultiStateOffset + i);
                    switch (i)
                    {
                        case 1: State1();
                            break;
                        case 2: State2();
                            break;
                        case 3: State3();
                            break;
                    }
                    showRotationValue = judgeNum;
                    return;
                }
            }
            
            showRotationValue = realRotationValue;
            Interacting();
        }
        
        //旋转上升
        private IEnumerator RotateObject()
        {
            var duration = 2f;  // 旋转上升总时间
            var elapsed = 0f;  // 已经过去的时间

            var startRotation = transform.rotation * Quaternion.Euler(0f, 180f, 0f);  // 初始旋转
            var targetRotation = transform.rotation;  // 目标旋转（这里以绕 Y 轴旋转90度为例）

            var startPosition = transform.position + Vector3.down * 3;// 初始位置
            var targetPosition = transform.position ;  // 目标位置（向上移动一个单位）

            while (elapsed < duration)
            {
                var t = elapsed / duration;  // 计算插值参数

                // 进行插值旋转
                transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

                // 进行插值移动
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);

                // 更新已经过去的时间
                elapsed += Time.deltaTime;

                yield return null;  // 等待一帧
            }
        }

        public void RoadDown()
        {
            StartCoroutine(RotateObjectDown());
        }
        
        //旋转下降
        private IEnumerator RotateObjectDown()
        {    
            var duration = 2f;  // 旋转下降总时间
            var elapsed = 0f;  // 已经过去的时间

            var startRotation = transform.rotation ;  // 初始旋转
            var targetRotation = transform.rotation * Quaternion.Euler(0f, 180f, 0f) ;  // 目标旋转（这里以绕 Y 轴旋转90度为例）

            var startPosition = transform.position;  // 初始位置
            var targetPosition = transform.position + Vector3.down * 5;  // 目标位置（向上移动一个单位）

            while (elapsed < duration)
            {
                var t = elapsed / duration;  // 计算插值参数

                // 进行插值旋转
                transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

                // 进行插值移动
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);

                // 更新已经过去的时间
                elapsed += Time.deltaTime;

                yield return null;  // 等待一帧
            }
        }
    }
}
