using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_StarWarsScroll : MonoBehaviour
{
    [ContextMenu("Scroll")]
    public void Scroll()
    {
        StartCoroutine(ScrollIt());    
    }

    public float ScrollSpeed = 0.3f;
    IEnumerator ScrollIt() {
        while(true) {
            transform.Translate(Vector3.up * Time.deltaTime * ScrollSpeed);
            yield return null;
        }
    }
}
