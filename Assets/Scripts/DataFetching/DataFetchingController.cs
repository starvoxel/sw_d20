using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class DataFetchingController : SingletonBehaviour<DataFetchingController>
{
	#region Fields & Properties
	//const
	public const string INCORRECT_PASSWORD = "Incorrect Password";

	//structs
    public struct sFetchRequestInfo
    {
        public string SpreadsheetID;
        public string Password;
        public sConnectInfo[] ConnectionInfo;

        public sFetchRequestInfo(string spreadsheetID, string password, params sConnectInfo[] connectionInfo)
        {
            SpreadsheetID = spreadsheetID;
            Password = password;
            ConnectionInfo = connectionInfo;
        }

        public bool IsInitialized
        {
            get { return !string.IsNullOrEmpty(SpreadsheetID) && ConnectionInfo != null && ConnectionInfo.Length > 0; }
        }
    }

	public struct sConnectInfo
	{
		public delegate void BeginCallbackDelegate(string worksheetName);
		public delegate void UpdateCallbackDelegate(string worksheetName, float progress);
		public delegate void CompletionCallbackDelegate(string worksheetName, JsonData[] data, string error = null);

		public string WorksheetName;
		public BeginCallbackDelegate BeginCallback;
		public UpdateCallbackDelegate UpdateCallback;
		public CompletionCallbackDelegate CompletionCallback;

		public sConnectInfo(string worksheetName, CompletionCallbackDelegate completionCallback, UpdateCallbackDelegate updateCallback = null, BeginCallbackDelegate beginCallback = null)
		{
			WorksheetName = worksheetName;
			CompletionCallback = completionCallback;
			UpdateCallback = updateCallback;
			BeginCallback = beginCallback;
		}
	}

	//public

	//protected
	[SerializeField] protected string m_WebServiceURL = string.Empty;
	[SerializeField] protected float m_MaxWaitTime = 30.0f;
    protected Queue<sFetchRequestInfo> m_RequestQueue = new Queue<sFetchRequestInfo>();
    sFetchRequestInfo m_CurrentRequest;



	//private
	JsonData[] m_Data;
	#endregion

    #region Unity Methods
    protected virtual void Update()
    {
        if (m_RequestQueue.Count > 0 && !m_CurrentRequest.IsInitialized)
        {
            m_CurrentRequest = m_RequestQueue.Dequeue();
            StartCoroutine(FetchData(m_CurrentRequest));
        }
    }

	protected virtual void OnDestroy()
	{
		this.StopAllCoroutines();
	}
	#endregion

    #region Public Methods
    /// <summary>
    /// Fethes the Google Sheets data from the provided sheets.
    /// </summary>
    /// <param name="spreadsheetID">Spreadsheet ID found in the URL of the spreadsheet.</param>
    /// <param name="password">Password for the provided spreadsheet.</param>
    /// <param name="connectInfo">Data set info with callbacks for completion.</param>
    /// <returns>Returns true request is properly added to the request queue.</returns>
    public bool Fetch(string spreadsheetID, string password, params sConnectInfo[] connectInfo)
    {
        return Fetch(new sFetchRequestInfo(spreadsheetID, password, connectInfo));
    }

    /// <summary>
    /// Fethes the Google Sheets data from the provided sheets.
    /// </summary>
    /// <param name="request">Request info to fetch.</param>
    /// <returns>Returns true request is properly added to the request queue.</returns>
    public bool Fetch(sFetchRequestInfo request)
    {
        if (request.IsInitialized)
        {
            m_RequestQueue.Enqueue(request);

            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Fetches the data from Google Sheets and returns an array of JSON objects.
    /// </summary>
    /// <param name="request">Request info</param>
    protected IEnumerator FetchData(sFetchRequestInfo request)
    {
        for (int connectIndex = 0; connectIndex < request.ConnectionInfo.Length; ++connectIndex)
        {
            sConnectInfo info = request.ConnectionInfo[connectIndex];

            string connectionString = m_WebServiceURL + "?ssid=" + request.SpreadsheetID + "&sheet=" + info.WorksheetName + "&pass=" + request.Password + "&action=GetData";

            WWW www = new WWW(connectionString);

            float elapsedTime = 0.0f;

            while (!www.isDone)
            {
                if (elapsedTime == 0)
                {
                    if (info.BeginCallback != null)
                    {
                        info.BeginCallback(info.WorksheetName);
                    }
                }
                else
                {
                    if (info.UpdateCallback != null)
                    {
                        info.UpdateCallback(info.WorksheetName, www.progress);
                    }
                }

                elapsedTime += Time.deltaTime;
                if (elapsedTime >= m_MaxWaitTime)
                {
                    break;
                }

                yield return null;
            }

            if (!www.isDone || !string.IsNullOrEmpty(www.error))
            {
                if (info.CompletionCallback != null)
                {
                    info.CompletionCallback(info.WorksheetName, null, www.error);
                }

                yield break;
            }
			else
			{
	            string response = www.text;

	            if (response.Contains(INCORRECT_PASSWORD))
	            {
	                if (info.CompletionCallback != null)
	                {
	                    info.CompletionCallback(info.WorksheetName, null, "Incorrect password provided");
	                }
	            }
				
				bool invalidData = false;

	            try
	            {
	                m_Data = JsonMapper.ToObject<JsonData[]>(response);
	            }
	            catch
	            {
	                if (info.CompletionCallback != null)
	                {
	                    info.CompletionCallback(info.WorksheetName, null, "Invalid data");
	                }

					invalidData = true;
	            }

	            if (!invalidData && info.CompletionCallback != null)
	            {
	                info.CompletionCallback(info.WorksheetName, m_Data);
	            }
			}
        }

		m_CurrentRequest = new sFetchRequestInfo();
    }

    //If we ever want to do any kind of uploading we can put this back in
    /*proteted IEnumerator SendData(string ballName, float collisionMagnitude)
    {
        if (!saveToGS)
            yield break;
		
        string connectionString = 	webServiceUrl +
            "?ssid=" + spreadsheetId +
                "&sheet=" + statisticsWorksheetName +
                "&pass=" + password +
                "&val1=" + ballName +
                "&val2=" + collisionMagnitude.ToString() +
                "&action=SetData";
		
        if (debugMode)
            Debug.Log("Connection String: " + connectionString);
        WWW www = new WWW(connectionString);
        float elapsedTime = 0.0f;
		
        while (!www.isDone)
        {
            elapsedTime += Time.deltaTime;			
            if (elapsedTime >= maxWaitTime)
            {
                // Error handling here.
                break;
            }
			
            yield return null;  
        }
		
        if (!www.isDone || !string.IsNullOrEmpty(www.error))
        {
            // Error handling here.
            yield break;
        }
		
        string response = www.text;
		
        if (response.Contains("Incorrect Password"))
        {
            // Error handling here.
            yield break;
        }
		
        if (response.Contains("RCVD OK"))
        {
            // Data correctly sent!
            yield break;
        }
    }*/
    #endregion

    #region Private Methods
    #endregion
}

