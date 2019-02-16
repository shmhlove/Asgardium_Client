using UnityEngine;
using UnityEngine.Networking;

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

using UObject = UnityEngine.Object;

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
    
    public async override void GetLoadList(eSceneType eType, Action<Dictionary<string, SHLoadData>> pCallback)
    {
        var pTable = await Single.Table.GetTable<JsonPreloadResources>();
        var dicLoadList  = new Dictionary<string, SHLoadData>();
        foreach (var strValue in pTable.GetData(eType))
        {
            if (true == m_dicResources.ContainsKey(strValue.ToLower()))
                continue;
            
            dicLoadList.Add(strValue, CreateLoadInfo(strValue));
        };
        
        pCallback(dicLoadList);
    }
    
    public async override void Load
    (
        SHLoadData pInfo, 
        Action<string, SHLoadStartInfo> pStart, 
        Action<string, SHLoadEndInfo> pDone
    )
    {
        pStart(pInfo.m_strName, new SHLoadStartInfo());

        if (true == m_dicResources.ContainsKey(pInfo.m_strName.ToLower()))
        {
            pDone(pInfo.m_strName, new SHLoadEndInfo(eErrorCode.Succeed));
            return;
        }

        var pTable = await Single.Table.GetTable<JsonResources>();
        var pResourceInfo = pTable.GetResouceInfo(pInfo.m_strName);
        if (null == pResourceInfo)
        {
            Debug.LogFormat("[LSH] 리소스 테이블에 {0}가 없습니다.(파일이 없거나 리소스 리스팅이 안되었음)", pInfo.m_strName);
            pDone(pInfo.m_strName, new SHLoadEndInfo(eErrorCode.Resources_NotExsitInTable));
            return;
        }
        
        LoadAsync<UObject>(pResourceInfo, (pObject) => 
        {
            if (null == pObject)
                pDone(pInfo.m_strName, new SHLoadEndInfo(eErrorCode.Resources_LoadFailed));
            else
                pDone(pInfo.m_strName, new SHLoadEndInfo(eErrorCode.Succeed));
        });
    }

    private SHLoadData CreateLoadInfo(string strName)
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

    private void LoadAsync<T>(SHResourcesInfo pTable, Action<T> pCallback) where T : UnityEngine.Object
    {
        if (null == pTable)
        {
            pCallback(null);
            return;
        }

        if (true == m_dicResources.ContainsKey(pTable.m_strName.ToLower()))
        {
            pCallback(m_dicResources[pTable.m_strName.ToLower()] as T);
            return;
        }

        Action<T> pLoadedAction = (pObject) =>
        {
            if (null == pObject)
            {
                Debug.LogError(string.Format("[LSH] {0}을 로드하지 못했습니다!!\n리소스 테이블에는 목록이 있으나 실제 파일은 없을 수도 있습니다.", pTable.m_strPath));
            }
            else
            {
                if (false == m_dicResources.ContainsKey(pTable.m_strName.ToLower()))
                {
                    m_dicResources.Add(pTable.m_strName.ToLower(), pObject);
                }
                else
                {
                    m_dicResources[pTable.m_strName.ToLower()] = pObject;
                }
            }

            pCallback(pObject);
        };

        DateTime pStartTime = DateTime.Now;

        //var pBundleData = Single.AssetBundle.GetBundleData(Single.Table.GetBundleInfoToResourceName(pTable.m_strName));
        //if (null != pBundleData)
        //{
        //    Single.Coroutine.Async(pBundleData.m_pBundle.LoadAssetAsync<T>(pTable.m_strName), (pRequest) =>
        //    {
        //        pLoadedAction((pRequest as ResourceRequest).asset);
        //    });
        //}
        //else
        {
            Single.Coroutine.Async(Resources.LoadAsync<T>(pTable.m_strPath), (pRequest) =>
            {
                var pAsset = (pRequest as ResourceRequest).asset;
                if (null != pAsset)
                {
                    Single.AppInfo.SetLoadResource(string.Format("Load : {0}({1}sec)",
                        pTable.m_strName, ((DateTime.Now - pStartTime).TotalMilliseconds / 1000.0f)));
                }

                pLoadedAction(pAsset as T);
            });
        }
    }

    private T LoadSync<T>(SHResourcesInfo pTable) where T : UObject
    {
        if (null == pTable)
            return null;

        if (true == m_dicResources.ContainsKey(pTable.m_strName.ToLower()))
            return m_dicResources[pTable.m_strName.ToLower()] as T;

        DateTime pStartTime = DateTime.Now;

        T pObject = null;
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

        Single.AppInfo.SetLoadResource(string.Format("Load : {0}({1}sec)", pTable.m_strName, ((DateTime.Now - pStartTime).TotalMilliseconds / 1000.0f)));

        if (true == m_dicResources.ContainsKey(pTable.m_strName.ToLower()))
            m_dicResources[pTable.m_strName.ToLower()] = pObject;
        else
            m_dicResources.Add(pTable.m_strName.ToLower(), pObject);

        return pObject;
    }

    public async Task<UObject> GetResources(string strFileName)
    {
        return await GetResources<UObject>(strFileName);
    }

    public async Task<T> GetResources<T>(string strFileName) where T : UObject
    {
        var pPromise = new TaskCompletionSource<T>();

        if (true == string.IsNullOrEmpty(strFileName))
        {
            Debug.Log(string.Format("[LSH] 전달받은 리소스 파일 이름이 null 입니다."));
            pPromise.TrySetResult(null);
            return await pPromise.Task;
        }

        strFileName = Path.GetFileNameWithoutExtension(strFileName);
        if (true == m_dicResources.ContainsKey(strFileName.ToLower()))
        {
            pPromise.TrySetResult(m_dicResources[strFileName.ToLower()] as T);
            return await pPromise.Task;
        }

        var pTable = await Single.Table.GetTable<JsonResources>();
        var pInfo = pTable.GetResouceInfo(strFileName);
        if (null == pInfo)
        {
            Debug.Log(string.Format("[LSH] 리소스 테이블에 {0}가 없습니다.(파일이 없거나 리소스 리스팅이 안되었음)", strFileName));
            pPromise.TrySetResult(null);
        }
        else
        {
            LoadAsync<T>(pInfo, (pResource) =>
            {
                pPromise.TrySetResult(pResource);
            });
        }

        return await pPromise.Task;
    }

    public T Instantiate<T>(T pPrefab) where T : UObject
    {
        if (null == pPrefab)
        {
            Debug.LogErrorFormat("[LSH] 오브젝트 복사중 null 프리팹이 전달되었습니다!!(Type : {0})", typeof(T));
            return default;
        }

        DateTime pStartTime = DateTime.Now;

        T pGameObject = UObject.Instantiate<T>(pPrefab);
        var strName = pGameObject.name;
        pGameObject.name = strName.Substring(0, strName.IndexOf("(Clone)"));

        Single.AppInfo.SetLoadResource(string.Format("Instantiate : {0}({1}sec)", pPrefab.name, SHUtils.GetElapsedSecond(pStartTime)));

        return pGameObject;
    }

    public async Task<GameObject> GetGameObject(string strName)
    {
        var pPromise = new TaskCompletionSource<GameObject>();
        {
            var pObject = await GetResources<GameObject>(strName);
            pPromise.TrySetResult(Instantiate<GameObject>(pObject));
        }
        return await pPromise.Task;
    }
    
    public async Task<Texture> GetTexture(string strName)
    {
        return await GetResources<Texture>(strName);
    }
    
    public async Task<Texture2D> GetTexture2D(string strName)
    {
        return await GetResources<Texture2D>(strName);
    }
    
    public async Task<Sprite> GetSprite(string strName)
    {
        var pTexture = await GetTexture2D(strName);
        if (null == pTexture)
        {
            return default;
        }
        else
        {
            return Sprite.Create(pTexture, new Rect(0.0f, 0.0f, pTexture.width, pTexture.height), new Vector2(0.5f, 0.5f));
        }
    }
    
    public async Task<Texture2D> GetDownloadTexture(string strURL)
    {
        var pPromise = new TaskCompletionSource<Texture2D>();
        
        Single.Coroutine.WWW(UnityWebRequestTexture.GetTexture(strURL), (pWWW) => 
        {
            pPromise.TrySetResult(((DownloadHandlerTexture)pWWW.downloadHandler).texture);
        });

        return await pPromise.Task;
    }

    public async Task<AnimationClip> GetAniamiton(string strName)
    {
        return await GetResources<AnimationClip>(strName);
    }
    
    public async Task<Material> GetMaterial(string strName)
    {
        return await GetResources<Material>(strName);
    }
    
    public async Task<AudioClip> GetSound(string strName)
    {
        return await GetResources<AudioClip>(strName);
    }
    
    public async Task<TextAsset> GetTextAsset(string strName)
    {
        return await GetResources<TextAsset>(strName);
    }
    
    public async Task<T> GetComponentByObject<T>(string strName)
    {
        var pPromise = new TaskCompletionSource<T>();
        
        var pObject = await GetGameObject(strName);
        if (null == pObject)
            pPromise.TrySetResult(default);
        else
            pPromise.TrySetResult(pObject.GetComponent<T>());
        
        return await pPromise.Task;
    }
}