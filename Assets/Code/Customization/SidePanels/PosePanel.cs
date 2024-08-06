using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PosePanel
	:
	SidePanelBase
{
	void Start()
	{
		animCtrl = charModel.GetComponent<Animator>();

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
		animCtrl.SetInteger( "State",( int )index );
	}

	Animator animCtrl;

	[SerializeField] GameObject charModel = null;
	[SerializeField] GameObject poseButtonPrefab = null;

	List<AnimHandler.AnimState> anims = new List<AnimHandler.AnimState>();
}