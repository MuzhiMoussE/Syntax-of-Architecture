using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMouseRay : MonoBehaviour
{
    // 鼠标位置在世界坐标系中的 实例
    private Transform mousePoint;
    // 高亮物体
    private Transform highLightObj;
    // 物体本身的材质球
    private Material oldMaterial;
    // 当前射线检测到的物体
    private GameObject nowObj;
    [SerializeField]
    private Texture2D cursorTexture;
    void Start()
    {
        // 鼠标的屏幕坐标转成世界坐标的点
        mousePoint = GameObject.CreatePrimitive(PrimitiveType.Sphere).GetComponent<Transform>();
        Destroy(mousePoint.GetComponent<Collider>());
        // 设置物体缩放
        mousePoint.localScale = Vector3.one * 0.1f;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit = new RaycastHit();
        if (Physics.Raycast(ray, out raycastHit))
        { 
            Debug.Log("CreateMouseRay: hit");
            Debug.Log(raycastHit.collider.gameObject.name);
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(null ,Vector2.zero, CursorMode.Auto);
        }
        Debug.DrawRay(Camera.main.transform.position, GetMousePositionOnWorld() - Camera.main.transform.position, Color.red);
        mousePoint.position = GetMousePositionOnWorld();
    }

    private Vector3 GetMousePositionOnWorld()
    {
        Vector3 mousePos = Input.mousePosition;
        // Z 值不能为零，Z 值为零该方法返回值为相机的世界坐标
        // 鼠标坐标值转换为世界坐标时，该方法返回值的 Z  值为：相机的 Z 值加上下面一行代码的赋值
        // 例如：相机 Z 值为-10，经过下面一行代码赋值后，该方法返回值的 Z 值为 0 
        mousePos.z = 138.2f;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}
