using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Assertions;

public class PosePanel
	:
	SidePanelBase
{
	class AnimPose
	{
		public AnimPose( string[] floats,float[] floatVals,string[] bools,bool[] boolVals,string[] triggers )
		{
			this.floats = floats;
			this.floatVals = floatVals;
			this.bools = bools;
			this.boolVals = boolVals;
			this.triggers = triggers;
		}

		public void ApplyParams( Animator animCtrl )
		{
			Assert.IsNotNull( animCtrl );
			Assert.IsTrue( ( floats == null && floatVals == null ) || ( floats != null && floatVals != null ) );
			Assert.IsTrue( ( bools == null && boolVals == null ) || ( bools != null && boolVals != null ) );
			if( floats != null && floatVals != null ) Assert.IsTrue( floats.Length == floatVals.Length );
			if( bools != null && boolVals != null ) Assert.IsTrue( bools.Length == boolVals.Length );

			if( bools != null )
			{
				for( int i = 0; i < bools.Length; ++i ) animCtrl.SetBool( bools[i],boolVals[i] );
			}
			if( floats != null )
			{
				for( int i = 0; i < floats.Length; ++i ) animCtrl.SetFloat( floats[i],floatVals[i] );
			}
			if( triggers != null )
			{
				foreach( var trigger in triggers ) animCtrl.SetTrigger( trigger );
			}
		}

		public string[] floats = null;
		public float[] floatVals = null;
		public string[] bools = null;
		public bool[] boolVals = null;
		public string[] triggers = null;
	}

	void Start()
	{
		for( int i = 0; i < ( int )AnimHandler.AnimState.Count; ++i )
		{
			anims.Add( ( AnimHandler.AnimState )i );
		}
		
		var posePanel = transform.Find( "Viewport" ).Find( "Content" );
		
		for( int i = 0; i < anims.Count; ++i )
		{
			var partButton = Instantiate( poseButtonPrefab,posePanel );
			partButton.GetComponentInChildren<TMP_Text>().text = anims[i].ToString();
			partButton.GetComponent<PoseButton>().SetupCallback( this,( AnimHandler.AnimState )i );
		}
	}

	public void PlayAnim( AnimHandler.AnimState index )
	{
		// animCtrl.SetInteger( "State",( int )index );
		animPoses[( int )index].ApplyParams( animCtrl );
	}

	[SerializeField] Animator animCtrl;

	// [SerializeField] GameObject charModel = null;
	[SerializeField] GameObject poseButtonPrefab = null;

	List<AnimHandler.AnimState> anims = new List<AnimHandler.AnimState>();

	static readonly AnimPose[] animPoses =
	{
		new AnimPose( new string[]{ "xMove","zMove" },new float[]{ 0.0f,0.0f },
			new string[]{ "moving","holdLeft","holdRight","swingingLeft","swingingRight","defeated" },
			new bool[]{ false,false,false,false,false,false },null ),
		new AnimPose( new string[]{ "zMove" },new float[]{ 1.0f },new string[]{ "moving" },new bool[]{ true },null ),
		new AnimPose( new string[]{ "zMove" },new float[]{ -1.0f },new string[]{ "moving" },new bool[]{ true },null ),
		new AnimPose( new string[]{ "xMove" },new float[]{ -1.0f },new string[]{ "moving" },new bool[]{ true },null ),
		new AnimPose( new string[]{ "xMove" },new float[]{ 1.0f },new string[]{ "moving" },new bool[]{ true },null ),
		new AnimPose( null,null,new string[]{ "holdLeft" },new bool[]{ true },null ),
		new AnimPose( null,null,new string[]{ "holdRight" },new bool[]{ true },null ),
		new AnimPose( null,null,new string[]{ "holdLeft" },new bool[]{ true },new string[]{ "throwLeft" } ),
		new AnimPose( null,null,new string[]{ "holdRight" },new bool[]{ true },new string[]{ "throwRight" } ),
		new AnimPose( null,null,new string[]{ "holdLeft","swingingLeft" },new bool[]{ true,true },null ),
		new AnimPose( null,null,new string[]{ "holdRight","swingingRight" },new bool[]{ true,true },null ),
		new AnimPose( null,null,new string[]{ "swingingLeft" },new bool[]{ false },null ),
		new AnimPose( null,null,new string[]{ "swingingRight" },new bool[]{ false },null ),
		new AnimPose( null,null,null,null,new string[]{ "takeDamage" } ),
		new AnimPose( null,null,null,null,new string[]{ "defeated" } )
	};
}