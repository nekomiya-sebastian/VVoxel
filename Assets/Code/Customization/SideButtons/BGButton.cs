using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGButton
	:
	MonoBehaviour
{
	public void SetupCallback( BGPanel panel,int index )
	{
		myPanel = panel;
		myIndex = index;
	}

	public void OnClick()
	{
		myPanel.LoadBG( myIndex );
	}

	BGPanel myPanel = null;
	int myIndex = -1;
}