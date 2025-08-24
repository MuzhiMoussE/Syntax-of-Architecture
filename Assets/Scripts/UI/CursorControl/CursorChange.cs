using Frame;
using Scene;
using UnityEngine;

namespace UI.CursorControl
{
    public class CursorChange
    {
        private Texture2D _cursorNormalTexture;
        private Texture2D _cursorPressedTexture;

        private bool _isIndicated;
        
        public void Init()
        {
            //Debug.Log("CursorChange: Init");
            _cursorNormalTexture = Resources.Load<Texture2D>("Textures/Cursor/normal");
            _cursorPressedTexture = Resources.Load<Texture2D>("Textures/Cursor/pressed");
            Cursor.SetCursor(_cursorNormalTexture ,Vector2.zero, CursorMode.Auto);
        }

        public void Update(RaycastHit raycastHit)
        {
            Cursor.SetCursor(
                ((raycastHit.collider.gameObject.CompareTag(Settings.TagName.INTERACTIVE_ITEM) &&
                 (raycastHit.collider.gameObject.GetComponent<InteractiveItem>() != null) &&
                 raycastHit.collider.gameObject.GetComponent<InteractiveItem>().canInteract) ||
                 raycastHit.collider.gameObject.CompareTag(Settings.TagName.LANTERN) ) //可交互
                    ? _cursorPressedTexture
                    : _cursorNormalTexture, Vector2.zero, CursorMode.Auto);
        }

        public void Reset()
        {
            Cursor.SetCursor(_cursorNormalTexture, Vector2.zero, CursorMode.Auto);
        }
    }
}