using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Frame;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Utility;
using SceneManager = UnityEngine.SceneManagement.SceneManager;
using UnityEngine.UI;
using DG.Tweening;
using UnityEditor;
namespace UI
{
    public class SeekRoadUIManager :  SingletonMonoBase<SeekRoadUIManager>
    {
        public GameObject titleCanvas;
        
        public List<string> totemNameList = new List<string>();
        public bool _haveFrontMenu;
        private int _totemIndex;

        private bool _isExitMenuOpen;
        //Debug用保留接口
        [SerializeField] 
        private bool isTrigger = false;

        protected override void Awake()
        {
            base.Awake();
            _totemIndex = 0;
            _isExitMenuOpen  = false;
            _haveFrontMenu = false;
        }
        
        // Update is called once per frame
        private void Update()
        {
            //Debug用保留接口
            if (isTrigger)
            {
                isTrigger = false;
                LightNextTotem();
            }
        }

        //改换list中下一个totem的sprite
        public void LightNextTotem()
        {
            if(_totemIndex >= totemNameList.Count) return;
            Image image = GameObject.Find(totemNameList[_totemIndex]).GetComponent<Image>();
            image.sprite = Resources.Load<Sprite>("Textures/Totem/totem_" + _totemIndex.ToString() + "_light");
            _totemIndex++;
            //Debug.Log("SeekRoadUIManager[LightNextTotem]: Success!");
        }

        public IEnumerator OpenOrCloseExitMenuAnimation()
        {
            if (!_isExitMenuOpen)
            {
                //打开ExitMenu
                //硬编码YYDS
                _isExitMenuOpen = true;
                _haveFrontMenu = true;
                var menu = GameObject.Find("ExitMenu");
                menu.GetComponent<RectTransform>().localScale = Vector3.one;
                menu.GetComponent<CanvasGroup>().DOFade(1,0.5f);
                yield return new WaitForSeconds(0.5f);
                //Time.timeScale = 0;
            }
            else
            {
                //Time.timeScale = 1;
                var menu = GameObject.Find("ExitMenu");
                menu.GetComponent<CanvasGroup>().DOFade(0,0.5f);
                yield return new WaitForSeconds(0.5f);
                menu.GetComponent<RectTransform>().localScale = Vector3.zero;
                _isExitMenuOpen = false;
                _haveFrontMenu = false;
            }
        }

        public void OpenEndMenu()
        {
            StartCoroutine(OpenEndMenuAnimation());
        }
        
        private IEnumerator OpenEndMenuAnimation()
        {
            _haveFrontMenu = true;
            StartCoroutine(OpenAnimation("EndMenu"));
            var fadingContainer = GameObject.Find("FadingContainer").transform;
            //遍历FadingContainer中的所有子物体
            foreach (Transform child in fadingContainer)
            {
                yield return new WaitForSeconds(2f);
                if(child.name == "MaskImage")
                {
                    yield return new WaitForSeconds(0.5f);
                    StartCoroutine(OpenAnimation(child.name, 4f));
                }
                StartCoroutine(OpenAnimation(child.name, 2f));
            }
        }
        
        private IEnumerator OpenAnimation(String menuName, float time = 0.5f)
        {
            _haveFrontMenu = true;
            var menu = GameObject.Find(menuName);
            menu.GetComponent<RectTransform>().localScale = Vector3.one;
            menu.GetComponent<CanvasGroup>().DOFade(1,time);
            yield return new WaitForSeconds(time);
        }
        private IEnumerator CloseAnimation(String menuName, float time = 0.5f)
        {
            var menu = GameObject.Find(menuName);
            menu.GetComponent<CanvasGroup>().DOFade(0,time);
            yield return new WaitForSeconds(time);
            menu.GetComponent<RectTransform>().localScale = Vector3.zero;
            _haveFrontMenu = false;
        }
        
        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
        }

        public void OpenOrCloseExitGameMenu()
        {
            StartCoroutine(OpenOrCloseExitMenuAnimation());
        }
    }
}