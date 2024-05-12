using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class CosmereCodesName : ScriptableObject
{
	public List<CosmereEntity> Cosmere; // Replace 'EntityType' to an actual type that is serializable.
	public List<CytonicEntity> Cytonic; // Replace 'EntityType' to an actual type that is serializable.
}
