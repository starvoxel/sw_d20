using UnityEngine;
using System.Collections;

public abstract class Singleton<T> where T : Singleton<T>, new()
{
	#region Fields & Properties
	protected static T m_Instance;

	public static T Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = new T();
			}

			return m_Instance;
		}
	}

	public static bool IsInstanceNull
	{
		get { return m_Instance == null; }
	}
	#endregion

	#region Unity Methods
	#endregion

	#region Public Methods
	#endregion

	#region Protected Methods
	protected abstract void Initialize();
	#endregion

	#region Private Methods
	#endregion

	#region Blah
	#endregion
}