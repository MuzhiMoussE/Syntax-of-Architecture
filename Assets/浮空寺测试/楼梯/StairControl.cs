using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Frame;

//楼梯拖动
namespace Scene.InteractiveItems
{
    
    public class StairControl : InteractiveItem
    {
        public List<GameObject> stairs;
        public List<GameObject> bells;
        private List<bool> isRight;
        public bool isCanClick;
        private void Start()
        {
            //初始时不可交互
            canInteract = false;
            isCanClick = true;
            isRight = new List<bool>();
            for (int i = 0;i<stairs.Count; i++)
            {
                isRight.Add(true);
            }
            //初始化 随机打乱
            FlipByID(1);
            FlipByID(2);
        }

        public void FlipByID(int id)
        {
            
            if (id > stairs.Count)
            {
                Debug.LogError("楼梯越界");
                return;
            }
            
            GameObject gameObject = stairs[id];
            //gameObject.transform.Rotate(0,180,0);
            StartCoroutine(StartRotationCoroutine(gameObject));
            isRight[id] = !isRight[id];
        }

        //展示所有铃铛，开始进行楼梯解谜
        public void ShowBell()
        {
            foreach (GameObject bell in bells)
            {
                StartCoroutine(Show(bell));
            }
            GetComponent<AudioSource>().Play();
        }
        
        //铃铛显示,并播放动画
        private IEnumerator Show(GameObject bell)
        {
            isCanClick = false;
            bell.SetActive(true);
        
            Quaternion startRotation = bell.transform.rotation; // 起始旋转
            Quaternion endRotation = Quaternion.Euler(90,0,  0) * startRotation; // 目标旋转

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime ;
                bell.transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
                yield return null;
            }
            //动画放完再交互
            isCanClick = true;
        }
        
        public float rotationTime = 1f; // 旋转时间
        //旋转动画
        private IEnumerator StartRotationCoroutine(GameObject targetObject)
        {
            if(targetObject.GetComponent<AudioSource>()) targetObject.GetComponent<AudioSource>().Play();
            Quaternion startRotation = targetObject.transform.rotation; // 记录起始旋转角度
            Quaternion targetRotation = targetObject.transform.rotation * Quaternion.Euler(0f, 180f, 0f); // 目标旋转角度

            float elapsedTime = 0f; // 已经过去的时间
            while (elapsedTime < rotationTime)
            {
                // 计算当前插值比例
                float t = elapsedTime / rotationTime;

                // 插值计算当前的旋转角度
                targetObject.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

                // 更新已经过去的时间
                elapsedTime += Time.deltaTime;

                yield return null; // 等待下一帧的更新
            }

            targetObject.transform.rotation = targetRotation; // 确保最终角度准确
            
        }
        
        //判断是否全中
        public bool JudgeAllRight()
        {
            foreach (var VARIABLE in isRight)
            {
                if (VARIABLE == false) return false;
            }
            //进入锁定状态
            Interacted();
            foreach(var ls in bells)
            {
                ls.tag = Settings.TagName.UNTAGGED;
            }
            return true;

        }
        
    }
}
