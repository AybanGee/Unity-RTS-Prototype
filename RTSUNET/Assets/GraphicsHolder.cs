using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsHolder : MonoBehaviour {
	public Renderer[] coloredGraphics;

	public void colorize (Color c) {
		if (coloredGraphics.Length <= 0) return;
		for(int i = 0; i < coloredGraphics.Length; i++){
			coloredGraphics[i].material.color = c;
		}
	}
}