using UnityEngine;
using LitJson;
using System.Collections;

public class DataFetchTester : MonoBehaviour 
{
	#region Fields & Properties
    protected int FETCH_COUNT = 10;

	[SerializeField] protected string m_SpreadSheetID;
	[SerializeField] protected string m_WorksheetName;

	[SerializeField] protected UnityEngine.UI.Image m_ProgressImage;
	[SerializeField] protected UnityEngine.UI.Text m_ProgressText;
	#endregion

	#region Unity Methods
	#endregion

	#region Public Methods
	#endregion

	#region Protected Methods
	#endregion

	#region Private Methods
	public void OnButtonClick()
	{
    	DataFetchingController.Instance.Fetch(m_SpreadSheetID, string.Empty, new DataFetchingController.sConnectInfo(m_WorksheetName, OnDownloadComplete, OnDownloadUpdate, OnDownloadStarted));
	}

	private void OnDownloadStarted(string worksheetName)
	{
		if (m_WorksheetName == worksheetName)
		{
			Debug.Log("Download started on: " + m_WorksheetName);
		}
	}

	private void OnDownloadUpdate(string worksheetName, float progress)
	{
		if (m_WorksheetName == worksheetName)
		{
			if (m_ProgressImage != null)
			{
				m_ProgressImage.fillAmount = progress;
			}

			if (m_ProgressText != null)
			{
				m_ProgressText.text = (progress * 100).ToString("00.00");
			}

			Debug.Log("Download progress update on: " + m_WorksheetName + " @ " + progress);
		}
	}

	private void OnDownloadComplete(string worksheetName, JsonData[] data, string error)
	{
		if (m_WorksheetName == worksheetName)
		{
			if (!string.IsNullOrEmpty(error))
			{
				if (m_ProgressImage != null)
				{
					m_ProgressImage.fillAmount = 0;
				}
				
				if (m_ProgressText != null)
				{
					m_ProgressText.text = "ERROR";
				}
				Debug.Log("Download error on: " + m_WorksheetName + " - " + error);
			}
			else
			{
				if (m_ProgressImage != null)
				{
					m_ProgressImage.fillAmount = 1;
				}
				
				if (m_ProgressText != null)
				{
					m_ProgressText.text = 100.ToString("00.00");
				}
				Debug.Log("Download complete on: " + m_WorksheetName + " - " + JsonMapper.ToJson(data));
			}
		}
	}
	#endregion
}
