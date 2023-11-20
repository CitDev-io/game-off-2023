using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class UI_OccassionalFlip : MonoBehaviour
{
    SkeletonAnimation _spineSkin;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FlipOccassionally());
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator FlipOccassionally() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(5f, 22f));
            Debug.Log("FLIP");
            GetComponent<SkeletonGraphic>().AnimationState.AddAnimation(0, "victory", false, 0f);
            // GetComponent<SkeletonGraphic>().AnimationState.AddAnimation(0, "victory", false, 0f);
        }
    }
}
