using PFrame.Entities;
using PFrame.Entities.Authoring;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace PFrame.Text3D.Authoring
{
    [GameDataAuthoring(Name = "Font3DData")]
    public class Font3DDataSOListConverter : AGameDataSOListConverter<Font3DData, Font3DDataSO>
    {
        public override byte TypeId => (byte)ECommonGameDataType.Font3D;

        public override Font3DData ConvertGameData(GameObject gameObject, Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem, Font3DDataSO dataSO)
        {
            var data = new Font3DData();
            data.Id = dataSO.Id;
            data.Name = dataSO.Name;

            //data.StageType = dataSO.StageType;
            data.Prefab = conversionSystem.GetPrimaryEntity(dataSO.Prefab);

            return data;
        }

        public override void DeclareReferencedPrefab(List<GameObject> referencedPrefabs, Font3DDataSO dataSO)
        {
            referencedPrefabs.Add(dataSO.Prefab);
        }
    }

    public class Font3DDataSOListAuthoring : AGameDataSOListAuthoring<Font3DData, Font3DDataSO, Font3DDataSOListConverter>
    {
    }
}
