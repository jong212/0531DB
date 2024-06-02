using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using System;
using System.Data;

public class DatabaseUI : Singleton<DatabaseUI>
{
    [Header("UI")]
    [SerializeField] InputField Input_Id;
    [SerializeField] InputField Input_Pw;
    [SerializeField] Text Input_CheckIdPw_Error;
    [SerializeField] Text Text_DBResult;
    [SerializeField] Text Text_Log;

    [Header("Connection Info")]
    string _ip = "13.124.160.199"; // Ensure this is your server's IP
    string _dbName = "test";
    string _uid = "root";
     string _pwd = "1q2w3e4r!";
   string _port = "3306";

    private bool _isConnectTestComplete;
    private static MySqlConnection _dbConnection;

    private void Awake()
    {
        _isConnectTestComplete = ConnectTest();
    }

    private bool ConnectTest()
    {
        string connectStr = $"Server={_ip};Database={_dbName};Uid={_uid};Pwd={_pwd};Port={_port};";
        Debug.Log($"Connection String: {connectStr}");
        try
        {
            _dbConnection = new MySqlConnection(connectStr);
            _dbConnection.Open();
            _dbConnection.Close();
            Text_Log.text = "Connection successful!";
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Connection error: {e.ToString()}");
            Text_Log.text = "DB connection failed";
            return false;
        }
    }

    public void OnSubmit_Login()
    {
        if (!_isConnectTestComplete)
        {
            Text_Log.text = "Please connect to the DB first.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Input_Id.text) || string.IsNullOrWhiteSpace(Input_Pw.text))
        {
            Input_CheckIdPw_Error.text = "Please enter your ID and password.";
            return;
        }

        string query = $"SELECT Password FROM u_info WHERE Nickname = '{Input_Id.text}'";
        string result = SendQuery(query, "u_info");

        if (string.IsNullOrEmpty(result))
        {
            Input_CheckIdPw_Error.text = "ID does not exist.";
            return;
        }

        string retrievedPassword = ExtractPassword(result);

        if (retrievedPassword == Input_Pw.text)
        {
            Input_CheckIdPw_Error.text = "Login successful!";
        }
        else
        {
            Input_CheckIdPw_Error.text = "Password does not match.";
        }
    }

    private string SendQuery(string queryStr, string tableName)
    {
        if (queryStr.Contains("SELECT"))
        {
            DataSet dataSet = OnSelectRequest(queryStr, tableName);
            return DeformatResult(dataSet);
        }
        return string.Empty;
    }

    public static DataSet OnSelectRequest(string query, string tableName)
    {
        try
        {
            _dbConnection.Open();
            MySqlCommand sqlCmd = new MySqlCommand(query, _dbConnection);
            MySqlDataAdapter adapter = new MySqlDataAdapter(sqlCmd);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet, tableName);
            _dbConnection.Close();
            return dataSet;
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            return null;
        }
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

    private string ExtractPassword(string result)
    {
        string[] lines = result.Split('\n');
        foreach (string line in lines)
        {
            if (line.StartsWith("Password : "))
            {
                return line.Substring("Password : ".Length).Trim();
            }
        }
        return string.Empty;
    }
}
