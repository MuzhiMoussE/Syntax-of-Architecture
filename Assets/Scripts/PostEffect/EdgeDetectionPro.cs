using UnityEngine;
 
[RequireComponent(typeof(Camera))] // 需要相机组件
public class EdgeDetectionPro : MonoBehaviour {
    [Range(0.0f, 1.0f)]
    public float edgesOnly = 0.0f; // 是否仅显示边缘
    public Color edgeColor = Color.black; // 边缘颜色
    public Color backgroundColor = Color.white; // 背景颜色
    public float sampleScale = 1.0f; // 采样缩放系数(值越大, 描边越宽)
    public float depthScale = 1.0f; // 深度缩放系数(值越大, 越易识别为边缘)
    public float normalScale = 1.0f; // 法线缩放系数(值越大, 越易识别为边缘)
 
    private Material material; // 材质
    public Shader shader;
    private void Start() {
        material = new Material(shader);
        material.hideFlags = HideFlags.DontSave;
    }
 
    private void OnEnable() {
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.DepthNormals;
    }
 
    //[ImageEffectOpaque] // 在不透明的 Pass 执行完毕后立即调用 OnRenderImage 方法(透明物体不需要描边)
    private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        if (material != null) {
            material.SetFloat("_EdgeOnly", edgesOnly);
            material.SetColor("_EdgeColor", edgeColor);
            material.SetColor("_BackgroundColor", backgroundColor);
            material.SetFloat("_SampleScale", sampleScale);
            material.SetFloat("_DepthScale", depthScale);
            material.SetFloat("_NormalScale", normalScale);
            Graphics.Blit(src, dest, material);
        } else {
            Graphics.Blit(src, dest);
        }
    }
}