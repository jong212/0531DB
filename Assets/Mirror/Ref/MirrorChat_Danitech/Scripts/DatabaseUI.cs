using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using Unity.VisualScripting;
public class DatabaseUI : Singleton<DatabaseUI>
{
    [Header("UI")]

    [SerializeField] InputField Input_Id;
    [SerializeField] Text Input_CheckId;
    [SerializeField] Text Text_DBResult;
    [SerializeField] Text Text_Log;

    [Header("CommectionInfo")]
    [SerializeField] string _ip = "127.0.0.1";
    [SerializeField] string _dbName = "test";
    [SerializeField] string _uid = "root";
    [SerializeField] string _pwd = "1234";
    [SerializeField] string _port = "3307";
    private string _getId = "SELECT * FROM members where nickname =";
    public string GetIdQuery { get => _getId; }

    private bool _isConnectTestComplete; //�߿����� ����
    private static MySqlConnection _dbConnection;
    private void SendQuery(string queryStr, string tableName)
    {
        //����� ���� �������� SELECT�� ���ԵǾ� ������ if Ž
        if (queryStr.Contains("SELECT"))
        {
            DataSet dataSet = OnSelectRequest(queryStr, tableName);
            Debug.Log(dataSet);
            Text_DBResult.text = DeformatResult(dataSet);
        }
        else // ���ٸ� INSERT �Ǵ� UPDATE ���� ����
        {
            if (Input_CheckId.text == "")
            {
                Input_CheckId.text = "���̵� �Է��� �ּ���.";
            }

            //Text_DBResult.text = OnInsertOnUpdateRequest(queryStr) ? "����" : "����";
        }

    }
    public static bool OnInsertOnUpdateRequest(string query)
    {
        try
        {
            MySqlCommand sqlCommand = new MySqlCommand();
            sqlCommand.Connection = _dbConnection;
            sqlCommand.CommandText = query;

            _dbConnection.Open();
            sqlCommand.ExecuteNonQuery();
            _dbConnection.Close();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    private void Awake()
    {
        _isConnectTestComplete = ConnectTest();
        //this.gameObject.SetActive(false);

    }

    private string DeformatResult(DataSet dataSet)
    {
        string resultStr = string.Empty;
        foreach (DataTable table in dataSet.Tables)
        {
            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn column in table.Columns)
                {
                    resultStr += $"{column.ColumnName} : {row[column]}\n";
                }
            }
        }
        Debug.Log(resultStr);
        return resultStr;
    }
    public static DataSet OnSelectRequest(string query, string tableName)
    {
        try
        {
            _dbConnection.Open();
            MySqlCommand sqlCmd = new MySqlCommand();
            sqlCmd.Connection = _dbConnection;
            sqlCmd.CommandText = query;
            MySqlDataAdapter sd = new MySqlDataAdapter(sqlCmd);
            DataSet dataSet = new DataSet();
            sd.Fill(dataSet, tableName);
            _dbConnection.Close();
            return dataSet;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return null;
        }
    }
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
            Text_Log.text = "����";
            return true;
        }
        catch (Exception e)
        {
            //Debug.LogWarning($"e: {e.ToString()}");
            //Text_Log.text = "DB���� ����";
            Debug.LogWarning("[1.��� ���� ����]");
            return false;
        }
    }


    public void OnClick_IdChk()
    {

    }

    // ������ ��ư Ŭ�� �� Ÿ�� �Լ� 
    public void OnSubmit_SendQuery()
    {
        if (_isConnectTestComplete == false)
        {
            Text_Log.text = "DB ������ ���� �õ����ּ���";
            return;
        }
        Debug.Log("������ Ŭ��");
        Text_Log.text = string.Empty;
        string query = string.IsNullOrWhiteSpace(Input_Id.text) ? "" : $"{GetIdQuery + Input_Id.text}";
        Debug.Log(query);
        SendQuery(query, "members");
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
