using PFrame.Entities;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

namespace PFrame.Text3D
{

    public struct Text3D : IComponentData
    {
        public ushort FontId;
        public NativeString128 Text;

        public float FontSize;
        //public bool IsNeedUpdate;
        public EHAlignType HAlignType;
        public EVAlignType VAlignType;

        public float HSpacing;
        public float VSpacing;
    }

    public struct Char3D : IComponentData
    {
        public char Char;
    }

    public struct SetText3DCmd : IComponentData
    {
        public NativeString128 Text;
    }

    public struct Char3DElement : IBufferElementData
    {
        public char Char;

        public Entity CharEntity;
    }

    //public class TextLayoutData : IDisposable
    //{
    //    public float Width;
    //    public float Height;

    //    public float MinX;
    //    public float MinY;

    //    public NativeList<LineLayoutData> LineLayoutDataList;

    //    public void Dispose()
    //    {
    //        if (LineLayoutDataList.IsCreated)
    //            LineLayoutDataList.Dispose();
    //    }
    //}

    public struct LineLayoutData
    {
        public int LineIndex;
        public int CharIndex;
        public int CharNum;

        public float Width;
        public float Height;

        public float MinX;
        public float MinY;
    }

}
