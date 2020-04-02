using PFrame.Entities;
using Unity.Entities;
using Unity.Transforms;

namespace PFrame.Text3D
{
    //[UpdateAfter(typeof(UI3DIconFontSystem))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public class Text3DUpdateSystem : ComponentSystem
    {
        //private UI3DIconFontSystem fontSystem;
        private GameDataManagerSystem gameDataManagerSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            //fontSystem = World.GetExistingSystem<UI3DIconFontSystem>();
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            gameDataManagerSystem = World.GetExistingSystem<GameDataManagerSystem>();
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref Text3D text3D, ref SetText3DCmd setTextCmd) =>
            {
                var text = text3D.Text;
                var newText = setTextCmd.Text;

                EntityManager.RemoveComponent<SetText3DCmd>(entity);

                if (text.Equals(newText))
                    return;

                text3D.Text = newText;
                //textComp.IsNeedUpdate = true;
                if (EntityManager.HasComponent<UpdatedState>(entity))
                    EntityManager.RemoveComponent<UpdatedState>(entity);
            });

            Entities
                .WithNone<UpdatedState>()
                .ForEach((Entity entity, ref Text3D text3D) =>
            {
                if (!gameDataManagerSystem.GetGameData<Font3DData>(text3D.FontId, out var font3DData))
                    return;

                Font3DUtil.UpdateText3D(EntityManager, entity, font3DData);
                EntityManager.AddComponent<UpdatedState>(entity);
            });

            Entities
                .WithNone<Parent>()
                .ForEach((Entity entity, ref Char3D char3D) =>
            {
                EntityManager.DestroyEntity(entity);
            });

        }
    }
}