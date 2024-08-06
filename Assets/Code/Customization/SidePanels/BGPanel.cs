using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BGPanel
	:
	SidePanelBase
{
	void Start()
	{
		var bgPanel = transform.Find( "Viewport" ).Find( "Content" );

		for( int i = 0; i < bgPrefabs.Count; ++i )
		{
			var bgButton = Instantiate( bgButtonPrefab,bgPanel );
			bgButton.GetComponentInChildren<TMP_Text>().text = bgPrefabs[i].name;
			bgButton.GetComponent<BGButton>().SetupCallback( this,i );
		}

		LoadBG( 0 ); // start with none background
	}

	public void LoadBG( int index )
	{
		Destroy( curBG );

		curBG = Instantiate( bgPrefabs[index] );

		bgCtrl.SetBGModel( curBG.transform );
	}

	public void ResetBGTransform()
	{
		curBG.transform.position = Vector3.zero;
		curBG.transform.rotation = Quaternion.identity;
		curBG.transform.localScale = Vector3.one;
	}
	
	public void ToggleMoveBG()
	{
		bgMove = !bgMove;
		bgCtrl.SetMoveBG( bgMove );
		moveBGToggle.SetIsOnWithoutNotify( bgMove );

		charCtrl.LockMovement( bgMove );
	}

	[SerializeField] GameObject bgButtonPrefab = null;
	[SerializeField] Toggle moveBGToggle = null;
	[SerializeField] BGCtrl bgCtrl = null;
	[SerializeField] CustomCharCtrl charCtrl = null;
	bool bgMove = false;

	[SerializeField] List<GameObject> bgPrefabs = new List<GameObject>();

	GameObject curBG = null;
}