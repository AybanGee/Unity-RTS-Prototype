using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public static class ToolTipFunctions {
	public static ToolTipData Skill (MonoSkill ms) {
		ToolTipData ttd = new ToolTipData ();
		ttd.displayPic = ms.sSprite;
		ttd.name = ms.sName;
		ttd.description = ms.description;
		return ttd;
	}
	public static ToolTipData Units (UnitFramework uf) {
		ToolTipData ttd = new ToolTipData ();
		ttd.displayPic = uf.artwork;
		ttd.description = uf.description;
		ttd.name = uf.name;
		ttd.cost = uf.manaCost.ToString ();;
		ttd.duration = uf.creationTime.ToString ();
		return ttd;
	}
	public static ToolTipData Units (MonoUnitFramework mfu) {
		ToolTipData ttd = new ToolTipData ();
		ttd.displayPic = mfu.artwork;
		ttd.description = mfu.description;
		ttd.name = mfu.name;
		ttd.cost = mfu.manaCost.ToString ();;
		ttd.duration = mfu.creationTime.ToString ();
		return ttd;
	}
	public static ToolTipData Building (Building b) {
		ToolTipData ttd = new ToolTipData ();
		ttd.displayPic = b.artwork;
		ttd.description = b.description;
		ttd.name = b.name;
		ttd.cost = b.manaCost.ToString ();;
		ttd.duration = b.creationTime.ToString ();
		return ttd;
	}
}