//=============================================================================
// <summary>
// FallDownAttack 
// </summary>
// <author>CGC_12_小宮 孝介</author>
//=============================================================================
using app;
using via;
using via.attribute;
using via.behaviortree;
using via.physics;
using static blackfilter.EnemyGasGeneratorManager;

namespace blackfilter
{
    public class GorillaFallDownAttack : Action
    {
        [DataMember, DisplayName("倒れるまでの時間")]
        private float fallDownTime = 2.0f;
        [DataMember, DisplayName("倒れている時間")]
        private float downTime = 2.0f;
        [DataMember, DisplayName("ガスが引くまでの時間")]
        private float gasRemoveTime = 1.0f;

        private bool isNavigation;
        private Character cpCharacter;
        private EnemyMotionController motionController;
        private float timer = 0;
        private GameObject ownerObj;
        private GameObject targetObj;
        private int phase = 0;
        private NavigationController navigationController;
        private RequestSetCollider cpRequestSetCollider;
        public override void start(ActionArg arg)
        {
            ownerObj = arg.OwnerGameObject;
            targetObj = PlayerManager.Instance.getPlayer().GameObject;

            //// LocalUserVariablesからwanderingSpeedを取得
            var isFallDown = new via.userdata.VariableBoolHandle(arg.UserVariables, via.str.makeHash("isAttack"));
            if (!isFallDown.Value) isFallDown.Value = true;

            motionController = ownerObj.getSameComponent<EnemyMotionController>();
            cpCharacter = ownerObj.getSameComponent<Character>();
            cpRequestSetCollider = ownerObj.getSameComponent<RequestSetCollider>();
            navigationController = ownerObj.getSameComponent<NavigationController>();

            motionController.setMotion((int)GorillaEnemy.MotionLayer.Base, (int)GorillaEnemy.MotionBankID.Locomotion, (int)GorillaEnemy.MotionID.Walk);
            debug.infoLine("FallDownStart");
        }

        public override void end(ActionArg arg)
        {
            debug.infoLine("FallDownEnd");
        }

        public override void update(ActionArg arg)
        {
            timer += Application.ElapsedSecond;
            phaseManager(arg);
            cpCharacter.setRotationY(getTargetRotY(navigationController.NavigationDirection));

            //debug.infoLine($"phase : {phase}");
        }

        private void phaseManager(ActionArg arg)
        {
            switch (phase)
            {
                case 0:
                    cpCharacter.updateRotation();
                    if (targetObj is not null && getTargetDistance() > 1) cpCharacter.addMoveDirection(ownerObj.Transform.AxisZ);
                    if (timer > fallDownTime && motionController.isEndMotion((int)GorillaEnemy.MotionLayer.Base))
                    {
                        timer = 0;
                        phase = 1;
                        motionController.setMotion((int)GorillaEnemy.MotionLayer.Base, (int)GorillaEnemy.MotionBankID.Battle, (int)GorillaEnemy.MotionID.FallDown);
                    }
                    break;
                case 1:
                    if (timer > downTime)
                    {
                        timer = 0;
                        phase = 2;
                    }
                    break;
                case 2:
                    if (timer > gasRemoveTime)
                    {
                        timer = 0;
                        phase = 0;
                        var isFallDown = new via.userdata.VariableBoolHandle(arg.UserVariables, via.str.makeHash("isAttack"));
                        isFallDown.Value = false;
                        arg.notifyNodeEnd();
                    }
                    break;
                default:
                    break;
            }

        }

        private float getTargetRotY(vec3 target_dir)
        {
            target_dir.y = 0.0f;
            target_dir = vector.normalizeFast(target_dir);

            return math.atan2(target_dir.x, target_dir.z);
        }

        private float getTargetDistance()
        {
            vec3 targetPos = targetObj.Transform.Position;
            vec3 enemyPos = ownerObj.Transform.Position;
            float targetDistance = vector.length(targetPos - enemyPos);
            return targetDistance;
        }
    }

}