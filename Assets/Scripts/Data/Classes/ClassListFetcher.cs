using UnityEngine;
using System.Collections;

public class ClassListFetcher : MonoBehaviour 
{
	[SerializeField] Transform m_UIParent;
	[SerializeField] UnityEngine.UI.Text m_TextPrefab;
	ClassList m_List = null;
	
	public void OnButtonClick()
	{
		m_List = new ClassList(m_UIParent, m_TextPrefab);
	}


}
