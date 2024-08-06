using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartsPanel
	:
	SidePanelBase
{
	[System.Serializable]
	public class Part
	{
		[SerializeField] public string readableName = "part name";
		[SerializeField] public string pivotName = "pivot name";
		[SerializeField] public GameObject panelPrefab = null;
		[SerializeField] public Vector3 zoomPos = Vector3.zero;
		[SerializeField] public Vector3 zoomRot = Vector3.zero;
		[SerializeField] public float zoomScale = 1.0f;
	}

	protected override void Awake()
	{
		base.Awake();

		SaveLoadHandler.SetupParts( parts );
	}

	void Start()
	{
		charCtrl = charModel.GetComponent<CustomCharCtrl>();

		var partsPanel = transform.Find( "Viewport" ).Find( "Content" );

		for( int i = 0; i < parts.Count; ++i )
		{
			var partButton = Instantiate( partButtonPrefab,partsPanel );
			partButton.GetComponentInChildren<TMP_Text>().text = parts[i].readableName;
			partButton.GetComponent<PartButton>().SetupCallback( this,i );
		}
	}

	public void SwitchToPanel( int index )
	{
		curPart = index;

		Destroy( curCustomizePanel );

		curCustomizePanel = Instantiate( parts[index].panelPrefab,canv.transform );
		customizePanel = curCustomizePanel.GetComponent<CustomizePanel>();
		customizePanel.SetCharModel( charModel );
		customizePanel.SetPartsPanel( this );
		customizePanel.SetCharCtrl( charCtrl );

		StartCoroutine( SetCharMovePart( movePart ) );
	}

	IEnumerator SetCharMovePart( bool movePart )
	{
		yield return( new WaitForEndOfFrame() );
		charCtrl.SetMovePart( movePart,customizePanel.GetCurModel().transform );
	}

	public void TryOpenPanel( string pivot )
	{
		for( int i = 0; i < parts.Count; ++i )
		{
			var part = parts[i];
			if( pivot == part.pivotName )
			{
				SwitchToPanel( i );
				break;
			}
		}
	}

	void SetCharTransform( Vector3 pos,Vector3 rot,float scale )
	{
		charModel.transform.localScale = Vector3.one * scale;
		charModel.transform.eulerAngles = rot;
		charModel.transform.position = pos;
	}

	public void ResetZoom()
	{
		SetCharTransform( Vector3.zero,Vector3.zero,1.0f );
	}

	public void ZoomPart()
	{
		if( curPart > -1 && curPart < parts.Count )
		{
			SetCharTransform( parts[curPart].zoomPos,parts[curPart].zoomRot,parts[curPart].zoomScale );
		}
	}

	public void Deselect()
	{
		Destroy( curCustomizePanel );
		curCustomizePanel = null;

		curPart = -1;
	}

	public void ResetPart()
	{
		customizePanel?.ResetCurModelDefaultTransform();
	}

	public void ToggleMovePart()
	{
		movePart = !movePart;
		if( curCustomizePanel != null ) charCtrl.SetMovePart( movePart,customizePanel.GetCurModel().transform );
		movePartToggle.SetIsOnWithoutNotify( movePart );
	}

	public void Scroll( int dir )
	{
		curPart += dir;
		if( curPart < 0 ) curPart = parts.Count - 1;
		else if( curPart > parts.Count - 1 ) curPart = 0;

		SwitchToPanel( curPart );
	}

	public bool HasOpenPanel()
	{
		return( curPart > -1 );
	}

	public List<Part> GetParts()
	{
		return( parts );
	}

	GameObject curCustomizePanel = null;
	CustomizePanel customizePanel = null;
	CustomCharCtrl charCtrl;

	[SerializeField] GameObject charModel = null;
	[SerializeField] Canvas canv = null;
	[SerializeField] Toggle movePartToggle = null;

	[SerializeField] GameObject partButtonPrefab = null;

	[SerializeField] List<Part> parts = new List<Part>();

	int curPart = -1;

	bool movePart = false;
}