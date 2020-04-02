using PFrame.Entities;
using PFrame.Entities.Authoring;
using UnityEngine;

namespace PFrame.Text3D.Authoring
{
    [CreateAssetMenu(fileName = "Font3DData_", menuName = "PFrame/GameData/Font3DData")]
    public class Font3DDataSO : AGameDataSO
    {
        public override byte DataType => (byte)ECommonGameDataType.Font3D;

        //public EStageType StageType;

        public GameObject Prefab;
    }
}
