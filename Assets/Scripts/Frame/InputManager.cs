using Scene.InteractiveItems;
using UI;
using UIControl.CursorControl;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Scene;
using UI.CursorControl;

namespace Frame
{
    public class InputManager : MonoBehaviour
    {
        private Player.Player _player;
        private InputAction _mousePositionAction;//获得鼠标屏幕坐标
        private InputAction _mouseClickAction;//获取鼠标是否点击
        private InputAction _mouseMoveAction;//获取鼠标移动
        
        //这里改了 改错了的话就是这里有错
        //private AnimationItem _animationItem;//用于存储当前控制的目标的脚本
        private AnimationItem _animationItem;//用于存储当前控制的目标的脚本 这是动画交互物体
        private DragRotatableItem_MultiState _dragRotatableItem;//用于存储当前控制的目标的脚本 这是 非 动画交互物体
        
        private CursorChange _cursorChange = new();//鼠标纹理改变
        private CursorFollow _cursorFollow = new();//鼠标拖尾跟随

        public bool canInput;//是否可以输入
        
        public void Awake()
        {
            _mousePositionAction = new InputAction("MousePosition", binding: "<Mouse>/position");
            _mouseClickAction = new InputAction("MouseClick", binding: "<Mouse>/leftButton");
            _mouseMoveAction= new InputAction("MouseMovement", binding: "<Mouse>/delta");
            //按下鼠标左键
            _mouseClickAction.started += DownLeftButton;
            _mouseClickAction.canceled += UpLeftButton;
            _mouseClickAction.performed += KeepButton;
            _mouseMoveAction.performed += MoveMouse;
            //初始化这两个东西
            _cursorChange.Init();
            _cursorFollow.Init();

            canInput = true;
        }
        
        //按下鼠标左键时
        private void DownLeftButton(InputAction.CallbackContext context)
        {
            //获取鼠标位置
            var mousePosition = _mousePositionAction.ReadValue<Vector2>();
            //获取射线检测的物品
            Physics.Raycast(Camera.main.ScreenPointToRay(mousePosition),out var hit, 100);
            //中间脚本更新
            if (hit.collider)
            {
                //如果是灯笼，则按下
                if(hit.collider.gameObject.CompareTag("Lantern"))hit.collider.gameObject.GetComponent<LanternDown>().Down();
                //动画
                _animationItem = hit.collider.gameObject.GetComponent<AnimationItem>();
                //非动画
                _dragRotatableItem = hit.collider.gameObject.GetComponent<DragRotatableItem_MultiState>();
            }
        }
        
        //放开鼠标左键时
        private void UpLeftButton(InputAction.CallbackContext context)
        {
            //先停止动画播放
            if (_animationItem != null)
            {
                _animationItem.AnimationMove(new Vector2(0,0));
                //移除控制的脚本
                _animationItem = null;
            }
            //先停止旋转
            if (_dragRotatableItem != null)
            {
                _dragRotatableItem.Rotate(new Vector2(0,0));
                //移除控制的脚本
                _dragRotatableItem = null;
            }
        }
        
        //鼠标长按时
        private void KeepButton(InputAction.CallbackContext context)
        {
            
        }
        
        //鼠标移动时
        private void MoveMouse(InputAction.CallbackContext context)
        {
            
        }
        
        public void Init(Player.Player player)
        {
            _player = player;
        }

        //开启鼠标
        private void OnEnable()
        {
            EventCenter.AddListener(GameEvent.AfterTitleEvent, () => canInput = true);
            _mousePositionAction.Enable();
            _mouseClickAction.Enable();
            _mouseMoveAction.Enable();
        }
        
        //禁用鼠标
        private void OnDisable()
        {
            EventCenter.RemoveListener(GameEvent.AfterTitleEvent, () => canInput = true);
            _mousePositionAction.Disable();
            _mouseClickAction.Disable();;
            _mouseMoveAction.Disable();
        }

        public void Update()
        {
            //键盘监听ESC
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                SeekRoadUIManager.Instance?.OpenOrCloseExitGameMenu();
            }
            if (!canInput) return;

            //鼠标位置
            var mousePosition = _mousePositionAction.ReadValue<Vector2>();
            //根据鼠标位置更新鼠标拖拽效果
            _cursorFollow.Update(mousePosition);
            //每时每刻射线检测 一般情况都会进行
            if (Physics.Raycast(Camera.main.ScreenPointToRay(mousePosition),
                    out var hit, 100))
            {
                //UI 界面弹出时禁止射线检测
                //【注意】不能全局禁用输入
                if(SeekRoadUIManager.Instance != null && SeekRoadUIManager.Instance._haveFrontMenu)
                        return;
                //根据鼠标所指物品进行逻辑判断
                MenuUIManager.Instance.TagTrigger(hit);
                //更改鼠标纹理
                _cursorChange.Update(hit);
                //Xuan的旋转脚本
                if(Mouse.current.leftButton.isPressed)
                    hit.collider.gameObject.GetComponent<RotatableItem>()?.RotateByMouse(Mouse.current.delta.x.ReadValue(),Mouse.current.delta.y.ReadValue(),mousePosition);
                //本来这里想放在鼠标移动中的，不过逻辑有问题
                //控制物体不为空则移动 
                if (_animationItem != null)
                {
                    //读取位移量
                    Vector2 moved = _mouseMoveAction.ReadValue<Vector2>();
                    //物体移动
                    _animationItem.AnimationMove(moved);
                }
                //非动画旋转
                if (_dragRotatableItem != null)
                {
                    //读取位移量
                    Vector2 moved = _mouseMoveAction.ReadValue<Vector2>();
                    //物体移动
                    _dragRotatableItem.Rotate(moved);
                }
            }
            else
            {
                _cursorChange.Reset();
            }
            //如果什么都没碰到，则抛出异常 标红好难看，算了
            //else throw new Exception("射线检测时什么都没碰到");          
           
        }
        
    }
}
