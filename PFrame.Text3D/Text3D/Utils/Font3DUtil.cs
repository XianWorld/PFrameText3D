using PFrame.Entities;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace PFrame.Text3D
{
    public unsafe class Font3DUtil
    {
        //public static Entity CreateCharEntity(EntityManager entityManager, Entity textEntity, char textChar)
        //{
        //    var charEntity = Entity.Null;

        //    var charCode = (int)textChar;
        //    var index = charCode - Font3DData.MinCharCode;
        //    if (index < 0 || index >= Font3DData.CharNum)
        //        return charEntity;

        //    var iconElement = iconElements[index];
        //    var prefab = iconElement.CharPrefab;
        //    if (prefab == Entity.Null)
        //        return charEntity;

        //    charEntity = CreateCharEntity(entityManager, prefab);

        //    entityManager.AddComponentData(charEntity, new Char3D { Char = textChar });

        //    EntityUtil.SetParent(entityManager, charEntity, textEntity);

        //    return charEntity;
        //}

        private static Entity CreateChar3DEntity(EntityManager entityManager, Entity prefab, int textChar)
        {
            return entityManager.Instantiate(prefab);
        }

        private static void DestroyChar3DEntity(EntityManager entityManager, Entity charEntity, int textChar)
        {
            entityManager.DestroyEntity(charEntity);
        }

        private static FontChar3DElement[] fontChar3DElementBuf = new FontChar3DElement[Font3DData.CharNum];

        public static bool UpdateText3D(EntityManager entityManager, Entity textEntity, Font3DData fontData)
        {
            var text3D = entityManager.GetComponentData<Text3D>(textEntity);
            var text = text3D.Text;
            var fontSize = text3D.FontSize;
            //var HAlignType = text3D.HAlignType;
            //var VAlignType = text3D.VAlignType;
            //var HSpacing = text3D.HSpacing;
            //var VSpacing = text3D.VSpacing;

            var buffer = entityManager.GetBuffer<Char3DElement>(textEntity);

            //remove all char elements
            foreach (var element in buffer)
            {
                if (element.CharEntity != Entity.Null)
                {
                    DestroyChar3DEntity(entityManager, element.CharEntity, element.Char);
                }
            }

            //add new char elements
            var len = text.LengthInBytes;

            if (len > 0)
            {
                var fontEntity = fontData.Prefab;
                var font3D = entityManager.GetComponentData<Font3D>(fontEntity);
                var fontCharElementBuffer = entityManager.GetBuffer<FontChar3DElement>(fontEntity);
                //var fontCharElementArray = fontCharElementBuffer.ToNativeArray(Allocator.Temp);
                var charWidth = font3D.CharWidth;
                var charHeight = font3D.CharHeight;

                //var buf = new NativeArray<Char3DElement>(len, Allocator.Temp);
                //var aa = fontCharElementArray.ToArray();
                //var ll = fontCharElementBuffer.Length;
                //var aa = new FontChar3DElement[ll];
                for(int i = 0; i < Font3DData.CharNum; i++)
                {
                    fontChar3DElementBuf[i] = fontCharElementBuffer[i];
                }

                var buf = new NativeList<Char3DElement>(Allocator.Temp);
                Font3DUtil.ProcessText3D<FontChar3DElement>(text3D, fontChar3DElementBuf, charWidth, charHeight, (fontChar3D, x, y) =>
                {
                    var prefab = fontChar3D.Prefab;
                    var textChar = fontChar3D.Char;

                    var charEntity = CreateChar3DEntity(entityManager, prefab, textChar);
                    entityManager.AddComponentData(charEntity, new Char3D { Char = textChar });
                    entityManager.SetOrAddComponentData(charEntity, new NonUniformScale { Value = new float3(fontSize) });
                    EntityUtil.SetParent(entityManager, charEntity, textEntity);

                    //var charEntity = CreateCharEntity(entityManager, textEntity, textChar);

                    buf.Add(new Char3DElement { Char = textChar, CharEntity = charEntity });

                    var pos = entityManager.GetComponentData<Translation>(charEntity);
                    pos.Value = new float3(x, y, 0f);
                    entityManager.SetComponentData(charEntity, pos);
                });

                buffer = entityManager.GetBuffer<Char3DElement>(textEntity);
                buffer.Clear();
                buffer.AddRange(buf.AsArray());

                buf.Dispose();

                //fontCharElementArray.Dispose();

                //unsafe
                //{
                //    byte* b = &text.buffer.byte0000;

                //    //calculate x, y 
                //    var minX = 0f;
                //    var minY = 0f;
                //    var textWidth = 0f;// = len * charWidth;
                //    var textHeight = charHeight;

                //    for (int i = 0; i < len; i++)
                //    {
                //        var textChar = (char)b[i];

                //        if (i > 0)
                //            textWidth += HSpacing;

                //        var charCode = (int)textChar;
                //        var index = charCode - Font3DData.MinCharCode;
                //        if (index < 0 || index >= Font3DData.CharNum)
                //        {
                //            textWidth += charWidth;
                //            continue;
                //        }

                //        var fontCharElement = fontCharElementArray[index];
                //        var prefab = fontCharElement.Prefab;
                //        if (prefab == Entity.Null)
                //        {
                //            textWidth += charWidth;
                //            continue;
                //        }

                //        textWidth += fontCharElement.Size.x;
                //    }

                //    if (HAlignType == EHAlignType.Middle)
                //    {
                //        minX = -(textWidth) * 0.5f;
                //    }
                //    else if (HAlignType == EHAlignType.Right)
                //    {
                //        minX = -(textWidth);
                //    }
                //    if (VAlignType == EVAlignType.Middle)
                //    {
                //        minY = -(textHeight) * 0.5f;
                //    }
                //    else if (VAlignType == EVAlignType.Top)
                //    {
                //        minY = -(textHeight);
                //    }

                //    var buf = new NativeArray<Char3DElement>(len, Allocator.Temp);
                //    float textOffsetX = 0f;
                //    for (int i = 0; i < len; i++)
                //    {
                //        var textChar = (char)b[i];
                //        //if (!IsSupportLowerCase)
                //        //{
                //        //    if (cc >= 97 && cc <= 122)
                //        //        cc = (char)(cc - 32);
                //        //}

                //        if (i > 0)
                //            textOffsetX += HSpacing;

                //        var charCode = (int)textChar;
                //        var index = charCode - Font3DData.MinCharCode;
                //        if (index < 0 || index >= Font3DData.CharNum)
                //        {
                //            textOffsetX += charWidth;
                //            continue;
                //        }

                //        var fontCharElement = fontCharElementArray[index];
                //        var prefab = fontCharElement.Prefab;
                //        if (prefab == Entity.Null)
                //        {
                //            textOffsetX += charWidth;
                //            continue;
                //        }

                //        var charEntity = CreateChar3DEntity(entityManager, prefab, textChar);
                //        entityManager.AddComponentData(charEntity, new Char3D { Char = textChar });
                //        EntityUtil.SetParent(entityManager, charEntity, textEntity);

                //        //var charEntity = CreateCharEntity(entityManager, textEntity, textChar);
                //        buf[i] = (new Char3DElement { Char = textChar, CharEntity = charEntity });

                //        textOffsetX += fontCharElement.Size.x;

                //        var x = minX + textOffsetX + fontCharElement.Min.x;
                //        var y = minY;
                //        var pos = entityManager.GetComponentData<Translation>(charEntity);
                //        pos.Value = new float3(x, y, 0f);
                //        entityManager.SetComponentData(charEntity, pos);
                //    }

                //    buffer = entityManager.GetBuffer<Char3DElement>(textEntity);
                //    buffer.Clear();
                //    buffer.AddRange(buf);

                //    buf.Dispose();
                //}

            }

            return true;
        }

        public static void ProcessText3D<T>(Text3D text3D, T[] charDatas, float charWidth, float charHeight, Action<T, float, float> action)
            where T : struct, IFontChar3DData
        {
            var text = text3D.Text;
            byte* chars = &text.buffer.byte0000;
            int charNum = text.LengthInBytes;
            var hAlignType = text3D.HAlignType;
            var vAlignType = text3D.VAlignType;
            var hSpacing = text3D.HSpacing;
            var vSpacing = text3D.VSpacing;
            var fontSize = text3D.FontSize;

            charWidth *= fontSize;
            charHeight *= fontSize;

            //calculate x, y 
            var textMinX = 0f;
            var textMinY = 0f;
            var textWidth = 0f;// = len * charWidth;
            var textHeight = charHeight;
            var lineWidth = 0f;
            var lineMinX = 0f;
            var lineMinY = 0f;

            //var textLayoutData = new TextLayoutData();
            var lineLayoutDataList = new NativeList<LineLayoutData>(Allocator.Temp);

            var line = 0;
            int lineCharIndex = 0;
            LineLayoutData lineLayoutData;
            for (int i = 0; i < charNum; i++)
            {
                var charCode = chars[i];

                if (charCode == '\n')
                {
                    textHeight += charHeight;
                    textHeight += vSpacing;

                    if (hAlignType == EHAlignType.Middle)
                    {
                        lineMinX = -(lineWidth) * 0.5f;
                    }
                    else if (hAlignType == EHAlignType.Right)
                    {
                        lineMinX = -(lineWidth);
                    }

                    lineLayoutData = new LineLayoutData();
                    lineLayoutData.LineIndex = line;
                    lineLayoutData.CharIndex = lineCharIndex;
                    lineLayoutData.CharNum = i - lineCharIndex;
                    lineLayoutData.Width = lineWidth;
                    lineLayoutData.MinX = lineMinX;
                    lineLayoutDataList.Add(lineLayoutData);

                    line++;
                    lineCharIndex = i + 1;

                    textMinX = math.min(textMinX, lineMinX);
                    textWidth = math.max(textWidth, lineWidth);
                    lineWidth = 0f;
                    continue;
                }

                if (i > 0)
                    lineWidth += hSpacing;

                var index = charCode - Font3DData.MinCharCode;
                if (index < 0 || index >= Font3DData.CharNum)
                {
                    lineWidth += charWidth;
                    continue;
                }

                var fontCharData = charDatas[index];
                if (!fontCharData.IsValid)
                {
                    lineWidth += charWidth;
                    continue;
                }

                lineWidth += fontCharData.BoundsSize.x * fontSize;
            }

            //last line
            if (hAlignType == EHAlignType.Middle)
            {
                lineMinX = -(lineWidth) * 0.5f;
            }
            else if (hAlignType == EHAlignType.Right)
            {
                lineMinX = -(lineWidth);
            }

            lineLayoutData = new LineLayoutData();
            lineLayoutData.LineIndex = line;
            lineLayoutData.CharIndex = lineCharIndex;
            lineLayoutData.CharNum = charNum - lineCharIndex;
            lineLayoutData.Width = lineWidth;
            lineLayoutData.MinX = lineMinX;
            lineLayoutDataList.Add(lineLayoutData);

            line++;

            textMinX = math.min(textMinX, lineMinX);
            textWidth = math.max(textWidth, lineWidth);

            if (vAlignType == EVAlignType.Middle)
            {
                textMinY = -(textHeight) * 0.5f;
            }
            else if (vAlignType == EVAlignType.Top)
            {
                textMinY = -(textHeight);
            }

            //textLayoutData.Width = textWidth;
            //textLayoutData.Height = textHeight;
            //textLayoutData.MinX = textMinX;
            //textLayoutData.MinY = textMinY;
            //textLayoutData.LineLayoutDataList = lineLayoutDataList;

            for (int j = 0; j < line; j++)
            {
                float lineOffsetX = 0f;
                float lineOffsetY = 0f;
                lineLayoutData = lineLayoutDataList[j];
                lineCharIndex = lineLayoutData.CharIndex;
                lineMinX = lineLayoutData.MinX;
                lineMinY = lineLayoutData.MinY;
                var lineCharNum = lineLayoutData.CharNum;
                lineOffsetY = textMinY + (line - 1 - j) * (charHeight + vSpacing);

                for (int i = 0; i < lineCharNum; i++)
                {
                    if (i > 0)
                        lineOffsetX += hSpacing;

                    var charCode = chars[lineCharIndex + i];

                    var index = charCode - Font3DData.MinCharCode;
                    if (index < 0 || index >= Font3DData.CharNum)
                    {
                        lineOffsetX += charWidth;
                        continue;
                    }

                    var fontChard3D = charDatas[index];
                    if (!fontChard3D.IsValid)
                    {
                        lineOffsetX += charWidth;
                        continue;
                    }

                    lineOffsetX += fontChard3D.BoundsSize.x * fontSize;
                    var x = lineMinX + lineOffsetX + fontChard3D.BoundsMin.x * fontSize;
                    var y = lineOffsetY;
                    action(fontChard3D, x, y);

                    //var prefabMatrix = prefab.transform.localToWorldMatrix;
                    //var a = transform.localToWorldMatrix * prefabMatrix;
                    //var rot = a.rotation;
                    ////var pos = a.MultiplyPoint(Vector3.zero);
                    //var scale = Vector3.Scale(transform.lossyScale, prefab.transform.localScale);
                    //float4x4 b = a;

                    //textOffsetX += fontChard3D.Size.x;

                    //var x = minX + textOffsetX + fontChard3D.Min.x;
                    //var y = minY + textOffsetY;
                    //var pos = transform.TransformPoint(new Vector3(x, y, 0f));
                    //var mesh = prefab.GetComponent<MeshFilter>().sharedMesh;
                    //Gizmos.DrawMesh(mesh, 0, pos, rot, scale);

                    //Debug.LogFormat("{0}, {1}, {2}, {3}, {4}", i, textOffsetX, fontChard3D.Size, x, y);
                }
            }

            lineLayoutDataList.Dispose();
            //return textLayoutData;
        }

        public static void SetText(EntityManager entityManager, Entity entity, string text)
        {
            var text3D = entityManager.GetComponentData<Text3D>(entity);
            if (text3D.Text.Equals(text))
                return;

            text3D.Text = text;
            entityManager.SetComponentData<Text3D>(entity, text3D);

            if (entityManager.HasComponent<UpdatedState>(entity))
                entityManager.RemoveComponent<UpdatedState>(entity);
        }
    }
}