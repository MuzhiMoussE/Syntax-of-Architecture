using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SO
{
    public enum SceneType
    {
        Menu,
        Level
    }

    [CreateAssetMenu(menuName = "GameScene/GameScene")]
    public class GameScene : ScriptableObject
    {
        public SceneType sceneType;
    
        public AssetReference sceneReference;
    }
}