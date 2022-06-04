using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenspaceUI : MonoBehaviour
{
    [SerializeField]
    private Transform follow;
    [SerializeField]
    private Graphic graphic;

    private void Update()
    {
        graphic.rectTransform.position = Camera.main.WorldToScreenPoint(follow.position);
    }
}
