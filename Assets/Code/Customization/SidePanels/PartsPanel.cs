using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
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
		var partsPanel = transform.Find( "Viewport" ).Find( "Content" );

		VerifyPartMeshReadability();

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

		// curCustomizePanel = Instantiate( parts[index].panelPrefab,canv.transform );
		curCustomizePanel = Instantiate( customizationPanelPrefab,canv.transform );
		customizePanel = curCustomizePanel.GetComponent<CustomizePanel>();
		parts[index].panelPrefab.GetComponent<CustomizePanel>().CopyInto( customizePanel );
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
		charCtrl.transform.localScale = Vector3.one * scale;
		charCtrl.transform.eulerAngles = rot;
		charCtrl.transform.position = pos;
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

	public void VerifyPartMeshReadability()
	{
		// verify that all models are read/write enabled (mostly for when we add new models &
		//  inevitably forget about doing this)
		// this only matters if we export the project
		// #if UNITY_EDITOR
		// bool foundInvalidMesh = false;
		// foreach( var part in parts )
		// {
		// 	var models = part.panelPrefab.GetComponent<CustomizePanel>().GetModels();
		// 	foreach( var model in models )
		// 	{
		// 		if( !model.transform.GetChild( 0 ).GetComponent<MeshFilter>().sharedMesh.isReadable )
		// 		{
		// 			foundInvalidMesh = true;
		// 			Debug.LogError( "Mesh must be marked as Read/Write in the import settings - " + model.name );
		// 		}
		// 	}
		// }
		// if( foundInvalidMesh )
		// {
		// 	Assert.IsTrue( false,
		// 		"Meshes must be marked as Read/Write enabled in import settings or else it causes build errors" );
		// }
		// #endif
	}

	GameObject curCustomizePanel = null;
	CustomizePanel customizePanel = null;
	[SerializeField] CustomCharCtrl charCtrl;

	[SerializeField] GameObject charModel = null;
	[SerializeField] Canvas canv = null;
	[SerializeField] Toggle movePartToggle = null;

	[SerializeField] GameObject partButtonPrefab = null;

	[SerializeField] GameObject customizationPanelPrefab = null;

	[SerializeField] List<Part> parts = new List<Part>();

	int curPart = -1;

	bool movePart = false;
}