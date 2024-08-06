using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelHotkeys
	:
	MonoBehaviour
{
	void Update()
	{
		if( Input.GetKeyDown( KeyCode.R ) ) partsPanel.ResetZoom();
		if( Input.GetKeyDown( KeyCode.F ) ) partsPanel.ZoomPart();
		if( Input.GetKeyDown( KeyCode.X ) ) partsPanel.Deselect();
		if( Input.GetKeyDown( KeyCode.E ) ) partsPanel.ToggleMovePart();
		if( Input.GetKeyDown( KeyCode.C ) ) partsPanel.Scroll( 1 );
		if( Input.GetKeyDown( KeyCode.Z ) ) partsPanel.Scroll( -1 );
		if( Input.GetKeyDown( KeyCode.T ) ) partsPanel.ResetPart();
		if( Input.GetKeyDown( KeyCode.V ) ) bgPanel.ResetBGTransform();
		if( Input.GetKeyDown( KeyCode.Q ) ) bgPanel.ToggleMoveBG();
	}
	
	[SerializeField] PartsPanel partsPanel = null;
	// [SerializeField] PosePanel posePanel = null;
	[SerializeField] BGPanel bgPanel = null;
}