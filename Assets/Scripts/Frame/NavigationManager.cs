using Unity.AI.Navigation;
using UnityEngine;

namespace Frame
{
    public class NavigationManager : MonoBehaviour
    {
        private NavMeshSurface _staticSurface;
        private NavMeshSurface _dynamicSurface;
        
        public void Init(NavMeshSurface staticSurface, NavMeshSurface dynamicSurface)
        {
            _staticSurface = staticSurface;
            _dynamicSurface = dynamicSurface;
        }
        
        public void RefreshDynamicSurface()
        {
            _dynamicSurface.BuildNavMesh();
        }
    }
}
