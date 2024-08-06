using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

public class SaveLoadHandler
	:
	MonoBehaviour
{
	class PartSaveInfo
	{
		public int model = -1;
		public int color = -1;
		public Vector3 offset = Vector3.zero;
		public Vector3 rot = Vector3.zero;

		public void LoadLine( string line )
		{
			var strings = line.Split( ' ' );
			model = int.Parse( strings[0] );
			color = int.Parse( strings[1] );
			offset.x = float.Parse( strings[2] );
			offset.y = float.Parse( strings[3] );
			offset.z = float.Parse( strings[4] );
			rot.x = float.Parse( strings[5] );
			rot.y = float.Parse( strings[6] );
			rot.z = float.Parse( strings[7] );
		}

		public string GenerateLine()
		{
			return( model.ToString() + " " + color.ToString() + " " +
				offset.x.ToString( "0.0000" ) + " " + offset.y.ToString( "0.0000" ) + " " + offset.z.ToString( "0.0000" ) + " " +
				rot.x.ToString( "0.0" ) + " " + rot.y.ToString( "0.0" ) + " " + rot.z.ToString( "0.0" ) );
		}
	}

	void Start()
	{
		defaultNekoPath = Application.streamingAssetsPath + "/" + "NekomiyaSebastian.vvox";
		customPath = Application.streamingAssetsPath + "/" + "MyCharacter.vvox";

		LoadFromPath( File.Exists( customPath ) ? customPath : defaultNekoPath );
	}

	void Update()
	{
		if( Input.GetKey( KeyCode.LeftControl ) && Input.GetKeyDown( KeyCode.S ) ) Save();
		if( Input.GetKey( KeyCode.LeftControl ) && Input.GetKeyDown( KeyCode.R ) ) LoadDefault();
	}

	public void Save()
	{
		// var path = EditorUtility.SaveFilePanel( "Select Save Location",Application.dataPath,"my character.vvox","vvox" );
		// 
		// if( path.Length > 0 ) // 0 len path = cancelled
		{
			var path = Application.streamingAssetsPath + "/" + "MyCharacter.vvox";
			var writer = new StreamWriter( path );
			foreach( var part in partOrder )
			{
				writer.WriteLine( parts[part].GenerateLine() );
			}
			writer.Close();
		}
	}

	public void Load()
	{
		// var path = EditorUtility.OpenFilePanel( "Select .vvox File",Application.dataPath,"vvox" );
		// if( path.Length > 0 ) LoadFromPath( path );
	}

	// load default nekomiya sebastian model
	// todo: menu to pick from lots of default models
	public void LoadDefault()
	{
		LoadFromPath( defaultNekoPath );
	}

	void LoadFromPath( string path )
	{
		var reader = new StreamReader( path );
		var lines = new List<string>();
		while( !reader.EndOfStream ) lines.Add( reader.ReadLine() );
		reader.Close();

		var data = new List<PartSaveInfo>();
		foreach( var line in lines )
		{
			data.Add( new PartSaveInfo() );
			data[data.Count - 1].LoadLine( line );
		}

		var panelParts = partsPanel.GetParts();
		if( panelParts.Count != data.Count )
		{
			print( "Part count mismatch!" );
			return;
		}
		for( int i = 0; i < panelParts.Count; ++i )
		{
			var customizePanel = panelParts[i].panelPrefab.GetComponent<CustomizePanel>();
			customizePanel.ReplaceModel( data[i].model,data[i].offset,data[i].rot,charModel,false );
			// model has to load in b4 setting mat
			if( data[i].color > -1 ) StartCoroutine( SetModelMat( customizePanel,data[i].color ) );

			parts[panelParts[i].pivotName] = data[i];
		}
	}

	IEnumerator SetModelMat( CustomizePanel customizePanel,int color )
	{
		yield return( new WaitForEndOfFrame() );
		
		customizePanel.ReplaceModelMat( color,charModel,false );
	}

	public static void SetupParts( List<PartsPanel.Part> partsList )
	{
		foreach( var part in partsList )
		{
			partOrder.Add( part.pivotName );
			parts.Add( part.pivotName,new PartSaveInfo() );
		}
	}

	public static void UpdateTargetPart( string targetPivotName )
	{
		pivotName = targetPivotName;
	}

	public static void UpdateModel( int model )
	{
		parts[pivotName].model = model;
	}

	public static void UpdateColor( int color )
	{
		parts[pivotName].color = color;
	}

	public static void UpdateOffset( Vector3 offset )
	{
		parts[pivotName].offset = offset;
	}

	public static void UpdateRot( Vector3 rot )
	{
		parts[pivotName].rot = rot;
	}

	[SerializeField] PartsPanel partsPanel = null;
	[SerializeField] GameObject charModel = null;

	static List<string> partOrder = new List<string>();
	static Dictionary<string,PartSaveInfo> parts = new Dictionary<string,PartSaveInfo>();
	static string pivotName = ""; // cur pivot that update funcs will affect
	string defaultNekoPath;
	string customPath;
}