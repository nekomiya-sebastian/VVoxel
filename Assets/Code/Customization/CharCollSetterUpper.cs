using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CharCollSetterUpper
	:
	MonoBehaviour
{
	void Start()
	{
		self = this;
	}

	public static void SetupColls()
	{
		if( self ) self.CheckChildren( self.transform,true );
	}

	void CheckChildren( Transform start,bool first )
	{
		// dont try to add collider to pivot with none object selected
		if( start.name.Contains( "None" ) ) return;

		// print( start.name + " " + first );
		bool contains = start.name.Contains( "Pivot" );
		if( contains || first )
		{
			var childCount = start.childCount;
			for( int i = 0; i < childCount; ++i )
			{
				latest = start.name;
				CheckChildren( start.GetChild( i ),false );
			}
		}
		else if( !contains && !first )
		{
			var meshColl = start.gameObject.AddComponent<MeshCollider>();
			if( start.Find( "default" ) == null ) print( "can't find default for " + start.name );
			meshColl.sharedMesh = start.Find( "default" ).GetComponent<MeshFilter>().mesh;
		}
	}

	static CharCollSetterUpper self = null;

	string latest = "";
}