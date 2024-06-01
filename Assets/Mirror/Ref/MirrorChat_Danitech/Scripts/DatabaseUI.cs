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
    [SerializeField] InputField Input_Pw;

    [SerializeField] Text Input_CheckIdPw_Error;

    [SerializeField] Text Text_DBResult;
    [SerializeField] Text Text_Log;

    [Header("CommectionInfo")]
    [SerializeField] string _ip = "127.0.0.1";
    [SerializeField] string _dbName = "test";
    [SerializeField] string _uid = "root";
    [SerializeField] string _pwd = "1234";
    [SerializeField] string _port = "3306";
    private string _getId = "SELECT * FROM u_info where Nickname =";
    public string GetIdQuery { get => _getId; }

    private bool _isConnectTestComplete; //�߿����� ����
    private static MySqlConnection _dbConnection;
    private string SendQuery(string queryStr, string tableName)
    {
        //����� ���� �������� SELECT�� ���ԵǾ� ������ if Ž
        if (queryStr.Contains("SELECT"))
        {
            DataSet dataSet = OnSelectRequest(queryStr, tableName);

              return DeformatResult(dataSet);
        }
        
            return string.Empty;
         


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
            Debug.LogWarning($"e: {e.ToString()}");
            //Text_Log.text = "DB���� ����";
            Debug.LogWarning("[1.��� ���� ����]");
            return false;
        }
    }


    public void OnClick_IdChk()
    {

    }

    // �г��� üũ
    public void OnSubmit_Login()
    {
        string query = string.Empty;
        if (_isConnectTestComplete == false)
        {
            Text_Log.text = "DB ������ ���� �õ����ּ���";
            return;
        }

        Text_Log.text = string.Empty;
        if (string.IsNullOrWhiteSpace(Input_Id.text) || string.IsNullOrWhiteSpace(Input_Pw.text))
        {

            Input_CheckIdPw_Error.text = "���̵�� ��й�ȣ�� �Է��� �ּ���.";
            return;
        }
        else
        {
            // Query to retrieve the password for the entered ID
            query = $"SELECT Password FROM u_info WHERE Nickname = '{Input_Id.text}'";
        }

        string result = SendQuery(query, "u_info");
        Debug.Log($"Query Result: {result}");

        if (string.IsNullOrEmpty(result))
        {
            Input_CheckIdPw_Error.text = "ID�� �������� �ʽ��ϴ�.";
            return;
        }

        // Assuming SendQuery returns the password as a string
        string retrievedPassword = result.Trim(); // Clean up the result if necessary

        if (retrievedPassword == Input_Pw.text)
        {
            Text_DBResult.text = "�α��� ����!";
            // Add any additional actions on successful login
        }
        else
        {
            Text_DBResult.text = "��й�ȣ�� ��ġ���� �ʽ��ϴ�.";
        }
    }

    public void Ontest()
    {
        Debug.Log("test123");
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
