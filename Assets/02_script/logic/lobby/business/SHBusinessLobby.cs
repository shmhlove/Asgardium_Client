﻿using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHBusinessLobby : MonoBehaviour
{
    public void Awake()
    {
        Single.AppInfo.CreateSingleton();
    }

    // 액티브
    // 필터동작시 이벤트 받아 스크롤뷰 재구성, 필터내용은 저장하여 켜질때 그대로 셋팅될 수 있도록
    // 스크롤뷰 구성 - 출력해야할 리스트 서버로 부터 받아 슬롯구성할 수 있도록 스크롤뷰클래스에 전달

    // 패시브

    // 컴퍼니

    private void Start()
    {
        // StartCoroutine(CoroutineForActiveInfomation());
    }

    //private IEnumerator CoroutineForActiveInfomation()
    //{
    //    // 메인 액티브 정보 처리 : 번개 충전 및 시간 처리
    //    return yield null;
    //}
}
