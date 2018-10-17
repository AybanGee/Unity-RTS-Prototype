using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
	public abstract class MonoSkill : NetworkBehaviour {
		public virtual void ActOn(GameObject go)
		{}
		public virtual void Act(){}
	}


