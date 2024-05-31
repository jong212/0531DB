using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using System;
public class DatabaseUI : MonoBehaviour
{
   [Header("UI")]
   [SerializeField] InputField Input_Query;
   [SerializeField] Text Text_DBResult;
   [SerializeField] Text Text_Log;

    [Header("CommectionInfo")]
    [SerializeField] string _ip = "127.0.0.1";
    [SerializeField] string _dbName = "test";
    [SerializeField] string _uid = "root";
    [SerializeField] string _pwd = "1234";
    [SerializeField] string _port = "3307";

    private bool _isConnectTestComplete; //중요하진 않음
    bool ConnectTest()
    {
        string connectStr = $"Server={_ip};Database={_dbName};Uid={_uid};Pwd={_pwd};Port={_port};";
        try
        {
            using (MySqlConnection conn = new MySqlConnection(connectStr))
            {
                _dbConnection = conn;
                conn.Open();
            }
            Text_Log.text = "성공";
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"e: {e.ToString()}");
            Text_Log.text = "DB연결 실패";
            return false;
        }
    }

    private static MySqlConnection _dbConnection;

    public void OnClick_TestDBConnect()
    {
        _isConnectTestComplete = ConnectTest();
       
    }

    public void OnSubmit_SendQuery()
    {
    }

    public void OnClick_OpenDatabaseUI()
    {
        this.gameObject.SetActive(true);
    }

    public void OnClick_CloseDatabaseUI()
    {
        this.gameObject.SetActive(false);
    }

}
