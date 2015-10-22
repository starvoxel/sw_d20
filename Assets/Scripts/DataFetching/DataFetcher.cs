using System;
using System.Collections;
using UnityEngine;
using LitJson;

public class DataFetcher : SingletonBehaviour<DataFetcher>
{
	#region Fields & Properties
	//const
	public const string INCORRECT_PASSWORD = "Incorrect Password";

	//structs
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



	//private	
	bool updating; //Probably temporary, will make a queue of some sort
	JsonData[] m_Data;
	#endregion
	
	public void Connect(string spreadsheetID, string password, params sConnectInfo[] connectInfo)
	{
		if (updating)
			return;
		
		updating = true;
		StartCoroutine(GetData(spreadsheetID, password, connectInfo));
	}
	
	IEnumerator GetData(string spreadsheetID, string password, sConnectInfo[] connectInfo)
	{
		for (int connectIndex = 0; connectIndex < connectInfo.Length; ++connectIndex)
		{
			sConnectInfo info = connectInfo[connectIndex];

			string connectionString = m_WebServiceURL + "?ssid=" + spreadsheetID + "&sheet=" + info.WorksheetName + "&pass=" + password + "&action=GetData";
		
			WWW www = new WWW (connectionString);
		
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
					updating = false;
					break;
				}
			
				yield return null;  
			}
		
			if (!www.isDone || !string.IsNullOrEmpty (www.error)) 
			{
				updating = false;

				if (info.CompletionCallback != null)
				{
					info.CompletionCallback(info.WorksheetName, null, www.error);
				}

				yield break;
			}
		
			string response = www.text;
		
			if (response.Contains (INCORRECT_PASSWORD)) 
			{
				updating = false;
				
				if (info.CompletionCallback != null)
				{
					info.CompletionCallback(info.WorksheetName, null, "Incorrect password provided");
				}
				yield break;
			}
		
			try 
			{
				m_Data = JsonMapper.ToObject<JsonData[]> (response);
			} 
			catch {
				updating = false;
				
				if (info.CompletionCallback != null)
				{
					info.CompletionCallback(info.WorksheetName, null, "Invalid data");
				}
				yield break;
			}

			updating = false;

			if (info.CompletionCallback != null)
			{
				info.CompletionCallback(info.WorksheetName, m_Data);
			}
		}
	}
	
	/*IEnumerator SendData(string ballName, float collisionMagnitude)
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
}

