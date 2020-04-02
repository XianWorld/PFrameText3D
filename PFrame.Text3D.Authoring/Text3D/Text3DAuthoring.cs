using PFrame.Entities;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace PFrame.Text3D.Authoring
{
    public class Text3DAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public Font3DDataSO FontData;
        //public Font3DAuthoring FontAuthoring;
        [Multiline]
        public string Text = "";
        public float FontSize = 1f;
        public EHAlignType HAlignType;
        public EVAlignType VAlignType;
        public float HSpacing;
        public float VSpacing;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            ushort fontId = 0;
            if (FontData != null)
                fontId = FontData.DataId;

            //bool isNeedUpdate = Text != "";
            var textComp = new Text3D
            {
                FontId = fontId,
                Text = Text,
                FontSize = FontSize,
                HAlignType = HAlignType,
                VAlignType = VAlignType,
                HSpacing = HSpacing,
                VSpacing = VSpacing,
                //IsNeedUpdate = isNeedUpdate
            };
            dstManager.AddComponentData(entity, textComp);

            var buffer = dstManager.AddBuffer<Char3DElement>(entity);

            //UnityEngine.Debug.Log("UI3DIconTextAuthoring: " + entity);
        }

        private void OnDrawGizmos()
        {
            if (FontData == null || FontData.Prefab == null)
                return;

            var FontAuthoring = FontData.Prefab.GetComponent<Font3DAuthoring>();

            Gizmos.color = UnityEngine.Color.blue;

            var fontChar3Ds = FontAuthoring.FontChar3Ds;
            var minCharCode = Font3DData.MinCharCode; //FontAuthoring.StartChar;
            var fontCharNum = Font3DData.CharNum;
            var charWidth = FontAuthoring.CharWidth;
            var charHeight = FontAuthoring.CharHeight;

            //add new char elements
            var text = Text;

            //if (!FontAuthoring.IsSupportLowerCase)
            //    text = text.ToUpper();
            var chars = text.ToCharArray();

            var len = chars.Length;
            if (len > 0)
            {
                var text3D = new Text3D
                {
                    FontSize = FontSize,
                    HAlignType = HAlignType,
                    VAlignType = VAlignType,
                    HSpacing = HSpacing,
                    VSpacing = VSpacing,
                    Text = Text,
                };

                //var fontCharDataArray = new NativeArray<FontChar3D>(fontChar3Ds, Allocator.Temp);
                Font3DUtil.ProcessText3D<FontChar3D>(text3D, fontChar3Ds, charWidth, charHeight, (fontChar3D, x, y) =>
                {
                    var prefab = fontChar3D.Prefab;
                    var prefabMatrix = prefab.transform.localToWorldMatrix;
                    var a = transform.localToWorldMatrix * prefabMatrix;
                    var rot = a.rotation;
                    //var pos = a.MultiplyPoint(Vector3.zero);
                    var scale = Vector3.Scale(transform.lossyScale, prefab.transform.localScale);
                    scale *= FontSize;
                    float4x4 b = a;

                    var pos = transform.TransformPoint(new Vector3(x, y, 0f));
                    var mesh = prefab.GetComponent<MeshFilter>().sharedMesh;
                    Gizmos.DrawMesh(mesh, 0, pos, rot, scale);
                });

                //fontCharDataArray.Dispose();

                ////calculate x, y 
                //var minX = 0f;
                //var minY = 0f;
                //var textWidth = 0f;// = len * charWidth;
                //var textHeight = charHeight;
                //var line = 0;

                //for (int i = 0; i < len; i++)
                //{
                //    if(i > 0)
                //        textWidth += HSpacing;

                //    var charCode = (int)chars[i];
                //    if (charCode == '~')
                //    {
                //        textHeight += charHeight;
                //        if (line > 0)
                //            textHeight += VSpacing;
                //        line++;
                //        continue;
                //    }

                //    var index = charCode - minCharCode;
                //    if (index < 0 || index >= fontCharNum)
                //    {
                //        textWidth += charWidth;
                //        continue;
                //    }

                //    var prefab = fontChar3Ds[index].Prefab;
                //    if (prefab == null)
                //    {
                //        textWidth += charWidth;
                //        continue;
                //    }

                //    textWidth += fontChar3Ds[index].Size.x;
                //}

                //if (HAlignType == EHAlignType.Middle)
                //{
                //    //minX = -(textWidth + HSpacing * (len - 1)) * 0.5f;
                //    minX = -(textWidth) * 0.5f;
                //}
                //else if (HAlignType == EHAlignType.Right)
                //{
                //    //minX = -(textWidth + HSpacing * (len - 1));
                //    minX = -(textWidth);
                //}

                //if (VAlignType == EVAlignType.Middle)
                //{
                //    minY = -(textHeight) * 0.5f;
                //}
                //else if (VAlignType == EVAlignType.Top)
                //{
                //    minY = -(textHeight);
                //}

                //float textOffsetX = 0f;
                //float textOffsetY = 0f;
                //line = 0;
                //for (int i = 0; i < len; i++)
                //{
                //    if (i > 0)
                //        textOffsetX += HSpacing;

                //    var charCode = (int)chars[i];
                //    if (charCode == '~')
                //    {
                //        textOffsetY += charHeight;
                //        if (line > 0)
                //            textOffsetY += VSpacing;
                //        line++;
                //        continue;
                //    }

                //    var index = charCode - minCharCode;
                //    if (index < 0 || index >= fontCharNum)
                //    {
                //        textOffsetX += charWidth;
                //        continue;
                //    }

                //    var fontChard3D = fontChar3Ds[index];
                //    var prefab = fontChard3D.Prefab;
                //    if (prefab == null)
                //    {
                //        textOffsetX += charWidth;
                //        continue;
                //    }

                //    var prefabMatrix = prefab.transform.localToWorldMatrix;
                //    var a = transform.localToWorldMatrix * prefabMatrix;
                //    var rot = a.rotation;
                //    //var pos = a.MultiplyPoint(Vector3.zero);
                //    var scale = Vector3.Scale(transform.lossyScale, prefab.transform.localScale);
                //    float4x4 b = a;

                //    textOffsetX += fontChard3D.Size.x;

                //    var x = minX + textOffsetX + fontChard3D.Min.x;
                //    var y = minY + textOffsetY;
                //    var pos = transform.TransformPoint(new Vector3(x, y, 0f));
                //    var mesh = prefab.GetComponent<MeshFilter>().sharedMesh;
                //    Gizmos.DrawMesh(mesh, 0, pos, rot, scale);

                //    //Debug.LogFormat("{0}, {1}, {2}, {3}, {4}", i, textOffsetX, fontChard3D.Size, x, y);
                //}
            }
        }
    }
}
