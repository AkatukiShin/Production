//=============================================================================
// <summary>
// GorillaEnemy 
// </summary>
// <author>CGC_12_小宮 孝介</author>
//=============================================================================
using blackfilter;
using via;
using via.attribute;
using via.physics;
using via.userdata;
using via.uvsequence;

namespace app
{
	public class GorillaEnemy : EnemyBase
	{
		[DataMember]
		private float IntimidationCoolTime = 20.0f;
        public enum MotionLayer
        {
            Base = 0,
        };

        public enum MotionBankID
        {
			Locomotion = 0,
            Battle = 10,
        };

        public enum MotionID
        {
			Walk = 1,
            Chase = 100,
			Intimidation = 110,
			ThreeStageAttack = 120,
			Guard = 130,
			Rush = 140,
			RushImpact = 150,
			FallDown = 160,
        };

        private VariableSingleHandle rushSpeed;
		private VariableBoolHandle isFirstContact;
		private VariableBoolHandle isGuard;
		private VariableBoolHandle isPlayerDetected;
		private VariableBoolHandle canIntimidation;

		private float timer = 0;
        private Player cpPlayer;
        private MedusaEyeAttackController cpMedusaEyeAttackController;
        
		private Transform playerTransform;

		public override void awake()
		{
			base.awake();
		}
		public override void start()
		{
			base.start();
            isPlayerDetected = new VariableBoolHandle(behaviorTreeComponent.findUserVariable(), str.makeHash("isPlayerDetected"));
			isGuard = new VariableBoolHandle(behaviorTreeComponent.findUserVariable(), str.makeHash("isGuard"));

            canIntimidation = new VariableBoolHandle(behaviorTreeComponent.ComponentUserVariables, via.str.makeHash("canIntimidation"));

			gasId = GasId.Monochrome;
		}

		public override void update()
		{
			base.update();
			cpSoundController.isPlayingSound(5);

            if (playerTransform == null && PlayerManager.Instance.getPlayer() != null)
            {
				cpPlayer = PlayerManager.Instance.getPlayer();
                playerTransform = cpPlayer.GameObject.Transform;
                cpMedusaEyeAttackController = playerTransform.find("MedusaEyeAttackController").GameObject.getSameComponent<MedusaEyeAttackController>();
            }

            cpRequestSetCollider.registerRequestSet(0, 0);
			cpRequestSetCollider.registerRequestSet(0, 2);
			cpRequestSetCollider.registerRequestSet(0, 5);

            if ((headSekikaValue + bodySekikaValue +
                LeftArmSekikaValue + rightArmSekikaValue +
                leftLegSekikaValue + rightLegSekikaValue) / 6 >= 0.85f)
            {
                if (isSekika.Value == false)
                {
                    isSekika.Value = true;
                    cpMotionController.freezeMotion((int)MotionLayer.Base);
                    partDict["Body"].sekikaAll();
                    updateGravity = true;
                    updatePosition = true;
                }
                return;
            }

            if(!canIntimidation.Value)
			{
                IntimidationCoolDown();
            }


			if (playerTransform is not null)
			{
                checkMedusaEye();
            }
		}

		private void IntimidationCoolDown()
		{
			if(!canIntimidation.Value)
			{
				timer += Application.ElapsedSecond;
				if(IntimidationCoolTime <= timer)
				{
					canIntimidation.Value = true;
					timer = 0;
				}
			}
		}

        private void checkMedusaEye()
        {
            if (!isPlayerDetected.Value)
            {
				guard(false);
                return;
            }

			var medusa_eye_info = cpPlayer.MedusaEyeInfo;

            if (medusa_eye_info != null && cpPlayer.MedusaEyeInfo.State == MedusaEyeState.Active)
			{
				vec3 plForward = vector.normalize(playerTransform.AxisZ);
				vec3 dirToEnemy = vector.normalize(GameObject.Transform.Position - playerTransform.Position);
				float angle = vector.angleBetween(plForward, dirToEnemy);
				if (math.abs(angle) < math.PI / 6)
				{
					guard(true);
					return;
				}
			}
			else
			{
				guard(false);
			}
        }

		private void guard(bool enabled)
		{
			isGuard.Value = enabled;
		}
    }
}
