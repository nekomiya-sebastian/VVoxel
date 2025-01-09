using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.Video;
using Unity.VisualScripting;
using System;
using System.Reflection;

public class CustomizePanel
	:
	MonoBehaviour
{
	void Start()
	{
		models.Add( nonePrefab );

		transform.Find( "TitleText" ).GetComponent<TMP_Text>().text = title;

		{
			var modelPanel = transform.Find( "CenterPanel" ).Find( "ModelPanel" ).Find( "Viewport" ).Find( "Content" );
			// var modelChildCount = modelPanel.transform.childCount;
			// Assert.IsTrue( modelChildCount >= models.Count );
			// // cull empty buttons
			// var modelButtons = new List<Transform>();
			// for( int i = 0; i < modelChildCount; ++i ) modelButtons.Add( modelPanel.GetChild( i ) );
			// for( int i = modelChildCount - models.Count - 1; i >= 0; --i )
			// {
			// 	Destroy( modelPanel.GetChild( i ).gameObject );
			// 	modelButtons.Remove( modelPanel.GetChild( i ) );
			// }
			
			for( int i = 0; i < models.Count; ++i )
			{
				var button = Instantiate( modelButtonPrefab,modelPanel ).transform;
				button.GetComponent<CustomizeButton>().SetupCallback( this,i );
				if( i < modelImgs.Count ) button.Find( "Image" ).GetComponent<Image>().sprite = modelImgs[i];
				else button.Find( "Image" ).GetComponent<Image>().color = Color.cyan;
				button.GetComponentInChildren<TMP_Text>().text = models[i].name;
			}

			// for( int i = 0; i < models.Count; ++i )
			// {
			// 	var curButton = modelButtons[i];
			// 	curButton.GetComponent<CustomizeButton>().SetupCallback( this,i );
			// 	if( i < modelImgs.Count ) curButton.Find( "Image" ).GetComponent<Image>().sprite = modelImgs[i];
			// 	else curButton.Find( "Image" ).GetComponent<Image>().color = Color.cyan;
			// }
		}

		{
			var colorPanel = transform.Find( "CenterPanel" ).Find( "ColorPanel" ).Find( "Viewport" ).Find( "Content" );

			for( int i = 0; i < mats.Count; ++i )
			{
				var button = Instantiate( modelButtonPrefab,colorPanel ).transform;
				var customizeButton = button.GetComponent<CustomizeButton>();
				customizeButton.buttonType = CustomizeButton.ButtonType.Color;
				customizeButton.SetupCallback( this,i );
				if( i < matImgs.Count ) button.Find( "Image" ).GetComponent<Image>().sprite = matImgs[i];
				else button.Find( "Image" ).GetComponent<Image>().color = mats[i].color;
			}

			// var colorChildCount = colorPanel.transform.childCount;
			// Assert.IsTrue( colorChildCount >= mats.Count );
			// var colorButtons = new List<Transform>();
			// for( int i = 0; i < colorChildCount; ++i ) colorButtons.Add( colorPanel.GetChild( i ) );
			// for( int i = colorChildCount - mats.Count - 1; i >= 0; --i )
			// {
			// 	Destroy( colorPanel.GetChild( i ).gameObject );
			// 	colorButtons.Remove( colorPanel.GetChild( i ) );
			// }
			// 
			// for( int i = 0; i < mats.Count; ++i )
			// {
			// 	var curButton = colorButtons[i];
			// 	curButton.GetComponent<CustomizeButton>().SetupCallback( this,i );
			// 	if( i < matImgs.Count ) curButton.Find( "Image" ).GetComponent<Image>().sprite = matImgs[i];
			// 	else curButton.Find( "Image" ).GetComponent<Image>().color = Color.red;
			// }
		}

		var curTarget = FindCurTarget();
		var targetModel = FindTargetModel( curTarget );
		curModel = targetModel;

		// set default model color
		if( targetModel != null )
		{
			var targetModelMesh = targetModel.GetComponentInChildren<MeshRenderer>();
			if( targetModelMesh != null ) curSelectedMat = targetModelMesh.material;
		}
		if( curSelectedMat == null && mats.Count > 0 ) curSelectedMat = mats[0];

		if( curModel != null )
		{
			curModelDefaultOffset = curModel.transform.localPosition;
			curModelDefaultRot = curModel.transform.localEulerAngles;
		}

		// maintain scroll position from prev panel
		var modelPanelScrollRect = transform.Find( "CenterPanel" ).Find( "ModelPanel" ).GetComponent<ScrollRect>();
		var colorPanelScrollRect = transform.Find( "CenterPanel" ).Find( "ColorPanel" ).GetComponent<ScrollRect>();
		modelPanelScrollRect.verticalNormalizedPosition = modelScroll;
		colorPanelScrollRect.verticalNormalizedPosition = colorScroll;
		modelPanelScrollRect.onValueChanged.AddListener( OnModelScroll );
		colorPanelScrollRect.onValueChanged.AddListener( OnColorScroll );

		SaveLoadHandler.UpdateTargetPart( pivotPath[pivotPath.Count - 1] );
	}

	public void ButtonCallback( CustomizeButton.ButtonType buttonType,int index )
	{
		// button clicked, apply change to character

		if( buttonType == CustomizeButton.ButtonType.Model )
		{
			// to switch out, delete first item found that doesn't have "pivot" in the name
			ReplaceModel( index,
				( modelOffsets.Count > index ) ? modelOffsets[index] : Vector3.zero,
				( modelRots.Count > index ) ? modelRots[index] : Vector3.zero,
				null,true );
		}
		else if( buttonType == CustomizeButton.ButtonType.Color )
		{
			ReplaceModelMat( index,null,true );
		}
	}

	public void ReplaceModel( int index,
		Vector3 offset,Vector3 rot,
		GameObject newCharModel = null,bool setDefaults = false )
	{
		// -1 = unset, models.Count = manually selected none
		bool invalidIndex = ( index < 0 || index == models.Count );

		if( charModel == null ) charModel = newCharModel;

		Transform curTarget = FindCurTarget();
		var targetModel = FindTargetModel( curTarget );
		// if( index != models.Count - 1 ) Assert.IsNotNull( targetModel );

		// to switch out, delete first item found that doesn't have "pivot" in the name
		Destroy( targetModel );
		// if( index >= models.Count - 1 ) return;

		// set default color if available
		if( !invalidIndex && defaultColors.Count > index && defaultColors[index] != null ) curSelectedMat = defaultColors[index];
		
		var newModel = Instantiate( !invalidIndex ? models[index] : nonePrefab );
		newModel.transform.SetParent( curTarget,false );
		newModel.transform.localPosition = offset;
		newModel.transform.localRotation = Quaternion.Euler( rot );
		if( newModel.transform.childCount > 0 )
		{
			var meshColl = newModel.AddComponent<MeshCollider>();
			meshColl.sharedMesh = newModel.transform.GetChild( 0 ).GetComponent<MeshFilter>().mesh;
		}
		// Instantiate( models[index],offset,Quaternion.Euler( rot ),curTarget );
		if( curSelectedMat != null )
		{
			var newModelMesh = newModel.GetComponentInChildren<MeshRenderer>();
			if( newModelMesh != null ) newModelMesh.material = curSelectedMat;
		}

		if( setDefaults )
		{
			curModel = newModel;
			curModelDefaultOffset = offset;
			curModelDefaultRot = rot;

			charCtrl.SetTargetPart( newModel.transform );
		}
		
		SaveLoadHandler.UpdateTargetPart( pivotPath[pivotPath.Count - 1] );
		// set -1 for none so we can add new models without messing up the index
		SaveLoadHandler.UpdateModel( index == models.Count - 1 ? -1 : index );
		SaveLoadHandler.UpdateOffset( offset );
		SaveLoadHandler.UpdateRot( rot );
	}

	public void ReplaceModelMat( int index,GameObject newCharModel = null,bool setDefaults = false )
	{
		if( charModel == null ) charModel = newCharModel;

		Transform curTarget = FindCurTarget();
		var targetModel = FindTargetModel( curTarget );
		Assert.IsNotNull( targetModel );

		var newMat = mats[index];
		var targetModelMesh = targetModel.GetComponentInChildren<MeshRenderer>();
		if( targetModelMesh != null ) targetModelMesh.material = newMat;
		
		if( setDefaults ) curSelectedMat = newMat;
		
		SaveLoadHandler.UpdateTargetPart( pivotPath[pivotPath.Count - 1] );
		SaveLoadHandler.UpdateColor( index );
	}

	Transform FindCurTarget()
	{
		Transform curTarget = charModel.transform;
		foreach( var pivot in pivotPath )
		{
			var prevTarget = curTarget;
			curTarget = curTarget.Find( pivot );
			Assert.IsNotNull( curTarget );
		}
		return( curTarget );
	}

	GameObject FindTargetModel( Transform curTarget )
	{
		var targetChildCount = curTarget.childCount;
		GameObject targetModel = null;
		for( int i = 0; i < targetChildCount; ++i )
		{
			if( !curTarget.GetChild( i ).name.Contains( "Pivot" ) )
			{
				targetModel = curTarget.GetChild( i ).gameObject;
				break;
			}
		}
		return( targetModel );
	}

	void OnModelScroll( Vector2 scroll )
	{
		modelScroll = scroll.y;
	}

	void OnColorScroll( Vector2 scroll )
	{
		colorScroll = scroll.y;
	}

	public void SetCharModel( GameObject charModel )
	{
		this.charModel = charModel;
	}

	public void SetPartsPanel( PartsPanel partsPanel )
	{
		this.partsPanel = partsPanel;
	}

	public void SetCharCtrl( CustomCharCtrl charCtrl )
	{
		this.charCtrl = charCtrl;
	}

	public void Scroll( int dir )
	{
		partsPanel.Scroll( dir );
	}

	public void ResetCurModelDefaultTransform()
	{
		if( curModel == null ) return;

		curModel.transform.localPosition = curModelDefaultOffset;
		curModel.transform.localRotation = Quaternion.Euler( curModelDefaultRot );
		curModel.transform.localScale = Vector3.one * curModelDefaultScale;
	}

	public GameObject GetCurModel()
	{
		if( curModel == null )
		{
			var curTarget = FindCurTarget();
			var targetModel = FindTargetModel( curTarget );
			if( targetModel == null ) targetModel = Instantiate( nonePrefab,curTarget );
			curModel = targetModel;
		}
		return( curModel );
	}

	public List<string> GetPivotPath()
	{
		return( pivotPath );
	}

	GameObject charModel = null;
	PartsPanel partsPanel;
	CustomCharCtrl charCtrl;

	[SerializeField] GameObject nonePrefab = null;

	[SerializeField] string title = "no title";

	[SerializeField] List<string> pivotPath = new List<string>();
	[SerializeField] GameObject modelButtonPrefab = null;

	[SerializeField] List<GameObject> models = new List<GameObject>();
	[SerializeField] List<Sprite> modelImgs = new List<Sprite>();
	[SerializeField] List<Vector3> modelOffsets = new List<Vector3>();
	[SerializeField] List<Vector3> modelRots = new List<Vector3>();
	[SerializeField] List<Material> defaultColors = new List<Material>();

	[SerializeField] List<Material> mats = new List<Material>();
	[SerializeField] List<Sprite> matImgs = new List<Sprite>();

	Material curSelectedMat = null;
	
	GameObject curModel = null;
	Vector3 curModelDefaultOffset = Vector3.zero;
	Vector3 curModelDefaultRot = Vector3.zero;
	const float curModelDefaultScale = 1.0f;

	static float modelScroll = 1.0f;
	static float colorScroll = 1.0f;
}