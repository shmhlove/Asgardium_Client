using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHUIActiver : MonoBehaviour
{
    public void OnEventActive(bool bIsActive)
    {
        NGUITools.SetActive(gameObject, bIsActive);
    }

    public void OnEventDisable(bool bIsDisable)
    {
        NGUITools.SetActive(gameObject, !bIsDisable);
    }
}
