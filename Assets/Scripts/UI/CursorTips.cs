using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorTips : MonoBehaviour
{
  public void TipsPlay()
  {
    gameObject.SetActive(true);
  }

  public void TipsStop()
  {
    gameObject.SetActive(false);
  }
}
