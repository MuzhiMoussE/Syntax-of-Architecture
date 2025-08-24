using System;
using System.Collections;
using System.Collections.Generic;
using Frame;
using Scene;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scene.InteractiveItems
{
    [Serializable]
    public class RotatableItem : InteractiveItem
    {
        [SerializeField] private float rotationSpeed = 1f;

        public void RotateByMouse(float x, float y, Vector2 mousePosition)
        {
            if (!canInteract) return;
            Interacting();
            if (CheckRotationRange())
            {
                StartCoroutine(RotateToRightPosition());
            }
            //鼠标控制物体旋转
            //比较鼠标和transform在屏幕上的坐标
            var transformPos = Camera.main.WorldToScreenPoint(transform.position);

            //旋转方向判断 这里的机制非常负责，没事千万别碰这坨
            var rotateDirection = 1;

            if (mousePosition.x < transformPos.x)
            {
                //左下
                if (mousePosition.y < transformPos.y)
                {
                    if (x < 0 || y > 0)
                    {
                        rotateDirection = -1;
                    }
                }
                else
                {
                    //左上
                    if (x > 0 || y > 0)
                    {
                        rotateDirection = -1;
                    }
                }
            }
            else
            {
                //右上
                if (mousePosition.y > transformPos.y)
                {
                    if (x > 0 || y < 0)
                    {
                        rotateDirection = -1;
                    }
                }
                //右下
                else
                {
                    if (x < 0 || y < 0)
                    {
                        rotateDirection = -1;
                    }     
                }
            }
            //搁这算术呢
            transform.Rotate(new Vector3(0, 0, rotateDirection*(float)Math.Sqrt(x * x + y * y)*rotationSpeed));
            Idle();
        }

        private IEnumerator RotateToRightPosition()
        {
            canInteract = false;
            
            const float rotateSpeed = 1f;
            //转到0度角
            if (transform.rotation.eulerAngles.z > 0)
            {
                while (CheckRotationRange()&&transform.rotation.eulerAngles.z > 2f)
                {
                    transform.Rotate(new Vector3(0, 0, -rotateSpeed));
                    yield return null;
                }
            }
            else
            {
                while (CheckRotationRange()&&transform.rotation.eulerAngles.z < -2f)
                {
                    transform.Rotate(new Vector3(0, 0, rotateSpeed));
                    yield return null;
                }
            }
            //直接归位
            transform.rotation = Quaternion.Euler(0, 0, 0);
            Interacted();
            // GlobalInstance.NavigationManager().RefreshDynamicSurface();
            //_animationLock = false;
        }
        
        //检测rotation是否在阈值度角内
        private bool CheckRotationRange()
        {
            const float rotationThreshold = 15f;
            var rotation = transform.rotation.eulerAngles.z;
            return rotation is >= -rotationThreshold and <= rotationThreshold;
        }
    }
}