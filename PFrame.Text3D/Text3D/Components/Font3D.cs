using Unity.Entities;
using Unity.Mathematics;

namespace PFrame.Text3D
{
    public struct Font3D : IComponentData
	{
        //public ushort DataId;

        public float CharWidth;
        public float CharHeight;
    }

    public interface IFontChar3DData
    {
        bool IsValid { get; }
        float3 BoundsMin { get; }
        float3 BoundsMax { get; }
        float3 BoundsSize { get; }
        //object GetPrefab();
    }

    public struct FontChar3DElement : IBufferElementData, IFontChar3DData
    {
        public Entity Prefab;
        public float3 Min;
        public float3 Max;
        public float3 Size;
        public char Char;

        public bool IsValid => Prefab != Entity.Null;

        public float3 BoundsMin => Min;

        public float3 BoundsMax => Max;

        public float3 BoundsSize => Size;

        //public object GetPrefab()
        //{
        //    return Prefab;
        //}
    }
}
