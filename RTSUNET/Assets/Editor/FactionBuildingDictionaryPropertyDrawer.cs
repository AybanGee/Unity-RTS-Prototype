﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(FactionBuildingDictionary))]
[CustomPropertyDrawer(typeof(FactionUnitDictionary))]
[CustomPropertyDrawer(typeof(ColorDictionary))]
public class DictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer{

}
