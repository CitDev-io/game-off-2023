using TMPro;
using UnityEngine;
using Spine.Unity;

public class UI_ScalePanelManager : MonoBehaviour
{
    SkeletonGraphic _spineSkin;
    int _prevLight = 0;
    int _prevShadow = 0;

    void Awake() {
        _spineSkin = GetComponent<SkeletonGraphic>();
    }

    public void SetScale(int lightCount, int shadowCount) {
        int sunFavorBefore = _prevLight - _prevShadow;
        int sunFavorAfter = lightCount - shadowCount;

        string animName;
        if (sunFavorBefore < 0 || sunFavorAfter < 0) {
            animName = "moon" + Mathf.Abs(sunFavorBefore).ToString() + "-" + Mathf.Abs(sunFavorAfter).ToString() + "sun" + lightCount;
        } else {
            animName = "sun" + sunFavorBefore + "-" + sunFavorAfter + "moon" + shadowCount;
        }

        SetAnimation(0, animName, false);

        _prevLight = lightCount;
        _prevShadow = shadowCount;

        AddAnimation(0, "idle-moon" + shadowCount + "sun" + lightCount, true, 0f);
        AddAnimation(0, "idle-sun" + lightCount + "moon" + shadowCount, true, 0f);
    }

    bool HasAnimation(string animation) {
        if (_spineSkin.skeletonDataAsset.GetSkeletonData(true).FindAnimation(animation) != null) {
            return true;
        }
        return false;
    }

    void SetAnimation(int trackIndex, string animation, bool loop) {
        if (HasAnimation(animation)) {
            _spineSkin.AnimationState.SetAnimation(trackIndex, animation, loop);
        }
    }

    void AddAnimation(int trackIndex, string animation, bool loop, float delay) {
        if (HasAnimation(animation)) {
            _spineSkin.AnimationState.AddAnimation(trackIndex, animation, loop, delay);
        }
    }
}
