//=============================================================================
// <summary>
// 3StageAttack
// </summary>
// <author>CGC_12_小宮 孝介</author>
//=============================================================================
using app;
using via;
using via.attribute;
using via.behaviortree;
using via.motion;
using via.physics;
using static blackfilter.EnemyGasGeneratorManager;

namespace blackfilter
{
    public class GorillaThreeStageAttack : Action
    {
        [DataMember, DisplayName("最大移動距離")]
        private float maxMovementDistance = 5.0f;
        [DataMember, DisplayName("総移動時間")]
        private float maxMovemntTime = 3.0f;
        [DataMember, DisplayName("攻撃後停止時間")]
        private float attackEndStopTime = 5.0f;
        [DataMember, DisplayName("移動速度")]
        private float attackMoveSpeed = 3.0f;
        [DataMember, DisplayName("右腕ガスのエフェクトID")]
        public uint effectIDR = 0;
        [DataMember, DisplayName("左腕ガスのエフェクトID")]
        public uint effectIDL = 0;
        [DataMember, DisplayName("ガススケール")]
        public vec3 gasScale = new vec3(1.0f, 1.0f, 1.0f);
        [DataMember, DisplayName("石化減衰率")]
        public float attenuationRate = 0.2f;
        [DataMember, DisplayName("エフェクト消失時間")]
        public float destroyTime = 5.0f;
        [DataMember]
        public Prefab armREffectPrefab;
        [DataMember]
        public Prefab armLEffectPrefab;

        private bool isInstantiateArmEffectPrefab = false;
        private EnemyMotionController motionController;
        private float timer = 0;
        private float nowMoveDistance = 0;
        private GameObject ownerObj;
        private GameObject target;
        private GameObject armREffectPosObj;
        private GameObject armLEffectPosObj;
        private int phase = 0;
        private NavigationController navigationController;
        private Character cpCharacter;

        public override void start(ActionArg arg)
        {
            if (target == null)
            {
                target = PlayerManager.Instance.getPlayer().GameObject;
            }

            ownerObj = arg.OwnerGameObject;
            
            cpCharacter = ownerObj.getSameComponent<Character>();
            motionController = ownerObj.getSameComponent<EnemyMotionController>();
            navigationController = ownerObj.getSameComponent<NavigationController>();

            var isThreeStageAttack = new via.userdata.VariableBoolHandle(arg.UserVariables, via.str.makeHash("isThreeStageAttack"));
            if (!isThreeStageAttack.Value) isThreeStageAttack.Value = true;

            Transform rightArm =  ownerObj.Transform.find("RightArm");
            Transform leftArm =  ownerObj.Transform.find("LeftArm");

            motionController.setMotion((int)GorillaEnemy.MotionLayer.Base, (int)GorillaEnemy.MotionBankID.Battle, (int)GorillaEnemy.MotionID.Chase);
            motionController.setMotion((int)GorillaEnemy.MotionLayer.Base, (int)GorillaEnemy.MotionBankID.Battle, (int)GorillaEnemy.MotionID.ThreeStageAttack);

            if (!isInstantiateArmEffectPrefab)
            {
                isInstantiateArmEffectPrefab = true;
                armREffectPosObj = armREffectPrefab.instantiate(rightArm.Position);
                armREffectPosObj.Transform.setParent(rightArm, true);
                armREffectPosObj.Transform.LocalPosition = vec3.Zero;
                armLEffectPosObj = armLEffectPrefab.instantiate(leftArm.Position);
                armLEffectPosObj.Transform.setParent(leftArm, true);
                armLEffectPosObj.Transform.LocalPosition = vec3.Zero;
            }

            EnemyGasEffectData effectDataR = new EnemyGasEffectData(effectIDR, 1, Quaternion.Identity, GasId.Monochrome);
            EnemyGasGeneratorManager.Instance.generateGas(effectDataR, armREffectPosObj.Transform.Position, gasScale, armREffectPosObj.Transform.Rotation, attenuationRate, destroyTime);
            EnemyGasEffectData effectDataL = new EnemyGasEffectData(effectIDL, 1, Quaternion.Identity, GasId.Monochrome);
            EnemyGasGeneratorManager.Instance.generateGas(effectDataL, armLEffectPosObj.Transform.Position, gasScale, armLEffectPosObj.Transform.Rotation, attenuationRate, destroyTime);

        }

        public override void end(ActionArg arg)
        {
            debug.infoLine("ThreeStageAttack END");
        }

        public override void update(ActionArg arg)
        {
            timer += Application.ElapsedSecond;
            phaseManager(arg);
            cpCharacter.setRotationY(getTargetRotY(navigationController.NavigationDirection));
            cpCharacter.updateRotation();
            //debug.infoLine($"distance : {getTargetDistance()}");
        }

        private void attackMove()
        {
            float movementDistance = maxMovementDistance / 2.0f;

            if (target is not null)
            {
                var target_dist = getTargetDistance();

                if (movementDistance >= nowMoveDistance && target_dist > 0.5)
                {
                    cpCharacter.addMoveDirection(ownerObj.Transform.AxisZ);
                    nowMoveDistance += attackMoveSpeed / Application.BaseFps * Application.DeltaTime;
                    //debug.infoLine($"nowMoveDistance : {nowMoveDistance}");
                }
            }
        }

        private void phaseManager(ActionArg arg)
        {
            switch (phase)
            {
                case 0:
                    attackMove();
                    //debug.infoLine("phase 0");
                    if (timer > maxMovemntTime / 3)
                    {
                        timer = 0;
                        phase = 1;
                        nowMoveDistance = 0;
                    }
                    break;
                case 1:
                    attackMove();
                    //debug.infoLine("phase 1");
                    if (timer > maxMovemntTime / 3)
                    {
                        timer = 0;
                        phase = 2;
                        nowMoveDistance = 0;
                    }
                    break;
                case 2:
                    //debug.infoLine("phase 2");
                    if (timer > maxMovemntTime / 3)
                    {
                        timer = 0;
                        phase = 3;
                        nowMoveDistance = 0;
                    }
                    break;
                case 3:
                    //debug.infoLine("phase 3");
                    if (timer > attackEndStopTime && motionController.isEndMotion((int)GorillaEnemy.MotionLayer.Base))
                    {
                        timer = 0;
                        phase = 0;
                        nowMoveDistance = 0;
                        var isThreeStageAttack = new via.userdata.VariableBoolHandle(arg.UserVariables, via.str.makeHash("isThreeStageAttack"));
                        isThreeStageAttack.Value = false;
                        arg.notifyNodeEnd();
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// ターゲットに向くためのラジアンを求める
        /// </summary>
        private float getTargetRotY(vec3 target_dir)
        {
            target_dir.y = 0.0f;
            target_dir = vector.normalizeFast(target_dir);

            return math.atan2(target_dir.x, target_dir.z);
        }

        private float getTargetDistance()
        {
            vec3 targetPos = target.Transform.Position;
            vec3 enemyPos = ownerObj.Transform.Position;
            float targetDistance = vector.length(targetPos - enemyPos);
            return targetDistance;
        }
    }
}