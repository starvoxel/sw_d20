using UnityEngine;
using System.Collections;
using LitJson;

public class Class 
{
	#region Fields & Properties
	protected string m_Name;
	public string Name
	{
		get { return m_Name; }
	}
	#endregion

	#region Constructor Methods
	public Class(string name)
	{
		m_Name = name;
	}
	#endregion
}
