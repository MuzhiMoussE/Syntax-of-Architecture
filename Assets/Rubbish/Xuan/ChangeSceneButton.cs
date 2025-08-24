using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ChangeSceneButton : MonoBehaviour
{
    [SerializeField] private GameObject _button;
    public void ChangeScene(string sceneName)
    {
         _button.SetActive(false);
         LevelManager.instance.LoadScene(sceneName);
    }
}
