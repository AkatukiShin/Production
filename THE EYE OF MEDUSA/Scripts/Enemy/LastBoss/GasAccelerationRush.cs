//=============================================================================
// <summary>
// GasAccelerationRush 
// </summary>
// <author>CGC_12_小宮 孝介</author>
//=============================================================================
using app;
using via;
using via.attribute;
using via.behaviortree;
using via.effect;
using via.physics;
using via.userdata;
using static blackfilter.EnemyGasGeneratorManager;

namespace blackfilter
{ 
    public class GasAccelerationRush : via.behaviortree.Action
    {
        [DataMember]
        private float gasScale = 10.0f;
        [DataMember]
        private float moveTime = 1.0f;
        [DataMember, DisplayName("突進速度")]
        private float rushSpeed = 25.0f;
        [DataMember, DisplayName("突進時の壁との接触判定を行うRayの長さ")]
        private float rayLength = 5.0f;
        [DataMember, DisplayName("突進時のボスの高さ")]
        private float rushHeight = 1.0f;
        [DataMember, DisplayName("方向定める時間")]
        private float rotationInterval = 2.0f;
        [DataMember, DisplayName("障害物に当たらなかった際に復帰するまでの時間")]
        private float recoveryInterval = 3.0f;
        [DataMember, DisplayName("後隙")]
        private float gap = 3.0f;
        [DataMember, DisplayName("攻撃判定ID")]
        private uint attackID = 6;

        [DataMember]
        private float angle = 90.0f;

        private float ownerAngleY = 0.0f;
        private bool isHitWall = false;
        private float timer = 0;
        private int phase = 0;
        private vec3 movePosition;
        private vec3 moveVector;
        private Character cpCharacter;
        private RequestSetCollider cpRequestSetCollider;
        private EnemyMotionController cpMotionController;
        private GameObject ownerObj;
        private NavigationController cpNavigationController;
        private VariableBoolHandle gasBlastRushCd;
        private LastBoss cpLastBossSource;

        public override void start(ActionArg arg)
        {
            ownerObj = arg.OwnerGameObject;

            if(cpCharacter == null)
            {
                cpCharacter = ownerObj.getSameComponent<Character>();
            }

            if(cpNavigationController == null)
            {
                cpNavigationController = ownerObj.getSameComponent<NavigationController>();
            }

            if(cpMotionController == null)
            {
                cpMotionController = ownerObj.getSameComponent<EnemyMotionController>();
            }

            if(cpLastBossSource == null)
            {
                cpLastBossSource = ownerObj.getSameComponent<LastBoss>();
                cpLastBossSource.SideStepEnabled = false;
            }

            if(cpRequestSetCollider == null)
            {
                cpRequestSetCollider = ownerObj.getSameComponent<RequestSetCollider>();
            }



            gasBlastRushCd = new via.userdata.VariableBoolHandle(arg.UserVariables, via.str.makeHash("GasBlastRushCd"));

            EnemyGasEffectData effectData = new()
            {
                EffectID = 21,
                GenerateDuration = 5,
                ParentRotation = Quaternion.Identity,
            };
            cpMotionController.setMotion((int)LastBoss.MotionLayer.BlastGas, (int)LastBoss.MotionBankID.Locomotion, (int)LastBoss.LocomotionMotionID.Empty);
            cpMotionController.setMotion((int)LastBoss.MotionLayer.Base, (int)LastBoss.MotionBankID.Battle, (int)LastBoss.BattleMotionID.GasCharge);
            effectData.GasId = GasId.Stealth;
            float scale = gasScale;
            EnemyGasGeneratorManager.Instance.generateGas(effectData, ownerObj.Transform.Position, vec3.One * scale, arg.OwnerGameObject.Transform.Rotation, 1.0f, 7);
        }

        public override void update(ActionArg arg)
        {
            timer += Application.ElapsedSecond;

            contactWall(arg);

            debug.infoLine($"phase : {phase}");

            switch(phase)
            {
                case 0:
                    // ガス放出
                    if(cpMotionController.isEndMotion((int)LastBoss.MotionLayer.Base))
                    {
                        cpMotionController.setMotion((int)LastBoss.MotionLayer.Base, (int)LastBoss.MotionBankID.Locomotion, (int)LastBoss.LocomotionMotionID.Neutral);
                        timer = 0;
                        phase = 1;
                    }
                    break;

                case 1:
                    if(wallMove(arg))
                    {
                        timer = 0;
                        phase = 2;
                    }
                    break;

                case 2:
                    if(contactWall(arg) || timer > recoveryInterval)
                    {
                        resetRotation();
                        timer = 0;
                        phase = 3;
                    }
                    rushMoveAndRotation();
                    break;

                case 3:
                    if (contactWall(arg) || timer > recoveryInterval)
                    {
                        resetRotation();
                        timer = 0;
                        phase = 4;
                    }
                    rushMoveAndRotation();
                    break;

                case 4:
                    if (contactWall(arg) || timer > recoveryInterval)
                    {
                        resetRotation();
                        timer = 0;
                        phase = 5;
                    }
                    rushMoveAndRotation();
                    break;

                case 5:
                    if(gap > timer)
                    {
                        timer = 0;
                        phase = 0;
                        gasBlastRushCd.Value = false;
                        cpLastBossSource.updateGravity = true;
                        arg.notifyNodeEnd();
                    }
                    break;

                case 6:
                    break;

                case 7:
                    break;
            }
        }

        public override void end(ActionArg arg)
        {
            cpLastBossSource.SideStepEnabled = false;
        }

        private void rushMoveAndRotation()
        {
            if(timer > rotationInterval)
            {
                cpRequestSetCollider.registerRequestSet(0, attackID);
                cpLastBossSource.updateGravity = false;
                upMove(ownerObj.Transform);
                rushMove();
                ownerObj.Transform.LocalEulerAngle = new vec3(angle, ownerAngleY, ownerObj.Transform.Rotation.z);
            }
            else
            {
                cpLastBossSource.updateGravity = true;
                cpCharacter.setRotationY(getTargetRotY(cpNavigationController.NavigationDirection));
                cpCharacter.updateRotation();
                movePosition = cpNavigationController.NavigationDirection;
                ownerAngleY = ownerObj.Transform.Rotation.y;
            }
        }

        private void rushMove()
        {
            cpCharacter.addMoveDirection(movePosition, rushSpeed);
            cpLastBossSource.blastRushGasObj.Target.DrawSelf = true;
        }

        private void upMove(Transform trans)
        {
            ownerObj.Transform.Position = new vec3(ownerObj.Transform.Position.x, rushHeight, ownerObj.Transform.Position.z);
        }

        private void resetRotation()
        {
            ownerObj.Transform.EulerAngle = new vec3(0, 0, 0);
            cpLastBossSource.blastRushGasObj.Target.DrawSelf = false;
        }

        private bool wallMove(ActionArg arg)
        {
            if(isHitWall)
            {
                vec3 vector = moveVector * 1 / moveTime * 1 / Application.BaseFps * cpCharacter.DeltaTime;
                ownerObj.Transform.Position += vector;
                phase = 2;
                return true;
            }
            else
            {
                wallCheck();
            }

            return false;
        }

        private void wallCheck()
        {
            CastRayQuery query = new CastRayQuery();
            FilterInfo filter = new()
            {
                Layer = via.physics.System.getLayerIndex("Character"),
            };
            filter.MaskBits = 0;

            // Rayの開始地点と終着地点の設定
            vec3 start = ownerObj.Transform.Position + new vec3(0, 1.5f, 0f);
            vec3 end = -ownerObj.Transform.AxisZ;

            // Ray照射
            query.copyFilterInfo(filter);
            query.setRay(start, end, 100);

            CastRayResult test = via.physics.System.castRay(query);

            if (test.NumContactPoints > 0)
            {
                timer = 0;
                isHitWall = true;
            }
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
            vec3 start = ownerObj.Transform.Position + new vec3(0, 0.5f, 0f);
            vec3 end = ownerObj.Transform.AxisY;

            // Ray照射
            query.copyFilterInfo(filter);
            query.setRay(start, end, rayLength);

            CastRayResult test = via.physics.System.castRay(query);

            if (test.NumContactPoints > 0)
            {
                debug.infoLine(test.getContactCollidable(0).GameObject.ToString());
                return true;
            }

            return false;
        }

        private float getTargetRotY(vec3 target_dir)
        {
            target_dir.y = 0.0f;
            target_dir = vector.normalizeFast(target_dir);

            return math.atan2(target_dir.x, target_dir.z);
        }
    }
}
