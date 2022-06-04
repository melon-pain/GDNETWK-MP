using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameTag : MonoBehaviour
{
    [SerializeField]
    private Transform follow;
    [SerializeField]
    private Text nameTag;

    private void FixedUpdate()
    {
        nameTag.rectTransform.position = Camera.main.WorldToScreenPoint(follow.position + Vector3.up);
    }
}
