//=============================================================================
// <summary>
// EnemyWandering_P 
// </summary>
// <author>CGC_12_小宮 孝介</author>
//=============================================================================
using via;
using via.attribute;
using via.behaviortree;
using via.physics;
using app;
using static blackfilter.EnemyGasGeneratorManager;

namespace blackfilter
{
    public class GorillaRush : via.behaviortree.Action
    {
        [DataMember, DisplayName("突進の速度")]
        private float rushSpeed = 1;
        [DataMember, DisplayName("壁との接触判定を行うRayの長さ")]
        private float rayLength = 1;
        [DataMember, DisplayName("最初の角度を合わせる時間")]
        private float angleCheckInterval = 2.0f;
        [DataMember, DisplayName("突進時間")]
        private float rushDuration = 5.0f;
        [DataMember, DisplayName("壁に当たらなかった場合の停止時間")]
        public float RecoveryInterval = 2.0f;
        [DataMember, DisplayName("壁に当たった場合の停止時間")]
        public float WallRecoveryInterval = 6.0f;
        [DataMember, DisplayName("突進のSoundID")]
        public int rushSoundID = 0;
        [DataMember, DisplayName("防御ガスのエフェクトID")]
        public uint effectID = 0;
        [DataMember, DisplayName("ガススケール")]
        public vec3 gasScale = new vec3(1.0f, 1.0f, 1.0f);
        [DataMember, DisplayName("石化減衰率")]
        public float attenuationRate = 0.2f;
        [DataMember, DisplayName("エフェクト消失時間")]
        public float destroyTime = 5.0f;

        private bool isHitWall = false;
        private bool test = false;
        private Character cpCharacter;
        private float timer = 0;
        private GameObject ownerObj;
        private GameObject target;
        private int phase = 0;
        private int motionPhase = 0;
        private GorillaEnemy gorillaEnemy;
        private NavigationController navigationController;

        public override void start(ActionArg arg)
        {
            if (target == null)
            {
                target = PlayerManager.Instance.getPlayer().GameObject;
            }
            ownerObj = arg.OwnerGameObject;

            var isRush = new via.userdata.VariableBoolHandle(arg.UserVariables, via.str.makeHash("isRush"));
            if (!isRush.Value) isRush.Value = true;

            var canRush = new via.userdata.VariableBoolHandle(arg.UserVariables, via.str.makeHash("canRush"));
            canRush.Value = false;

            var rotationSpeedVariable = new via.userdata.VariableSingleHandle(arg.UserVariables, via.str.makeHash("rotationSpeed"));

            cpCharacter = ownerObj.getSameComponent<Character>();
            gorillaEnemy = ownerObj.getSameComponent<GorillaEnemy>();
            navigationController = ownerObj.getSameComponent<NavigationController>();

            cpCharacter.MoveSpeed = rushSpeed;
            gorillaEnemy.MotionController.setMotion((int)GorillaEnemy.MotionLayer.Base, (int)GorillaEnemy.MotionBankID.Battle, (int)GorillaEnemy.MotionID.Guard);
            EnemyGasEffectData effectData = new EnemyGasEffectData(effectID, 1, Quaternion.Identity, GasId.Monochrome);
            EnemyGasGeneratorManager.Instance.generateGas(effectData, ownerObj.Transform.Position, gasScale, ownerObj.Transform.Rotation, attenuationRate, destroyTime);
        }

        public override void end(ActionArg arg)
        {
            debug.infoLine("Rush End");
        }

        public override void update(ActionArg arg)
        {
            timer += Application.ElapsedSecond;
            //debug.infoLine($"timer : {timer}");
            motionManager();
            phaseManager(arg);

            switch (phase)
            {
                case 0:
                    cpCharacter.setRotationY(getTargetRotY(navigationController.NavigationDirection));
                    cpCharacter.updateRotation();
                    break;
                case 1:
                    rushMove(ownerObj);
                    //cpRequestSetCollider.registerRequestSet(0, 3);
                    break;
                case 2:
                    // wait
                    break;
                case 3:
                    var isRush = new via.userdata.VariableBoolHandle(arg.UserVariables, via.str.makeHash("isRush"));
                    isRush.Value = false;
                    timer = 0;
                    phase = 0;
                    motionPhase = 0;
                    arg.notifyNodeEnd();
                    break;
            }
        }

        private void rushMove(GameObject obj)
        {
            cpCharacter.addMoveDirection(ownerObj.Transform.AxisZ);
        }

        private bool contactWall(ActionArg arg)
        {
            CastRayQuery query = new CastRayQuery();
            FilterInfo filter = new()
            {
                Layer = via.physics.System.getLayerIndex("Character"),
            };
            filter.MaskBits = 0;

            // Rayの開始地点と終着地点の設定
            vec3 start = arg.OwnerGameObject.Transform.Position + new vec3(0, 1.5f, 0f);
            vec3 end = arg.OwnerGameObject.Transform.AxisZ + new vec3(0, 0.15f, 0);

            // Ray照射
            query.copyFilterInfo(filter);
            query.setRay(start, end, rayLength);

            CastRayResult test = via.physics.System.castRay(query);

            if (test.NumContactPoints > 0)
            {
                //debug.infoLine("Wall Hit");
                return true;
            }

            return false;
        }

        private void phaseManager(ActionArg arg)
        {
            switch (phase)
            {
                case 0:
                    if (timer > angleCheckInterval && gorillaEnemy.MotionController.isEndMotion((int)GorillaEnemy.MotionLayer.Base))
                    {
                        if(!test)
                        {
                            test = true;
                            gorillaEnemy.SoundController.play(rushSoundID);
                        }
                        phase = 1;
                        timer = 0;
                    }
                    break;
                case 1:
                    if (timer > rushDuration)
                    {
                        phase = 2;
                        isHitWall = false;
                        gorillaEnemy.SoundController.stop(rushSoundID);
                        timer = 0;
                        //debug.infoLine("phase 2");
                    }
                    if (contactWall(arg))
                    {
                        phase = 2;
                        isHitWall = true;
                        gorillaEnemy.SoundController.stop(rushSoundID);
                        timer = 0;
                        gorillaEnemy.MotionController.setMotion((int)GorillaEnemy.MotionLayer.Base, (int)GorillaEnemy.MotionBankID.Battle, (int)GorillaEnemy.MotionID.RushImpact);
                    }
                    break;
                case 2:
                    if (isHitWall)
                    {
                        if (timer > WallRecoveryInterval)
                        {
                            //debug.infoLine("phase 3");
                            phase = 3;
                            timer = 0;
                        }
                    }
                    else
                    {
                        if (timer > RecoveryInterval)
                        {
                            //debug.infoLine("phase 3");
                            phase = 3;
                            timer = 0;
                        }
                    }
                    break;
            }
        }

        private void motionManager()
        {
            if (gorillaEnemy.MotionController.isEndMotion((int)GorillaEnemy.MotionLayer.Base))
            {
                if (motionPhase == 0)
                {
                    motionPhase = 1;
                    gorillaEnemy.MotionController.setMotion((int)GorillaEnemy.MotionLayer.Base, (int)GorillaEnemy.MotionBankID.Battle, (int)GorillaEnemy.MotionID.Rush);
                    gorillaEnemy.SoundController.play(rushSoundID);
                }
                else if (motionPhase == 1)
                {
                    motionPhase = 2;
                }
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
    }
}