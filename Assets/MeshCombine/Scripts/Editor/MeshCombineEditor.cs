using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(MeshCombine))] 
public class MeshCombineEditor : Editor 
{
	private MeshCombine t = null;

	public override void OnInspectorGUI()
	{
		t = (MeshCombine)target;

		GUILayout.Space(10);
		GUILayout.Label("*所有网格必须拥有同一个材质球");	
		GUILayout.Space(10);
		t.sameMat = EditorGUILayout.Toggle("必须相同的材质球", t.sameMat);
		GUILayout.Space(10);

		if(!t.combined)
		{
			t.generateLightmapUV = EditorGUILayout.Toggle("生成 Ligthmap UV's", t.generateLightmapUV);
			GUILayout.Label("合并所有enabled的网格");
			if(GUILayout.Button("合并")) 
			{
				if(t.transform.childCount > 1)
					combineMeshes();
			}   
		}
		else
		{
			GUILayout.Label("拆解合并的网格");
			if(GUILayout.Button("拆解")) 
			{
				EnableRenderers(true);
				t.savedPrefab = false;
				if(t.combined)
					DestroyImmediate(t.combined);
			}
		}

		if(t.combined && !t.savedPrefab)
		{
			t.advanced = EditorGUILayout.Toggle("高级功能", t.advanced);
		}
		
		if(t.combined && t.advanced && !t.savedPrefab)
		{
			if(GUILayout.Button("保存预设")) 
			{
				string n = t.meshName;
				if(System.IO.Directory.Exists("Assets/MeshCombine/SavedMeshes/"))
				{
					if(!System.IO.File.Exists("Assets/MeshCombine/SavedMeshes/" + t.meshName + ".asset"))
					{     	
						AssetDatabase.CreateAsset(t.combined.GetComponent<MeshFilter>().sharedMesh, "Assets/MeshCombine/SavedMeshes/" + n + ".asset");
						t.advanced = false;
						t.savedPrefab = true;
						Debug.Log("SavedAssets : " + n + ".asset");
					}
					else
					{
						Debug.Log(t.meshName + ".asset" + " 已经存在, 请修改名字！");
					}

				}
				else
				{
					Debug.Log("Missing Folder: Assets/Artwork/MeshCombine/SavedMeshes/");
				}
			}

			t.meshName = GUILayout.TextField(t.meshName);
		}

		if (GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}

	private void EnableRenderers(bool e) 
	{	
		for (int i = 0; i < t.combinedGameOjects.Length; i++)
		{
			Renderer rendererTemp = t.combinedGameOjects[i].GetComponent<Renderer>();
			if(rendererTemp != null)
			{
				rendererTemp.enabled = e;	
			}
		}  
	}

	/**
	 * 获取 MeshFilter 数组;
	 * @return MeshFilter 数组，不包括 disabled 掉的网格(对挂载了可见碰撞体的物件是起效的);
	 * */
	private MeshFilter[] FindEnabledMeshes() 
	{
		int count = 0;
		MeshFilter[] renderers = t.transform.GetComponentsInChildren<MeshFilter>();

		for (int i = 0; i < renderers.Length; i++)
		{
			// 计算当前节点下面所有开启的meshrenderers;
			MeshRenderer meshRendererTemp = renderers[i].GetComponent<MeshRenderer>();
			if(meshRendererTemp != null && meshRendererTemp.enabled)
			{
				count++;
			}
		}

		// 创建一个新的 MeshFilter 数组;
		var meshfilters = new MeshFilter[count];
		count = 0;
		for (int ii = 0; ii < renderers.Length; ii++)
		{
			// 添加所有开启的 Mesh 到数组;
			MeshRenderer meshRendererTemp = renderers[ii].GetComponent<MeshRenderer>();
			if(meshRendererTemp != null && meshRendererTemp.enabled)
			{
				meshfilters[count] = renderers[ii];
				count++;
			}
		}

		return meshfilters;
	}

	/**
	 * 合并网格;
	 * */
	private void combineMeshes () 
	{
		GameObject combinedFrags = new GameObject();
		combinedFrags.AddComponent<MeshFilter>();
		combinedFrags.AddComponent<MeshRenderer>();	

		MeshFilter[] meshFilters = FindEnabledMeshes();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];

		Debug.Log("网格合并: 总共合并了 " + meshFilters.Length + " 个网格");

		t.combinedGameOjects = new GameObject[meshFilters.Length];   
		List<Material> matList = new List<Material>();
		for (int i = 0; i < meshFilters.Length; i++)
		{
			if(!t.sameMat)
			{
				Material[] sharedMaterialsTemp = meshFilters[i].transform.gameObject.GetComponent<MeshRenderer>().sharedMaterials;
				if(sharedMaterialsTemp != null)
				{
					if(matList.Count <= 0 && sharedMaterialsTemp.Length > 0)
					{
						matList.AddRange(sharedMaterialsTemp);
					}
					else
					{
						List<Material> matListTemp = new List<Material>();
						for(int k = 0; k < sharedMaterialsTemp.Length; k++)
						{
							bool bFind = false;
							for(int kk = 0; kk < matList.Count; kk++)
							{
								if(sharedMaterialsTemp[k].Equals(matList[kk]))
								{
									bFind = true;
								}
							}

							if(!bFind)
							{
								matListTemp.Add(sharedMaterialsTemp[k]);
							}
						}

						for(int k = 0; k < matListTemp.Count; k++)
						{
							matList.Add(matListTemp[k]);
						}	
					}
				}
			}
			else
			{
				Material[] sharedMaterialsTempChcek = meshFilters[i].transform.gameObject.GetComponent<MeshRenderer>().sharedMaterials;
				if(sharedMaterialsTempChcek.Length > 1)
				{
					Debug.Log("存在多个材质球的物体 ： " + meshFilters[i].transform.gameObject.name);
				}

				Material sharedMaterialTemp = meshFilters[i].transform.gameObject.GetComponent<MeshRenderer>().sharedMaterial;
				if(matList.Count <= 0)
					matList.Add(sharedMaterialTemp);
			}

			t.combinedGameOjects[i] = meshFilters[i].gameObject;
			combine[i].mesh = meshFilters[i].transform.GetComponent<MeshFilter>().sharedMesh;
			combine[i].transform = meshFilters[i].transform.localToWorldMatrix;  		
		}

		// 创建一个新的网格并合并;
		combinedFrags.GetComponent<MeshFilter>().mesh = new Mesh();
		combinedFrags.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);

		// 为合并后的网格添加材质;
		string strMatName = null;
		for(int i = 0; i < matList.Count; i++)
			strMatName += matList[i].name + "|";
		Debug.Log("当前合并的网格中包含的材质球列表 : " + strMatName);

		combinedFrags.GetComponent<MeshRenderer>().sharedMaterials = matList.ToArray();

		if(t.generateLightmapUV)
		{
			Unwrapping.GenerateSecondaryUVSet(combinedFrags.GetComponent<MeshFilter>().sharedMesh);
			combinedFrags.isStatic = true;
		}

		combinedFrags.name = "_CombinedMesh_" + target.name + "_";
		t.combined = combinedFrags.gameObject;
		EnableRenderers(false);
		combinedFrags.transform.parent = t.transform;
	}	
}
