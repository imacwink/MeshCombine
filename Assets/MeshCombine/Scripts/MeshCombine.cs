using UnityEngine;
using System.Collections;

//[AddComponentMenu("MeshCombine")]
public class MeshCombine : MonoBehaviour 
{
	public GameObject[] combinedGameOjects = null;
	public GameObject combined= null;
	public string meshName = "Combined_Meshes";
	public bool advanced = false;
	public bool savedPrefab = false;
	public bool generateLightmapUV = false;
	public bool sameMat = false;
}