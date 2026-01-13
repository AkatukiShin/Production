//=============================================================================
// <summary>
// GorillaContact 
// </summary>
// <author>CGC_12_小宮 孝介</author>
//=============================================================================
using via.attribute;
using via;
using via.behaviortree;
using static blackfilter.EnemyGasGeneratorManager;
using app;

namespace blackfilter
{
	public class GorillaIntimidation : Action
	{
        [DataMember]
        public float gasSpan = 0.5f;
        [DataMember]
        public vec3 gasScale = new vec3(1.0f, 1.0f, 1.0f);
        [DataMember]
        public vec3 offset = new vec3(0, 0.2f, 0);
        [DataMember, DisplayName("威嚇ガスエフェクトID")]
        public uint effectID = 3;
        [DataMember, DisplayName("石化減衰率")]
        public float attenurationRate = 0.2f;
        [DataMember, DisplayName("エフェクト消失時間")]
        public float  destroyTime = 5.0f;

        private EnemyMotionController motionController;
        private Transform ownerTransform;
        private GameObject ownerObj;
        private Character character;
        private NavigationController navigationController;

        public override void start(ActionArg arg)
        {
            debug.infoLine("Intimidation Start");
            ownerObj = arg.OwnerGameObject;
            ownerTransform = ownerObj.Transform;

            motionController = ownerObj.getSameComponent<EnemyMotionController>();
            character = ownerObj.getSameComponent<Character>();
            navigationController = ownerObj.getSameComponent<NavigationController>();

            var canIntimidation = new via.userdata.VariableBoolHandle(arg.UserVariables, via.str.makeHash("canIntimidation"));
            canIntimidation.Value = false;

            motionController.setMotion((int)GorillaEnemy.MotionLayer.Base, (int)GorillaEnemy.MotionBankID.Battle, (int)GorillaEnemy.MotionID.Intimidation);

            EnemyGasEffectData effectData = new EnemyGasEffectData(effectID, 1, Quaternion.Identity, GasId.Monochrome);
            EnemyGasGeneratorManager.Instance.generateGas(effectData, ownerTransform.Position + offset, gasScale, ownerTransform.Rotation, attenurationRate, destroyTime);
        }

        public override void update(ActionArg arg)
        {
            character.setRotationY(getTargetRotY(navigationController.NavigationDirection));
            character.updateRotation();

            if (motionController.isEndMotion((int)GorillaEnemy.MotionLayer.Base))
            {
                arg.notifyNodeEnd();
            }
        }

        public override void end(ActionArg arg)
        {

        }
        private float getTargetRotY(vec3 target_dir)
        {
            target_dir.y = 0.0f;
            target_dir = vector.normalizeFast(target_dir);
            return math.atan2(target_dir.x, target_dir.z);
        }
    }
}
