using PFrame.Entities.Authoring;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace PFrame.Text3D.Authoring
{
    [Serializable]
    public struct FontChar3D : IFontChar3DData
    {
        public GameObject Prefab;
        public Vector3 Min;
        public Vector3 Max;
        public Vector3 Size;

        public float3 BoundsMin => Min;

        public float3 BoundsMax => Max;

        public float3 BoundsSize => Size;

        public bool IsValid => Prefab != null;

        public object GetPrefab()
        {
            return Prefab;
        }
    }

    public class Font3DAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        //public int Id;
        //public char StartChar;
        public float CharWidth = 0.01f;
        public float CharHeight = 0.01f;
        public string PrefabPath = "";

        //public GameObject[] Prefabs = new GameObject[Font3DData.CharNum];
        public FontChar3D[] FontChar3Ds = new FontChar3D[Font3DData.CharNum];
        //public bool IsSupportLowerCase;


        //private EditorUI3DIconFont font;
        //public EditorUI3DIconFont Font => font;

        private void OnValidate()
        {
            if (PrefabPath.IndexOf("Assets") < 0)
                return;

            string[] paths = EntityAuthoringUtil.GetAssetPaths(PrefabPath, ".prefab");

            for (int i = 0; i < paths.Length; i++)
            {
                var path = paths[i];
                var name = Path.GetFileNameWithoutExtension(path);
                int index = name.IndexOf('.');
                name = name.Substring(0, index);
                int charCode = 0;
                try
                {
                    charCode = int.Parse(name);
                }
                catch (System.Exception)
                {
                    Debug.LogErrorFormat("Font3DAuthoring: {0}, {1}", path, name);
                    continue;
                }

                if (charCode < Font3DData.MinCharCode || charCode > Font3DData.MaxCharCode)
                    continue;

                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                int charIndex = charCode - Font3DData.MinCharCode;
                //Prefabs[charIndex] = prefab;
                if (prefab != null)
                {
                    var mesh = prefab.GetComponent<MeshFilter>().sharedMesh;
                    var bounds = mesh.bounds;

                    var min = bounds.min;
                    var max = bounds.max;

                    FontChar3Ds[charIndex] = new FontChar3D
                    {
                        Prefab = prefab,
                        Min = bounds.min,
                        Max = bounds.max,
                        Size = bounds.size,
                    };
                }

                //Debug.LogErrorFormat("Font3DAuthoring: {0}, {1}, {2}", path, charCode, prefab);
            }
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var manager = new Font3D
            {
                //StartChar = StartChar,
                CharWidth = CharWidth,
                CharHeight = CharHeight,
                //IsSupportLowerCase = IsSupportLowerCase
            };
            dstManager.AddComponentData<Font3D>(entity, manager);

            var buffer = dstManager.AddBuffer<FontChar3DElement>(entity);

            for (int i = 0; i < FontChar3Ds.Length; i++)
            {
                var char3D = FontChar3Ds[i];
                var charPrefab = char3D.Prefab;
                var charEntity = conversionSystem.GetPrimaryEntity(charPrefab);

                var char3DElement = new FontChar3DElement
                {
                    Prefab = charEntity,
                    Min = char3D.Min,
                    Max = char3D.Max,
                    Size = char3D.Size,
                };
                buffer.Add(char3DElement);
            }
        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            for (int i = 0; i < FontChar3Ds.Length; i++)
            {
                if(FontChar3Ds[i].Prefab != null)
                    referencedPrefabs.Add(FontChar3Ds[i].Prefab);
            }
        }
    }
}
