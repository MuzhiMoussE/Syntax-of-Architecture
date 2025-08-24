using System;
using System.Collections;
using System.Collections.Generic;
using Frame;
using Scene;
using UnityEngine;
public class WaveEffect : MonoBehaviour {
    //为了调数值，这里！必须！硬编码！
    
    //挂在可交互物体上的
    /// <summary>
    /// 水面质点水平波动振幅
    /// </summary>
    [Header("水面质点水平波动振幅")]
    [SerializeField]
    public float A = 1.5f; 
    /// <summary>
    /// 水波截面波形角频率(值越大, 波纹越密)
    /// </summary>
    [Header("水波截面波形角频率(值越大, 波纹越密)")]
    [SerializeField]
    public float w1 = 150; 
    /// <summary>
    /// 水面质点水平波动角频率(值越大, 水波质点振动越快)
    /// </summary>
    [Header("水面质点水平波动角频率(值越大, 水波质点振动越快)")]
    [SerializeField]
    public float w2 = 10; 
    /// <summary>
    /// 扩散部分的水波宽度
    /// </summary>
    [Header("扩散部分的水波宽度")]
    [SerializeField]
    public float waveWidth = 0.05f;
    /// <summary>
    /// 传播速度
    /// </summary>
    [Header("传播速度")]
    [SerializeField]
    public float waveSpeed = 0.3f; 
    /// <summary>
    /// 水波产生的间隔时间
    /// </summary>
    [Header("水波产生的间隔时间")]
    [SerializeField] private float MaxTime = 3f;
    private float waveTime; // 水波传播时间
    /// <summary>
    /// 水波中心位置列表
    /// </summary>
    [SerializeField]
    private List<Vector4> waveCenter; 
    private Material waveMaterial; // 水波材质
    private bool enabledWave = false; // 水波开关
    /// <summary>
    /// 渲染的canvas
    /// </summary>
    [SerializeField]
    private Canvas MainCanvas;
    /// <summary>
    /// 可交互物体列表
    /// </summary>
    [SerializeField]
    private GameObject []interactObj;

    
    private void Start()
    {
        WaveAwake();
    }

    public void WaveAwake() {
        //Debug.Log("水波纹初始化");
        waveMaterial = new Material(Shader.Find("Book/WaterWave"));
        waveMaterial.hideFlags = HideFlags.DontSave;
        interactObj = GameObject.FindGameObjectsWithTag("InteractiveItem");
        foreach (var item in interactObj)
        {
            waveCenter.Add(GetScreenPosition(item.gameObject));
        }

        enabledWave = true;

    }
    public Vector4 GetScreenPosition(GameObject target)//获取到屏幕坐标
    {
        Vector4 viewportPos = Camera.main.WorldToViewportPoint(target.transform.position);
        RectTransform canvasRtm = MainCanvas.GetComponent<RectTransform>();
        Debug.Log(MainCanvas.name);
        Vector2 uguiPos = Vector2.zero;
        uguiPos.x = (viewportPos.x - 0.5f) * canvasRtm.sizeDelta.x;
        uguiPos.y = (viewportPos.y - 0.5f) * canvasRtm.sizeDelta.y;
        return uguiPos;
    }

    IEnumerator WaveStart()
    {

        while (true)
        {
            enabledWave = true;
            
            yield return new WaitForSeconds(MaxTime);

        }
    }


    void OnRenderImage (RenderTexture source, RenderTexture destination) {
        if (enabledWave) {
            //Debug.Log("waveeeeeeeeeeeeeeeeeeeeeeee!!!!!!!!!!nnd你倒是动啊 啊啊啊啊啊啊啊啊啊啊啊");
            StartCoroutine("WaveStart");
            SetWaveMaterialParams();
            Graphics.Blit (source, destination, waveMaterial);
            waveTime += Time.deltaTime;
            if (waveTime > 2 / waveSpeed) { // 结束水波特效
                enabledWave = false;
                waveTime = 0f;
            }
        } else {
            Graphics.Blit (source, destination);
        }
    }
 
    private void SetWaveMaterialParams() { // 设置水波材质参数
        waveMaterial.SetFloat("_A", A); 
        waveMaterial.SetFloat("_w1", w1);
        waveMaterial.SetFloat("_w2", w2); 
        waveMaterial.SetFloat("_t", waveTime);
        
        waveMaterial.SetFloat("_waveDist", waveTime * waveSpeed);
        waveMaterial.SetFloat("_waveWidth", waveWidth);
        foreach (var item in waveCenter)
        {
            waveMaterial.SetVector("_o", item); 
        }
    }
}