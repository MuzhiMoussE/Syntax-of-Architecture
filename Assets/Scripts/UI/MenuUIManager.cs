using System;
using System.Collections;
using Cinemachine;
using Frame;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;
using UnityEngine.UI;
using DG.Tweening;

namespace UI
{
    public class MenuUIManager : SingletonMonoBase<MenuUIManager>
    {
        private Transform _lastTag = null;
        [SerializeField] private GameObject bookOpen;
        [SerializeField] private GameObject coverOpen;
        [SerializeField] private GameObject mCamera;
        [SerializeField] private AudioSource tagTriggerAudioSource;
        [SerializeField] private AudioSource bookOpenAudioSource;
        [SerializeField] private Slider volumeSlider;
        private bool _isLock = false;
        private bool _isOpen = false;
        private Animator _animator;
        private Animator _animatorCover;
        //当有界面覆盖时，禁止射线检测
        private bool _haveFrontMenu;

        [SerializeField] private AudioClip menuBGMClip;
        [SerializeField] private float menuBGMVolume = 1f;

        private void Start()
        {
            _animator = bookOpen.GetComponent<Animator>();
            _animatorCover = coverOpen.GetComponent<Animator>();
            //Debug.Log("MenuUIManager:"+_animator);
            _animator.Play("BookOpen", 0, 0.99f);
            _animator.SetFloat(Settings.AnimatorParameters.SPEED, 0f);
            _animatorCover.Play("Cover_open", 0, 0f);
            _animatorCover.SetFloat(Settings.AnimatorParameters.SPEED, 0f);
            _haveFrontMenu = false;
            
            GlobalInstance.AudioManager().PlayBGM(menuBGMClip, menuBGMVolume);
        }

        public void TagTrigger(RaycastHit hit)
        {
            if (!hit.transform.CompareTag("MenuTag") || _isOpen || _haveFrontMenu)
                return;
            if ((_lastTag == null || _lastTag.name != hit.transform.name) && !_isLock)
            {
                //播放声音
                tagTriggerAudioSource.Play();
              //  StopAllCoroutines();
                StartCoroutine(OffsetAToRight(hit.transform));
                if (_lastTag != null)
                    StartCoroutine(ReOffset(_lastTag));
                _lastTag = hit.transform;
            }

            if (!Mouse.current.leftButton.isPressed) return;


            //硬编码 有意见？

            //说的好！from 木某人实名支持
            //小蝴蝶：6
            switch (hit.transform.name)
            {
                case "Tag1":
                    _isOpen = true;
                    StartCoroutine(PlayBookOpenAnimation());
                    StartCoroutine(CameraMove());
                    StartCoroutine(CameraRotate());
                    // StartButton();
                    break;
                case "Tag2":
                    OpenSettingMenu();
                    break;
                case "Tag3":
                    StartCoroutine(OpenStaffMenu());
                    break;
                case "Tag4":
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                    break;
            }
        }


        private IEnumerator PlayBookOpenAnimation()
        {
            EventCenter.Broadcast(GameEvent.ScenePreloadEvent, GlobalInstance.SceneManager().gameScenes[0]);
            //  bookOpen.transform.localScale = Vector3.one;
            _animator.SetFloat(Settings.AnimatorParameters.SPEED, -1f);
            _animatorCover.SetFloat(Settings.AnimatorParameters.SPEED, 1f);
            //播放声音
            bookOpenAudioSource.Play();
            yield return new WaitForSeconds(5f);
            EventCenter.Broadcast(GameEvent.PreloadSceneDisplayEvent);
        }

        //相机位置移动
        private IEnumerator CameraMove()
        {
            CinemachineVirtualCamera camera = mCamera.GetComponent<CinemachineVirtualCamera>();
            CinemachineTrackedDolly path = camera.GetCinemachineComponent<CinemachineTrackedDolly>();
            while (path.m_PathPosition < 3f)
            {
                if (path.m_PathPosition < 2f)
                    path.m_PathPosition += 0.04f;
                else
                {
                    path.m_PathPosition += 0.04f;
                }

                yield return new WaitForSeconds(0.08f);
            }
        }

        //相机角度移动
        private IEnumerator CameraRotate()
        {
            Quaternion target = Quaternion.Euler(0, 0, 0);
            while (Quaternion.Angle(target, mCamera.transform.rotation) > 0.1f)
            {
                mCamera.transform.rotation =
                    Quaternion.Slerp(mCamera.transform.rotation, target, Time.deltaTime * 0.5f);
                yield return new WaitForSeconds(0.005f);
            }
        }

        //加载下一场景 方法已弃用
        public void StartButton()
        {
            Debug.Log("start!");
            //   EventCenter.Broadcast(GameEvent.SceneUnloadEvent);
            // EventCenter.Broadcast(GameEvent.PreloadSceneDisplayEvent);
            //   EventCenter.Broadcast(GameEvent.ScenePreloadEvent, GlobalInstance.SceneManager().gameScenes[0]);
        }

        public void OpenSettingMenu()
        {
            _haveFrontMenu = true;
            StartCoroutine(OpenAnimation("SettingMenu"));
        }

        private IEnumerator OpenAnimation(String menuName, float time = 0.5f)
        {
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
           // Debug.Log("MenuUIManager[CloseAnimation]: IEnumerator yield return!");
            menu.GetComponent<RectTransform>().localScale = Vector3.zero;
        }
        
        public void CloseSettingMenu()
        {
            _haveFrontMenu = false;
            StartCoroutine(CloseAnimation("SettingMenu"));
        }
        
        public IEnumerator OpenStaffMenu()
        {
            _haveFrontMenu = true;
            StartCoroutine(OpenAnimation("StaffMenu"));
            var fadingContainer = GameObject.Find("FadingContainer").transform;
            //遍历FadingContainer中的所有子物体
            foreach (Transform child in fadingContainer)
            {
                yield return new WaitForSeconds(0.5f);
                if(child.name == "Maker")
                    yield return new WaitForSeconds(0.5f);
                StartCoroutine(OpenAnimation(child.name, 2f));
            }

        }
        
        public void CloseStaffMenu()
        {
            _haveFrontMenu = false;
            StartCoroutine(CloseAnimation("StaffMenu"));
            var fadingContainer = GameObject.Find("FadingContainer").transform;
            foreach (Transform child in fadingContainer)
            {
                StartCoroutine(CloseAnimation(child.name));
            }
        }


        private IEnumerator OffsetAToRight(Transform tagTransform)
        {
            _isLock = true;
            Vector3 offset = Vector3.right * 2f;
            Vector3 targetPositon = tagTransform.position + offset;
            var position = tagTransform.position;
            while (Vector3.Distance(position, targetPositon) > 0.1f)
            {
                position = tagTransform.position;
                position = Vector3.Lerp(position, targetPositon, 0.1f);
                tagTransform.position = position;
                yield return null;
            }

            _isLock = false;
        }

        private IEnumerator ReOffset(Transform tagTransform)
        {
            _isLock = true;
            Vector3 offset = Vector3.right * 2f;
            Vector3 targetPositon = tagTransform.position - offset;
            var position = tagTransform.position;
            while (Vector3.Distance(position, targetPositon) > 0.1f)
            {
                position = tagTransform.position;
                position = Vector3.Lerp(position, targetPositon, 0.1f);
                tagTransform.position = position;
                yield return null;
            }

            _isLock = false;

        }

        //通过volumeSlider调节音量
        //注意：本音量调节为非可持久化调节
        public void VolumeSliderChange()
        {
            AudioListener.volume = volumeSlider.value;
        }
    }
}