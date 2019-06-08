using UnityEngine;
using UnityEngine.Networking;

using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHBusinessTest : MonoBehaviour
{
    void Awake()
    {
        Single.AppInfo.CreateSingleton();
    }

    [FuncButton]
    void OnClickSendRequest()
    {
        Single.Network.GET(SHAPIs.SH_API_RETRY_REQUEST, null, (reply) =>
        {
            Single.BusinessGlobal.ShowAlertUI(reply);
        });
    }

    [FuncButton]
    void OnClickRSASample()
    {
        // payload
        var payload = new Dictionary<string, object>();
        payload.Add("iss", "MangoNight.Client.com");
        payload.Add("sub", "MangoNight");
        payload.Add("aud", "Asgardium");
        payload.Add("iat", ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString());
        
        JWT.JsonWebToken.JsonSerializer = new SHCustomJsonSerializer();
        string token = JWT.JsonWebToken.Encode(payload, SHCustomCertificateHandler.CERT_KEY, JWT.JwtHashAlgorithm.HS256);
        Debug.Log(token);
        string value = JWT.JsonWebToken.Decode(token, SHCustomCertificateHandler.CERT_KEY, true);
        Debug.Log(value);
    }

    [FuncButton]
    void OnClickModularTemper()
    {
        int iMod = (3 * 5) % 4;
        int iSubMod1 = 3 % 4;
        int iSubMod2 = 5 % 4;
        int iMulSubMod = iSubMod1 * iSubMod2;
        int iLastMod = iMulSubMod % 4; 

        Debug.LogFormat("(3 * 5) % 4 = {0}", iMod);
        Debug.LogFormat("(3 % 4) = {0}", iSubMod1);
        Debug.LogFormat("(5 % 4) = {0}", iSubMod2);
        Debug.LogFormat("iSubMod1 * iSubMod2 = {0}", iMulSubMod);
        Debug.LogFormat("iMulSubMod % 4 = {0}", iLastMod);
    }

    public int a = 0;
    public int n = 0;
    [FuncButton]
    void OnClickSumReverseSource()
    {
        List<string> ReverseSources = new List<string>();
        for (int i=1; i<n; i++)
        {
            if ((a+i) % n == 0)
            {
                ReverseSources.Add(string.Format("{0} + {1}({4}) % {2} = {3}", a, i, n, (a+i) % n, a+i));
            }
        }

        string strBuff = "";
        for(int i = 0; i < ReverseSources.Count; i++) {
            strBuff += string.Format("{0}\n", ReverseSources[i]);
        }
        Debug.Log(strBuff);
    }

    [FuncButton]
    void OnClickMulReverseSource()
    {
        List<string> ReverseSources = new List<string>();
        for (int i=1; i<n; i++)
        {
            if ((a*i) % n == 1)
            {
                ReverseSources.Add(string.Format("{0} * {1}({4}) % {2} = {3}", a, i, n, (a*i) % n, a*i));
            }
        }

        string strBuff = "";
        for(int i = 0; i < ReverseSources.Count; i++) {
            strBuff += string.Format("{0}\n", ReverseSources[i]);
        }
        Debug.Log(strBuff);
    }

    [FuncButton]
    void OnClickEulerPhiFunction()
    {
        List<int> Zns = new List<int>();
        for (int i=1; i<n; i++)
        {
            if (1 == GetGCD(i, n))
            {
                Zns.Add(i);
            }
        }

        Debug.Log(Zns.Count + " = {" + string.Join(",", Zns) + "}");
    }

    public int GetGCD(int m, int n)
    {
        for (int i=Math.Min(m, n); i>=1; i--)    
        {
            if (m%i==0 && n%i==0)
                return i;
        }

        return -1;
    }

    public class SHSortData
    {
        public int iLevel;
        public int iUnitID;
        public int iSupply;

        public override string ToString()
        {
            return string.Format("{0} : {1}, {2}", iUnitID, iLevel, iSupply);
        }
    }

    private bool? SortConditionForUnitID(SHSortData pValue1, SHSortData pValue2)
    {
        if (pValue1.iUnitID == pValue2.iUnitID)
            return null;

        return pValue1.iUnitID > pValue2.iUnitID;
    }

    private bool? SortConditionForLevel(SHSortData pValue1, SHSortData pValue2)
    {
        if (pValue1.iLevel == pValue2.iLevel)
            return null;

        return pValue1.iLevel < pValue2.iLevel;
    }

    private bool? SortConditionForSupply(SHSortData pValue1, SHSortData pValue2)
    {
        if (pValue1.iSupply == pValue2.iSupply)
            return null;

        return pValue1.iSupply < pValue2.iSupply;
    }

    private List<SHSortData> m_pSortTestData = new List<SHSortData>();
    private void CreateSortData()
    {
        m_pSortTestData.Clear();
        for (int iLoop = 0; iLoop < 10; ++iLoop)
        {
            var pData = new SHSortData
            {
                iLevel = UnityEngine.Random.Range(1, 4),
                iUnitID = UnityEngine.Random.Range(1, 4),
                iSupply = UnityEngine.Random.Range(100, 10001)
            };
            m_pSortTestData.Add(pData);
        }
    }

    [FuncButton]
    void OnClickSort()
    {
        CreateSortData();

        var m_pFilters = new List<Func<SHSortData, SHSortData, bool?>>
        {
            SortConditionForLevel,
            SortConditionForUnitID
        };

        m_pSortTestData.Sort((x, y) =>
        {
            foreach (var pFilter in m_pFilters)
            {
                bool? result = pFilter(x, y);
                if (null != result)
                {
                    return result.Value ? 1 : -1;
                }
            }

            return 1;
        });

        string logBuff = "";
        foreach (var pData in m_pSortTestData)
        {
            logBuff += pData.ToString() + "\n";
        }
        Debug.Log(logBuff);
    }

    [FuncButton]
    void OnClickSort2()
    {
        List<int> pListData = new List<int>();
        for (int iLoop = 0; iLoop < 10; ++iLoop)
        {
            pListData.Add(iLoop);
        }

        pListData.Sort((x, y) =>
        {
            if (0 == x)
                return 1;
            if (0 == y)
                return -1;
            return 0;
        });

        string logBuff = "";
        foreach (var pData in pListData)
        {
            logBuff += pData.ToString() + "\n";
        }
        Debug.Log(logBuff);
    }
}
