using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHBusinessPatch : MonoBehaviour
{
    void Awake()
    {
        Single.AppInfo.CreateSingleton();
    }
    
    void Start()
    {
        PatchProcess();
    }

    async void PatchProcess()
    {
        await Single.Data.Load(eSceneType.Patch, (pLoadInfo) =>
        {
            if (pLoadInfo.IsSucceed())
            {
                Single.Scene.LoadScene(eSceneType.Login, pCallback: (pReply) =>
                {

                });
            }
            else
            {
                // 업데이트 실패!! 재시도 유도 처리
                var pAlertInfo = new SHUIAlertInfo();
                pAlertInfo.m_strMessage = "업데이트 실패!!\n재시도하시겠습니까?";
                pAlertInfo.m_strTwoLeftBtnLabel = "재시도";
                pAlertInfo.m_strTwoRightBtnLabel = "종료";
                pAlertInfo.m_eButtonType = eAlertButtonType.TwoButton;
                pAlertInfo.m_pCallback = (eSelectBtnType) =>
                {
                    if (eAlertButtonAction.Left_Button == eSelectBtnType)
                    {
                        PatchProcess();
                    }
                    else
                    {
                        SHUtils.GameQuit();
                    }
                };
                Single.Global.GetAlert().Show(pAlertInfo);
            }
        },
        (pProgressInfo) =>
        {
            // UI 표현 처리

            // 콘솔로그 처리
            string strFiles = string.Empty;
            pProgressInfo.m_pLoadingDatas.ForEach((pItem) => strFiles += pItem.m_pLoadDataInfo.m_strName + " / ");
            strFiles = string.IsNullOrEmpty(strFiles) ? "empty" : strFiles;

            Debug.LogFormat("Data Load Progress : <color=yellow>{0}%({1}/{2}), Loading Now : {3}</color>",
                            pProgressInfo.GetPercent(),
                            pProgressInfo.m_iLoadDoneCount,
                            pProgressInfo.m_iTotalDataCount,
                            strFiles);
        });
    }
}
