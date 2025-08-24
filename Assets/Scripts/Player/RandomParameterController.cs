using Frame;
using UnityEngine;

namespace Player
{
    public class RandomParameterController : MonoBehaviour
    {
        public float minValue = 0f;
        public float maxValue = 1f;
        public float updateInterval = 1f;

        private Animator _animator;
        private float _timer;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _timer = 0f;
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (!(_timer >= updateInterval)) return;
        
            // 生成随机数并更新Animator参数的值
            var randomValue = Random.Range(minValue, maxValue);
            _animator.SetFloat(Settings.AnimatorParameters.RANDOM_VALUE, randomValue);

            _timer -= updateInterval; // 重置计时器
        }
    }
}