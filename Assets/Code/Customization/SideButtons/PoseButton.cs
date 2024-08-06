using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseButton
	:
	MonoBehaviour
{
	public void SetupCallback( PosePanel panel,AnimHandler.AnimState index )
	{
		myPanel = panel;
		myIndex = index;
	}

	public void OnClick()
	{
		myPanel.PlayAnim( myIndex );
	}

	PosePanel myPanel = null;
	AnimHandler.AnimState myIndex = AnimHandler.AnimState.None;
}