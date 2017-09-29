using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MaterialDivide))] 
public class MaterialDivideEditor : Editor
{
	private MaterialDivide t = null;

	public override void OnInspectorGUI()
	{
		t = (MaterialDivide)target;

		GUILayout.Space(10);
		GUILayout.Label("查询当前节点中对应的材质球列表");	
		GUILayout.Space(10);
		t.iCount = EditorGUILayout.IntField("材质球个数", t.iCount);
		t.material1 = EditorGUILayout.TextField("材质球名字1", t.material1);
		t.material2 = EditorGUILayout.TextField("材质球名字2", t.material2);
		t.material3 = EditorGUILayout.TextField("材质球名字3", t.material3);
		GUILayout.Space(10);

		if(GUILayout.Button("开始")) 
		{
			findMaterial ();
		}  

		if(GUILayout.Button("显示Shader名字"))
		{
			MeshRenderer[] meshRendererArray = t.transform.GetComponentsInChildren<MeshRenderer>();
			for (int i = 0; i < meshRendererArray.Length; i++)
			{
				if(meshRendererArray[i] != null)
				{
					string shaderName = null;
					Material[] sharedMaterialsArray = meshRendererArray[i].sharedMaterials;
					for(int j = 0; j < sharedMaterialsArray.Length; j++)
					{
						shaderName += sharedMaterialsArray[j].shader.name + "|";
					}
					Debug.Log("shaderName : " + shaderName);
				}
			}
		}
	}

	private void findMaterial () 
	{
		GameObject findRoot = new GameObject();
		findRoot.name = "_findRoot_";

		MeshRenderer[] meshRendererArray = t.transform.GetComponentsInChildren<MeshRenderer>();

		for (int i = 0; i < meshRendererArray.Length; i++)
		{
			if(meshRendererArray[i] != null)
			{
				Material[] sharedMaterialsArray = meshRendererArray[i].sharedMaterials;
				if(sharedMaterialsArray != null)
				{
					if(sharedMaterialsArray.Length == t.iCount)
					{
						if(isEquals(sharedMaterialsArray))
						{
							meshRendererArray[i].gameObject.transform.parent = findRoot.transform;
							continue;
						}
					}	
				}
			}
		}

//		for (int i = 0; i < t.transform.childCount; i++)
//		{
//			GameObject obj = t.transform.GetChild(i).gameObject;
//			if(obj != null)
//			{
//				if(obj.GetComponent<MeshRenderer>() == null)
//					continue;
//				
//				Material[] sharedMaterialsArray = obj.GetComponent<MeshRenderer>().sharedMaterials;
//				if(sharedMaterialsArray.Length == 1 && t.iCount == 1)
//				{
//					if(sharedMaterialsArray[0].name.Equals(t.material1))
//					{
//						obj.transform.parent = findRoot.transform;
//						continue;
//					}
//				}
//				else if(sharedMaterialsArray.Length == t.iCount)
//				{
//					if(isEquals(sharedMaterialsArray))
//					{
//						obj.transform.parent = findRoot.transform;
//						continue;
//					}
//				}
//			}
//		}
	}

	private bool isEquals(Material[] materialArray)
	{
		bool retValue = false;
		int iFoundCnt = 0;
		for(int i = 0; i < materialArray.Length; i++)
		{
			if(materialArray[i].name.Equals(t.material1))
			{
				iFoundCnt++;
				break;
			}
		}

		for(int i = 0; i < materialArray.Length; i++)
		{
			if(materialArray[i].name.Equals(t.material2))
			{
				iFoundCnt++;
				break;
			}
		}

		for(int i = 0; i < materialArray.Length; i++)
		{
			if(materialArray[i].name.Equals(t.material3))
			{
				iFoundCnt++;
				break;
			}
		}

		if(iFoundCnt == t.iCount && t.iCount >= 1)
			retValue = true;

		return retValue;
	}
}
