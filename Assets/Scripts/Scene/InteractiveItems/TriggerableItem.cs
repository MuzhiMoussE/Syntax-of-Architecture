using System;
using Frame;
using UnityEngine;

namespace Scene.InteractiveItems
{
    [Serializable]
    public class TriggerableItem : InteractiveItem
    {
        // Start is called before the first frame update
        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag(Settings.TagName.PLAYER))
            {
                Interacted();
            }
        }
    }
}