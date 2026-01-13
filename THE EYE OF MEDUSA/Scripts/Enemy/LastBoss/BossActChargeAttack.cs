//=============================================================================
// <summary>
// BossActChargeAttack 
// </summary>
// <author>CGC_12_小宮 孝介</author>
//=============================================================================
using System;
using System.Collections.Generic;
using via;
using via.attribute;
using via.behaviortree;
using via.userdata;

namespace blackfilter
{
    public class BossActChargeAttack : via.behaviortree.Action
    {
        [DataMember, DisplayName("攻撃開始(持ち上げ)までの時間")]
        public float attackStartLength = 1.0f;
        [DataMember, DisplayName("攻撃終了までの時間")]
        public float attackEndLength = 2.0f;
        [DataMember, DisplayName("瞬間移動までの時間")]
        public float teleportTime = 0.1f;
        [DataMember, DisplayName("瞬間移動から攻撃までの溜め時間")]
        public float chargeTime = 2.0f;
        [DataMember, DisplayName("中パンチ後隙")]
        public float panchRecoverDuration = 2.0f;
        [DataMember, DisplayName("瞬間移動後のプレイヤーとの距離")]
        public float teleportDistance = 1.0f;

        private float timer = 0;
        private int phase = 0;
        private Transform playerTransform;
        private vec3 targetPos;
        private vec3 moveVector;
        private VariableBoolHandle isArmSekika;
        private VariableBoolHandle isActionEnd;
        private VariableBoolHandle chargeAttackHandle;
        private VariableBoolHandle stepBackHandle;
        private HumanEnemy cpHumanEnemy;
        private Character cpCharacter;

        private Player cpPlayer;

        private bool isHit = false;
        public override void start(ActionArg arg)
        {
            if (cpHumanEnemy == null)
            {
                cpHumanEnemy = arg.OwnerGameObject.getComponent<HumanEnemy>();
            }
            if (cpCharacter == null)
            {
                cpCharacter = arg.OwnerGameObject.getSameComponent<Character>();
            }
            if (cpPlayer == null)
            {
                cpPlayer = PlayerManager.Instance.getPlayer();
            }
            chargeAttackHandle = new VariableBoolHandle(arg.UserVariables, str.makeHash("chargeAttack"));
            chargeAttackHandle.Value = true;

            isArmSekika = new VariableBoolHandle(arg.UserVariables, str.makeHash("isArmSekika"));

            stepBackHandle = new VariableBoolHandle(arg.UserVariables, str.makeHash("stepBack"));

            isActionEnd = new VariableBoolHandle(arg.UserVariables, str.makeHash("isActionEnd"));

            debug.warningLine("charge attack");
        }

        public override void end(ActionArg arg)
        {
            debug.infoLine("chase attack end");
            isActionEnd.Value = true;
        }

        public override void update(ActionArg arg)
        {
            if (isArmSekika.Value)
            {
                phase = 0;
                timer = 0;
                chargeAttackHandle.Value = false;
                return;
            }
            timer += Application.ElapsedSecond;
            switch (phase)
            {
                case 0:
                    if (timer > attackStartLength)
                    {
                        phase = 1;
                        timer = 0;
                        cpHumanEnemy.initPlayerSensor();
                        isHit = false;
                        cpHumanEnemy.SideStepEnabled = false;
                        cpHumanEnemy.SoundController.play(11);
                        cpHumanEnemy.MotionController.setMotion((int)HumanEnemy.MotionLayer.Base, (int)HumanEnemy.MotionBankID.Battle, (int)HumanEnemy.BattleMotionID.ChargeAttack);
                    }
                    //cpCharacter.updateRotation();
                    break;
                case 1:
                    if (cpHumanEnemy.getRequestMethod() == 1)
                    {
                        if (cpHumanEnemy.getPlayerSensorValue())
                        {
                            cpHumanEnemy.SoundController.play(15);
                            isHit = true;
                            cpPlayer.Enabled = false;
                        }
                        else
                        {
                            cpHumanEnemy.SoundController.play(14);
                            isHit = false;
                        }

                        phase = 2;
                        timer = 0;
                    }
                    break;
                case 2:
                    if (isHit && cpHumanEnemy.getRequestMethod() == 2)
                    {
                        cpHumanEnemy.RequestSetCollider.registerRequestSet(0, 5);
                        cpPlayer.Enabled = true;
                    }
                    // 攻撃終了
                    if (timer > attackEndLength && cpHumanEnemy.MotionController.isEndMotion((int)HumanEnemy.MotionLayer.Base))
                    {
                        cpHumanEnemy.MotionController.setMotion((int)HumanEnemy.MotionLayer.Base, (int)HumanEnemy.MotionBankID.Locomotion, (int)HumanEnemy.LocomotionMotionID.Neutral);
                        phase = 3;
                        timer = 0;
                        cpHumanEnemy.SideStepEnabled = true;
                    }
                    break;
                case 3:
                    // テレポートまで待機
                    if (timer > teleportTime)
                    {
                        phase = 4;
                        timer = 0;
                        chargeAttackHandle.Value = false;
                        stepBackHandle.Value = true;
                        cpHumanEnemy.SideStepEnabled = false;
                        playerTransform = PlayerManager.Instance.getPlayer().GameObject.Transform;
                        vec3 playerForward = playerTransform.AxisZ;
                        vec3 behindOffset = -vector.normalize(playerForward);
                        vec3 targetPos = playerTransform.Position + behindOffset * teleportDistance;
                        moveVector = targetPos - arg.OwnerGameObject.Transform.Position;
                    }
                    break;
                case 4:
                    // テレポート処理
                    if(timer > teleportTime)
                    {
                        cpHumanEnemy.MotionController.setMotion((int)HumanEnemy.MotionLayer.Base, (int)HumanEnemy.MotionBankID.Locomotion, (int)HumanEnemy.LocomotionMotionID.Neutral);
                        cpHumanEnemy.SideStepEnabled = true;
                        phase = 5;
                        timer++;
                    }
                    else
                    {
                        teleportMove(arg.OwnerGameObject.Transform);
                        cpCharacter.setRotationY(playerTransform.Rotation.y);
                        cpCharacter.updateRotation();
                    }
                     break;
                case 5:
                    // パンチ処理
                    cpHumanEnemy.MotionController.setMotion((int)HumanEnemy.MotionLayer.Base, (int)HumanEnemy.MotionBankID.Battle, (int)HumanEnemy.BattleMotionID.Attack);
                    if(cpHumanEnemy.MotionController.isEndMotion((int)HumanEnemy.MotionLayer.Base))
                    {
                        phase = 6;
                        timer = 0;
                    }
                    break;
                case 6:
                    // 後隙
                    if(timer > panchRecoverDuration)
                    {
                        phase = 0;
                        timer = 0;
                        chargeAttackHandle.Value = false;
                        stepBackHandle.Value = true;
                        arg.notifyNodeEnd();
                    }
                    break;
            }
        }

        private void teleportMove(Transform trans)
        {
            vec3 a = moveVector * 1 / teleportTime * 0.1f / Application.BaseFps * cpCharacter.DeltaTime;
            trans.Position += a;
        }
    }
}

