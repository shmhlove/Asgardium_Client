using UnityEngine;

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class SHSerializer
{
    #region Members
    private int         m_iIndex = 0;
    private byte[]      m_pByteArray;
    private List<byte>  m_pStream = new List<byte>();
    
    public byte[] ByteArray
    {
        get
        {
            if ((null == m_pByteArray) || 
                (m_pStream.Count != m_pByteArray.Length))
                m_pByteArray = m_pStream.ToArray();

            return m_pByteArray;
        }
    }
    #endregion


    #region System Functions
    public SHSerializer() { }
    public SHSerializer(byte[] ByteArray)
    {
        m_pByteArray = ByteArray;
        m_pStream    = new List<byte>(ByteArray);
    }
    #endregion


    #region Interface Functions
    // 인터페이스 : int
    public void Serialize(int iValue)
    {
        m_pStream.AddRange(BitConverter.GetBytes(iValue));
    }
    public int DeserializeInt()
    {
        int iValue = BitConverter.ToInt32(ByteArray, CheckIndex(m_iIndex));
        MoveToByteIndex(sizeof(int));
        return iValue;
    }

    // 인터페이스 : uint
    public void Serialize(uint iValue)
    {
        m_pStream.AddRange(BitConverter.GetBytes(iValue));
    }
    public uint DeserializeuInt()
    {
        uint iValue = BitConverter.ToUInt32(ByteArray, CheckIndex(m_iIndex));
        MoveToByteIndex(sizeof(uint));
        return iValue;
    }

    // 인터페이스 : ulong
    public void Serialize(ulong iValue)
    {
        m_pStream.AddRange(BitConverter.GetBytes(iValue));
    }
    public ulong DeserializeuLong()
    {
        ulong iValue = BitConverter.ToUInt64(ByteArray, CheckIndex(m_iIndex));
        MoveToByteIndex(sizeof(ulong));
        return iValue;
    }
    
    // 인터페이스 : float
    public void Serialize(float fValue)
    {
        m_pStream.AddRange(BitConverter.GetBytes(fValue));
    }
    public float DeserializeFloat()
    {
        float fValue = BitConverter.ToSingle(ByteArray, CheckIndex(m_iIndex));
        MoveToByteIndex(sizeof(float));
        return fValue;
    }
    
    // 인터페이스 : double
    public void Serialize(double dValue)
    {
        m_pStream.AddRange(BitConverter.GetBytes(dValue));

    }
    public double DeserializeDouble()
    {
        double dValue = BitConverter.ToDouble(ByteArray, CheckIndex(m_iIndex));
        MoveToByteIndex(sizeof(double));
        return dValue;
    }

    // 인터페이스 : bool
    public void Serialize(bool bValue)
    {
        m_pStream.AddRange(BitConverter.GetBytes(bValue));
    }
    public bool DeserializeBool()
    {
        bool bValue = BitConverter.ToBoolean(ByteArray, CheckIndex(m_iIndex));
        MoveToByteIndex(sizeof(bool));
        return bValue;
    }

    // 인터페이스 : Vector2
    public void Serialize(Vector2 vValue)
    {
        m_pStream.AddRange(GetBytes(vValue));
    }
    public Vector2 DeserializeVector2()
    {
        Vector2 vValue = new Vector2();
        vValue.x = BitConverter.ToSingle(ByteArray, CheckIndex(m_iIndex));
        MoveToByteIndex(sizeof(float));
        vValue.y = BitConverter.ToSingle(ByteArray, CheckIndex(m_iIndex));
        MoveToByteIndex(sizeof(float));
        return vValue;
    }

    // 인터페이스 : Vector3
    public void Serialize(Vector3 vValue)
    {
        m_pStream.AddRange(GetBytes(vValue));
    }
    public Vector3 DeserializeVector3()
    {
        Vector3 vValue = new Vector3();
        vValue.x = BitConverter.ToSingle(ByteArray, CheckIndex(m_iIndex));
        MoveToByteIndex(sizeof(float));
        vValue.y = BitConverter.ToSingle(ByteArray, CheckIndex(m_iIndex));
        MoveToByteIndex(sizeof(float));
        vValue.z = BitConverter.ToSingle(ByteArray, CheckIndex(m_iIndex));
        MoveToByteIndex(sizeof(float));
        return vValue;
    }

    // 인터페이스 : Vector4
    public void Serialize(Vector4 vValue)
    {
        m_pStream.AddRange(GetBytes(vValue));
    }
    public Vector4 DeserializeVector4()
    {
        Vector4 vValue = new Vector4();
        vValue.x = BitConverter.ToSingle(ByteArray, CheckIndex(m_iIndex));
        MoveToByteIndex(sizeof(float));
        vValue.y = BitConverter.ToSingle(ByteArray, CheckIndex(m_iIndex));
        MoveToByteIndex(sizeof(float));
        vValue.z = BitConverter.ToSingle(ByteArray, CheckIndex(m_iIndex));
        MoveToByteIndex(sizeof(float));
        vValue.w = BitConverter.ToSingle(ByteArray, CheckIndex(m_iIndex));
        MoveToByteIndex(sizeof(float));
        return vValue;
    }

    // 인터페이스 : Quaternion
    public void Serialize(Quaternion qValue)
    {
        m_pStream.AddRange(GetBytes(qValue));
    }

    public Quaternion DeserializeQuaternion()
    {
        Quaternion qValue = new Quaternion();
        qValue.x = BitConverter.ToSingle(ByteArray, CheckIndex(m_iIndex));
        MoveToByteIndex(sizeof(float));
        qValue.y = BitConverter.ToSingle(ByteArray, CheckIndex(m_iIndex));
        MoveToByteIndex(sizeof(float));
        qValue.z = BitConverter.ToSingle(ByteArray, CheckIndex(m_iIndex));
        MoveToByteIndex(sizeof(float));
        qValue.w = BitConverter.ToSingle(ByteArray, CheckIndex(m_iIndex));
        MoveToByteIndex(sizeof(float));
        return qValue;
    }
    
    // 인터페이스 : string
    public void Serialize(string strValue)
    {
        UTF8Encoding pEncoder = new UTF8Encoding();
        Serialize(pEncoder.GetBytes(strValue));
    }
    public string DeserializeString()
    {
        UTF8Encoding pEncoder = new UTF8Encoding();
        return pEncoder.GetString(DeserializeByteArray());
    }

    // 인터페이스 : byte []
    public void Serialize(byte[] pValue)
    {
        m_pStream.AddRange(BitConverter.GetBytes(pValue.Length));
        m_pStream.AddRange(pValue);
    }
    public byte[] DeserializeByteArray()
    {
        int iLenght = BitConverter.ToInt32(ByteArray, CheckIndex(m_iIndex));
        MoveToByteIndex(sizeof(int));

        byte[] pBytes = new byte[iLenght];
        for (int iLoop = 0; iLoop < iLenght; ++iLoop)
        {
            pBytes[iLoop] = ByteArray[CheckIndex(m_iIndex)];
            MoveToByteIndex(sizeof(byte));
        }

        return pBytes;
    }
    #endregion


    #region Utility Functions
    // 유틸 : Vector 혹은 Quaternion 시리얼라이즈
    byte[] GetBytes(Vector2 vValue)
    {
        List<byte> pBytes = new List<byte>(sizeof(float) * 2);
        pBytes.AddRange(BitConverter.GetBytes(vValue.x));
        pBytes.AddRange(BitConverter.GetBytes(vValue.y));
        return pBytes.ToArray();
    }

    byte[] GetBytes(Vector3 vValue)
    {
        List<byte> pBytes = new List<byte>(sizeof(float) * 3);
        pBytes.AddRange(BitConverter.GetBytes(vValue.x));
        pBytes.AddRange(BitConverter.GetBytes(vValue.y));
        pBytes.AddRange(BitConverter.GetBytes(vValue.z));
        return pBytes.ToArray();
    }

    byte[] GetBytes(Vector4 vValue)
    {
        List<byte> pBytes = new List<byte>(sizeof(float) * 4);
        pBytes.AddRange(BitConverter.GetBytes(vValue.x));
        pBytes.AddRange(BitConverter.GetBytes(vValue.y));
        pBytes.AddRange(BitConverter.GetBytes(vValue.z));
        pBytes.AddRange(BitConverter.GetBytes(vValue.w));
        return pBytes.ToArray();
    }

    byte[] GetBytes(Quaternion qValue)
    {
        List<byte> pBytes = new List<byte>(sizeof(float) * 4);
        pBytes.AddRange(BitConverter.GetBytes(qValue.x));
        pBytes.AddRange(BitConverter.GetBytes(qValue.y));
        pBytes.AddRange(BitConverter.GetBytes(qValue.z));
        pBytes.AddRange(BitConverter.GetBytes(qValue.w));
        return pBytes.ToArray();
    }

    // 유틸 : 인덱스 이동
    void MoveToByteIndex(int iMoveIndex)
    {
        m_iIndex += iMoveIndex;
        CheckIndex(m_iIndex);
    }

    int CheckIndex(int iIndex)
    {
        if (ByteArray.Length < m_iIndex)
        {
            // Out Of Memoory시 로그를 남기고, 마지막 10바이트 읽어서 보여주려고
            string strRightByte = string.Empty;
            for(int iLoopBytes = 0, iLoop = ByteArray.Length - 10; iLoop < ByteArray.Length; ++iLoop, ++iLoopBytes)
            {
                strRightByte += ByteArray[iLoop].ToString();
            }

            Debug.LogError(
                string.Format("[LSH] Deserialize ArgumentOutOfRangeException!!(Length : {0}, Index : {1}, Right10 : {2})", 
                ByteArray.Length, m_iIndex, strRightByte));
        }

        return m_iIndex;
    }
    #endregion


    #region Example Functions
    public static void Example()
    {
        Vector2 vVector2    = UnityEngine.Random.insideUnitCircle;
        Vector3 vVector3    = UnityEngine.Random.onUnitSphere;
        Quaternion qQuater  = UnityEngine.Random.rotation;
        float fFloat        = UnityEngine.Random.value;
        int iInt            = UnityEngine.Random.Range(0, 10000);
        double dDouble      = (double)UnityEngine.Random.Range(0, 10000);
        string strString    = "Brundle Fly";
        bool bBool          = UnityEngine.Random.value < 0.5f ? true : false;

        Debug.Log("--- Before ---");
        Debug.Log(string.Format("vVector2 : {0}, vVector3 : {1}, qQuater : {2}, fFloat : {3}, iInt : {4}, dDouble : {5}, strString : {6}, bBool : {7}",
            vVector2, vVector3, qQuater, fFloat, iInt, dDouble, strString, bBool));

        Debug.Log("--- Serialize ---");
        SHSerializer pSerializer = new SHSerializer();
        pSerializer.Serialize(vVector2);
        pSerializer.Serialize(vVector3);
        pSerializer.Serialize(qQuater);
        pSerializer.Serialize(fFloat);
        pSerializer.Serialize(iInt);
        pSerializer.Serialize(dDouble);
        pSerializer.Serialize(strString);
        pSerializer.Serialize(bBool);

        Debug.Log("--- Deserialize ---");
        SHSerializer pDeSerializer = new SHSerializer(pSerializer.ByteArray);
        vVector2    = pDeSerializer.DeserializeVector2();
        vVector3    = pDeSerializer.DeserializeVector3();
        qQuater     = pDeSerializer.DeserializeQuaternion();
        fFloat      = pDeSerializer.DeserializeFloat();
        iInt        = pDeSerializer.DeserializeInt();
        dDouble     = pDeSerializer.DeserializeDouble();
        strString   = pDeSerializer.DeserializeString();
        bBool       = pDeSerializer.DeserializeBool();

        Debug.Log("--- After ---");
        Debug.Log(string.Format("vVector2 : {0}, vVector3 : {1}, qQuater : {2}, fFloat : {3}, iInt : {4}, dDouble : {5}, strString : {6}, bBool : {7}",
            vVector2, vVector3, qQuater, fFloat, iInt, dDouble, strString, bBool));
    }
    #endregion
}