using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHResourceManager : SHSingleton<SHResourceManager>
{
    public override void OnInitialize()
    {
        SetDontDestroy();
    }
}
