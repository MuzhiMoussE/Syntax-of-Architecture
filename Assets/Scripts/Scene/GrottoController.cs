using System.Collections.Generic;
using Frame;
using UnityEngine;
using UnityEngine.AI;

namespace Scene
{
    public class GrottoController : MonoBehaviour
    {
        //当前石窟状态
        [SerializeField] private int grottoState;

        //石窟列表
        public List<GameObject> grottos;

        [SerializeField] private List<Vector3> targetPlayerPositions;
        private int _nextPositionIndex;

        private void Awake()
        {
            _nextPositionIndex = 0;
            foreach (var grotto in grottos)
            {
                grotto.SetActive(false);
            }
            grottoState = -1;
            ShowNextGrotto();
        }
        
        public void ShowNextGrotto()
        {
            if (grottoState >= 0)
                grottos[grottoState].SetActive(false);
            
            ++grottoState;
            
            if (grottoState >= grottos.Count) return;
            
            grottos[grottoState].SetActive(true);
            GlobalInstance.NavigationManager().RefreshDynamicSurface();

            
            if(grottoState == 2)
                ChangeNextPlayerPosition();
        }

        private void ChangeNextPlayerPosition()
        {
            if(_nextPositionIndex >= targetPlayerPositions.Count) return;
            
            var player = GlobalInstance.Player();
            player.GetComponent<NavMeshAgent>().enabled = false;
            player.transform.position = targetPlayerPositions[_nextPositionIndex++];
            player.GetComponent<NavMeshAgent>().enabled = true;
        }
    }
}
