using UnityEngine;
using UObject = UnityEngine.Object;

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public partial class SHResourceData : SHBaseData
{
    private Dictionary<string, UObject> m_dicResources = new Dictionary<string, UObject>();
    
    public override void OnInitialize()
    {
        m_dicResources.Clear();
    }
    
    public override void OnFinalize()
    {
        m_dicResources.Clear();
    }
    
    public override Dictionary<string, SHLoadData> GetLoadList(eSceneType eType)
    {
        var dicLoadList  = new Dictionary<string, SHLoadData>();
        var PreloadTable = Single.Table.GetTable<JsonPreloadResources>();
        foreach (var strValue in PreloadTable.GetData(eType))
        {
            if (true == m_dicResources.ContainsKey(strValue.ToLower()))
                continue;
            
            dicLoadList.Add(strValue, CreateLoadInfo(strValue));
        };

        return dicLoadList;
    }
    
    public override IEnumerator Load(SHLoadData pInfo, Action<string, SHLoadStartInfo> pStart, 
                                                       Action<string, SHLoadEndInfo> pDone)
    {
        pStart(pInfo.m_strName, new SHLoadStartInfo());

        if (true == m_dicResources.ContainsKey(pInfo.m_strName.ToLower()))
        {
            pDone(pInfo.m_strName, new SHLoadEndInfo(null));
            yield break;
        }

        var pTable = Single.Table.GetTable<JsonResources>(false);
        var pResourceInfo = pTable.GetResouceInfo(pInfo.m_strName);
        if (null == pResourceInfo)
        {
            Debug.LogFormat("[LSH] 리소스 테이블에 {0}가 없습니다.(파일이 없거나 리소스 리스팅이 안되었음)", pInfo.m_strName);
            pDone(pInfo.m_strName, new SHLoadEndInfo(eErrorCode.Resources_NotExsitInTable));
            yield break;
        }

        // Async Load
        //Single.Coroutine.Routine(() => 
        //{
        //    var pResource = GetResources(pInfo.m_strName);
        //    if (null == pResource)
        //        pDone(pInfo.m_strName, new SHLoadEndInfo(eErrorCode.Resource_Load_Fail));
        //    else
        //        pDone(pInfo.m_strName, new SHLoadEndInfo(eErrorCode.Succeed));

        //}, LoadAsync<UObject>(pResourceInfo));

        // Sync Load
        var pObject = LoadSync<UObject>(pResourceInfo);
        if (null == pObject)
            pDone(pInfo.m_strName, new SHLoadEndInfo(eErrorCode.Resources_LoadFailed));
        else
            pDone(pInfo.m_strName, new SHLoadEndInfo(null));

        yield return null;
    }
    
    public UObject GetResources(string strFileName)
    {
        return GetResources<UObject>(strFileName);
    }

    public T GetResources<T>(string strFileName) where T : UObject
    {
        if (true == string.IsNullOrEmpty(strFileName))
            return null;
        
        strFileName = Path.GetFileNameWithoutExtension(strFileName);
        if (false == m_dicResources.ContainsKey(strFileName.ToLower()))
        {
            var Table = Single.Table.GetTable<JsonResources>(false);
            var pInfo = Table.GetResouceInfo(strFileName);
            if (null == pInfo)
            {
                Debug.Log(string.Format("[LSH] 리소스 테이블에 {0}가 없습니다.(파일이 없거나 리소스 리스팅이 안되었음)", strFileName));
                return null;
            }

            return LoadSync<T>(pInfo);
        }

        return m_dicResources[strFileName.ToLower()] as T;
    }
    
    public GameObject GetPrefab(string strName)
    {
        return GetResources<GameObject>(strName);
    }
    
    public GameObject GetGameObject(string strName)
    {
        return Instantiate<GameObject>(GetPrefab(strName));
    }
    
    public Texture GetTexture(string strName)
    {
        return GetResources<Texture>(strName);
    }
    
    public Texture2D GetTexture2D(string strName)
    {
        return GetResources<Texture2D>(strName);
    }
    
    public Sprite GetSprite(string strName)
    {
        Texture2D pTexture = GetTexture2D(strName);
        if (null == pTexture)
            return null;

        return Sprite.Create(pTexture, new Rect(0.0f, 0.0f, pTexture.width, pTexture.height), new Vector2(0.5f, 0.5f));
    }
    
    public Texture2D GetDownloadTexture(string strURL)
    {
        WWW pWWW = Single.Coroutine.WWWOfSync(new WWW(strURL));
        if (false == string.IsNullOrEmpty(pWWW.error))
            return null;

        return pWWW.texture;
    }
    public void GetDownloadTexture(string strURL, Action<Texture2D> pCallback)
    {
        if (null == pCallback)
            return;

        Single.Coroutine.WWW((pWWW) =>
        {
            if (false == string.IsNullOrEmpty(pWWW.error))
                pCallback(null);
            else
                pCallback(pWWW.texture);
        }, new WWW(strURL));
    }
    
    public AnimationClip GetAniamiton(string strName)
    {
        return GetResources<AnimationClip>(strName);
    }
    
    public Material GetMaterial(string strName)
    {
        return GetResources<Material>(strName);
    }
    
    public AudioClip GetSound(string strName)
    {
        return GetResources<AudioClip>(strName);
    }
    
    public TextAsset GetTextAsset(string strName)
    {
        return GetResources<TextAsset>(strName);
    }
    
    public T GetComponentByObject<T>(string strName)
    {
        GameObject pObject = GetGameObject(strName);
        if (null == pObject)
            return default(T);

        return pObject.GetComponent<T>();
    }
    
    public T Instantiate<T>(T pPrefab) where T : UObject
    {
        if (true == Single.AppInfo.m_bIsAppQuit)
            return null;

        if (null == pPrefab)
        {
            Debug.LogErrorFormat("[LSH] 오브젝트 복사중 Null 프리팹이 전달되었습니다!!(Type : {0})", typeof(T));
            return default(T);
        }

#if UNITY_EDITOR
        DateTime pStartTime = DateTime.Now;
#endif

        T pGameObject    = UObject.Instantiate<T>(pPrefab);
        var strName      = pGameObject.name;
        pGameObject.name = strName.Substring(0, strName.IndexOf("(Clone)"));
        
#if UNITY_EDITOR
        Single.AppInfo.SetLoadResource(string.Format("Instantiate : {0}({1}sec)", pPrefab.name, ((DateTime.Now - pStartTime).TotalMilliseconds / 1000.0f)));
#endif

        return pGameObject;
    }
    
    // 유틸 : 로드정보 만들기
    SHLoadData CreateLoadInfo(string strName)
    {
        return new SHLoadData()
        {
            m_eDataType = eDataType.Resources,
            m_strName = strName,
            m_pLoadFunc = Load,
            m_pLoadOkayTrigger = () =>
            {
                // 테이블 데이터를 먼저 로드하고 리소스 로드할 수 있도록 트리거 설정
                return Single.Data.IsLoadDone(eDataType.LocalTable);
            },
        };
    }
    
    // 유틸 : 어싱크로 리소스 로드하기
    IEnumerator LoadAsync<T>(SHResourcesInfo pTable) where T : UnityEngine.Object
    {
        if (null == pTable)
            yield break;

        if (true == m_dicResources.ContainsKey(pTable.m_strName.ToLower()))
            yield break;
        
#if UNITY_EDITOR
        DateTime pStartTime = DateTime.Now;
#endif

        UObject pObject = null;
        //var pBundleData = Single.AssetBundle.GetBundleData(Single.Table.GetBundleInfoToResourceName(pTable.m_strName));
        //if (null != pBundleData)
        //{
        //    var pRequest = pBundleData.m_pBundle.LoadAssetAsync<T>(pTable.m_strName);
        //    yield return pRequest;

        //    pObject = pRequest.asset;
        //}
        //else
        {
            var pRequest = Resources.LoadAsync<T>(pTable.m_strPath);
            yield return pRequest;

            pObject = pRequest.asset;
        }
        
        if (null == pObject)
        {
            Debug.LogError(string.Format("[LSH] {0}을 로드하지 못했습니다!!\n리소스 테이블에는 목록이 있으나 실제 파일은 없을 수도 있습니다.", pTable.m_strPath));
            yield break;
        }
        
#if UNITY_EDITOR
        Single.AppInfo.SetLoadResource(string.Format("Load : {0}({1}sec)", pTable.m_strName, ((DateTime.Now - pStartTime).TotalMilliseconds / 1000.0f)));
#endif

        m_dicResources.Add(pTable.m_strName.ToLower(), pObject);
    }

    // 유틸 : 싱크로 리소스 로드하기
    T LoadSync<T>(SHResourcesInfo pTable) where T : UObject
    {
        if (null == pTable)
            return null;

        if (true == m_dicResources.ContainsKey(pTable.m_strName.ToLower()))
            return m_dicResources[pTable.m_strName.ToLower()] as T;
        
#if UNITY_EDITOR
        DateTime pStartTime = DateTime.Now;
#endif

        T pObject       = null;
        //var pBundleData = Single.AssetBundle.GetBundleData(Single.Table.GetBundleInfoToResourceName(pTable.m_strName));
        //if (null != pBundleData)
        //    pObject = pBundleData.m_pBundle.LoadAsset<T>(pTable.m_strName);
        //else
            pObject = Resources.Load<T>(pTable.m_strPath);

        if (null == pObject)
        {
            Debug.LogError(string.Format("[LSH] {0}을 로드하지 못했습니다!!\n리소스 테이블에는 목록이 있으나 실제 파일은 없을 수 있습니다.", pTable.m_strPath));
            return null;
        }

#if UNITY_EDITOR
        Single.AppInfo.SetLoadResource(string.Format("Load : {0}({1}sec)", pTable.m_strName, ((DateTime.Now - pStartTime).TotalMilliseconds / 1000.0f)));
#endif

        if (true == m_dicResources.ContainsKey(pTable.m_strName.ToLower()))
            m_dicResources[pTable.m_strName.ToLower()] = pObject;
        else
            m_dicResources.Add(pTable.m_strName.ToLower(), pObject);

        return pObject;
    }
}