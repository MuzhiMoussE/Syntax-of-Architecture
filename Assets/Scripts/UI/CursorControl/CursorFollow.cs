using UnityEngine;

namespace UIControl.CursorControl
{
    public class CursorFollow
    {
        private Transform _mouseTrail;

        public void Init()
        {
            _mouseTrail = GameObject.Find("MouseTrail").transform;
        }
        public void Update(Vector2 mousePosition)
        {
            //获取拖拽点鼠标坐标
            //print(Input.mousePosition.x + "     y  " + Input.mousePosition.y + "     z  " + Input.mousePosition.z);
            //新的屏幕点坐标
            Vector3 currentScenePosition = new Vector3(mousePosition.x, mousePosition.y, 8f);
            //将屏幕坐标转换为世界坐标
            Vector3 crrrentWorldPosition = Camera.main.ScreenToWorldPoint(currentScenePosition); 
            //设置对象位置为鼠标的世界位置
            _mouseTrail.position = crrrentWorldPosition;
        }
    }
}