using PFrame.Entities;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace PFrame.Text3D
{
    public struct Font3DData : IBufferElementData, IGameData
    {
        public static int MinCharCode = 32;
        public static int MaxCharCode = 126;
        public static int CharNum = MaxCharCode - MinCharCode + 1;

        public ushort Id;
        public NativeString32 Name;
        public ushort DataId => Id;
        public byte DataType => (byte)ECommonGameDataType.Font3D;
        public string DataName => Name.ToString();

        public Entity Prefab;
    }
}
