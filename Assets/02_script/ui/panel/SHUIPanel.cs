using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHUIPanel : MonoBehaviour
{
    virtual public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
