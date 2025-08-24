using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{ 
    public static LevelManager instance;
    [SerializeField] private GameObject _loaderCanvas;
    [SerializeField] private GameObject _scrollBar;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public async Task LoadScene(string name)
    {
        var scene = SceneManager.LoadSceneAsync(name);
        //允许在场景准备就绪后立即激活场景。
        scene.allowSceneActivation = false;
        _loaderCanvas.SetActive(true);
        Scrollbar scrollbar = _scrollBar.GetComponent<Scrollbar>();
        do
        {
            await Task.Delay(200);
            scrollbar.size += 0.1f;
        } while (scrollbar.size < 0.9f);
        await Task.Delay(1000);
        scene.allowSceneActivation = true;
        _loaderCanvas.SetActive(false);
    }
}
