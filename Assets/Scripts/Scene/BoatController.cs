using Cinemachine;
using Frame;
using Scene.InteractiveItems;
using UnityEngine;
using UnityEngine.AI;

namespace Scene
{
    public class BoatController : MonoBehaviour
    {
        private Animator _animator;
        private AnimationItem _animationItem;
        private Collider _player;
    
        public CinemachineVirtualCamera virtualCameraBegin;
        public CinemachineVirtualCamera virtualCamera1;
        public CinemachineVirtualCamera virtualCamera2;

        public AudioSource fallWaterAudioSource;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _animationItem = GetComponent<AnimationItem>();
        
            virtualCameraBegin = GameObject.Find("CM vcamBegin").GetComponent<CinemachineVirtualCamera>();
            virtualCamera1 = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();
            virtualCamera2 = GameObject.Find("CM vcam2").GetComponent<CinemachineVirtualCamera>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Settings.TagName.PLAYER) && !_player)
            {
                _player = other;
                _animationItem.SpecialState();
            }
        }
    
        //人上船
        public void BoatBeginRun()
        {
            _player.transform.SetParent(this.transform);
            _player.GetComponent<NavMeshAgent>().enabled = false;
            _animator.SetBool("IsDown", true);
        
            virtualCameraBegin.Priority = 0;
            virtualCamera1.Priority = 100;
        }
    
        //人下船
        public void PeoPleCanMove()
        {
            _player.GetComponent<NavMeshAgent>().enabled = true;
            _player.transform.SetParent(transform.parent.parent);

            //设置相机优先级
            virtualCamera1.Priority = 0;
            virtualCamera2.Priority = 100;
        }

        public void StartFallWaterAudioClip()
        {
            fallWaterAudioSource.Play();
        }
    }
}
