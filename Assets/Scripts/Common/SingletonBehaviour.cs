using UnityEngine;
using System.Collections;

public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>, new()
{
	#region Fields & Properties
	protected const string RESOURCE_PATH = "SingletonBehaviours/";

	[SerializeField] protected bool m_DontDestroyOnLoad = false;
	protected static T m_Instance;	
	public static T Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = GameObject.FindObjectOfType<T>();
				if(m_Instance == null)
				{
					m_Instance = Resources.Load<T>(RESOURCE_PATH + typeof(T).Name);
					if (m_Instance == null)
					{
						Debug.LogError("No prefab found for " + typeof(T).Name + ".  There must be a object in the scene of a prefab for this class.");
					}
				}
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
	protected virtual void Awake()
	{
		Initialize();
	}
	#endregion

	#region Public Methods
	#endregion

	#region Protected Methods
	protected virtual void Initialize()
	{
		if (m_DontDestroyOnLoad)
		{
			DontDestroyOnLoad(this);
		}
	}
	#endregion

	#region Private Methods
	#endregion
}
