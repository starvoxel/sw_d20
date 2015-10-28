using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class ClassList 
{
	#region Fields & Properties

	protected Transform m_UIParent;
	protected Text m_TextPrefab;

	protected Action<string> m_OnClassListFetchComplete;

	protected List<Class> m_BasicClasses = new List<Class>();
	protected List<Class> m_PrestigeClasses = new List<Class>();

	/// <summary>
	/// Gets the basic classes.
	/// </summary>
	/// <value>The basic classes.</value>
	public Class[] BasicClasses
	{
		get { return m_BasicClasses.ToArray (); }
	}
	/// <summary>
	/// Gets the prestige classes.
	/// </summary>
	/// <value>The prestige classes.</value>
	public Class[] PrestigeClasses
	{
		get { return m_PrestigeClasses.ToArray (); }
	}
	/// <summary>
	/// Gets all classes.
	/// </summary>
	/// <value>All classes.</value>
	public Class[] AllClasses
	{
		get
		{
			List<Class> allClasses = new List<Class>(m_BasicClasses.Count + m_PrestigeClasses.Count);

			allClasses.AddRange(m_BasicClasses);
			allClasses.AddRange(m_PrestigeClasses);

			return allClasses.ToArray();
		}
	}

	public bool IsInitialized
	{
		get { return BasicClasses != null && BasicClasses.Length > 0; }
	}
	#endregion

	#region Constructor Methods
	public ClassList(/*Temp*/Transform uiParent, Text textPrefab)
	{
		m_UIParent = uiParent;
		m_TextPrefab = textPrefab;
	}
	#endregion

	#region Public Methods
	public void Initialize()
	{
		FetchClassListInfo(null);
	}

	/// <summary>
	/// Fethes the basic class list info which includes: name, vitality dice, class skills, ability importance, page ref #
	/// </summary>
	public void FetchClassListInfo(Action<string> onClassListFethComplete)
	{
		m_OnClassListFetchComplete = onClassListFethComplete;

		DataFetchingController.sFetchRequestInfo request = new DataFetchingController.sFetchRequestInfo (GlobalConstants.SPREADSHEET_ID, GlobalConstants.PASSWORD, new DataFetchingController.sConnectInfo (GlobalConstants.CLASS_INFO_SHEET_NAME, OnFetchComplete));
		
		DataFetchingController.Instance.Fetch(request);
	}
	#endregion

	#region Protected Methods
	/// <summary>
	/// Callback for when a worksheet fetch has just started by the DataFetcher
	/// </summary>
	/// <param name="worksheetName">Worksheet name.</param>
	protected void OnFetchStart(string worksheetName)
	{
		//TODO jsmellie: If we have a progress bar we could update it here
	}

	/// <summary>
	/// Callback for when a worksheet fetch has been updated by the DataFetcher
	/// </summary>
	/// <param name="worksheetName">Worksheet name.</param>
	/// <param name="progress">Progress.</param>
	protected void OnFetchUpdate(string worksheetName, float progress)
	{
		//TODO jsmellie: If we have a progress bar we could update it here
	}

	/// <summary>
	/// Callback for when a worksheet fetch is complete.
	/// </summary>
	/// <param name="worksheet">Worksheet.</param>
	/// <param name="data">Data.</param>
	/// <param name="error">If not null or empty then there was an error.</param>
	protected void OnFetchComplete(string worksheet, JsonData[] data, string error)
	{
		if (!string.IsNullOrEmpty(error))
		{

		}
		else
		{
			switch(worksheet)
			{
			case GlobalConstants.CLASS_INFO_SHEET_NAME:
				ParseClassInfoJson(data);
				break;
			}
		}

		//TODO jsmellie: Fire the proper callback.  I don't think I need the switch state anymore, just a if because something else will be in charge of downloading the actual classes
	}

	protected void ParseClassInfoJson(JsonData[] data)
	{
		foreach(JsonData element in data)
		{
			if (element.Keys.Contains("Class Name"))
			{
				Class curClass = new Class(element["Class Name"].ToString());

				if (!element.Keys.Contains("Starting Credits") || string.IsNullOrEmpty(element["Starting Credits"].ToString()))
				{
					m_BasicClasses.Add(curClass);
				}
				else
				{
					m_PrestigeClasses.Add(curClass);
				}
			}
		}

		CreateClassListUI();
	}

	protected void CreateClassListUI()
	{
		Class[] allClasses = AllClasses;

		for(int i = 0; i < allClasses.Length; ++i)
		{
			Class curClass = allClasses[i];

			Text classText = GameObject.Instantiate<Text>(m_TextPrefab);
			classText.transform.SetParent(m_UIParent,false);
			classText.transform.SetSiblingIndex(i + 1);
			classText.text = curClass.Name;
			classText.name = curClass.Name;

		}
	}
	#endregion
}
