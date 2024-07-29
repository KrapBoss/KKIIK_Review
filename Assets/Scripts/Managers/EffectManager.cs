using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FxInfo
{
    public string NAME;
    public GameObject FX;
}

public class EffectManager : MonoBehaviour
{
    public FxInfo[] fxInfos_moment;

    public FxInfo[] fxInfos_continuous;

    public void GetMomentFx()
    {

    }

    public void GetContinuousFx()
    {

    }
}
