//=============================================================================
// <summary>
// EnemyBase 
// </summary>
// <author>CGC_12_小宮 孝介</author>
//=============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using blackfilter;
using via;
using via.attribute;
using via.behaviortree;
using via.physics;
using via.userdata;

namespace app
{
    public class EnemyBase : via.Behavior, IExtractEventHandler
	{
		public int wanderingLayer = 0;
		public uint wanderingBankID = 0;
		public uint wanderingMotionID = 0;

		public int chaseLayer = 0;
		public uint chaseBankID = 0;
		public uint chaseMotionID = 0;

		public int StoningSoundID = -1;
		public int StonedSoundID = -1;

		public bool isFadeOutStop = true;

        [DataMember]
		public EnemyUserDataBase enemyUserData;

		public float headSekikaValue = 0;
		public float bodySekikaValue = 0;
		public float rightArmSekikaValue = 0;
		public float leftArmSekikaValue = 0;
		public float rightLegSekikaValue = 0;
		public float leftLegSekikaValue = 0;

		protected BehaviorTree behaviorTreeComponent;
        protected VariableBoolHandle isSekika;
		protected VariableSingleHandle wanderingSpeed;
		protected VariableSingleHandle chaseSpeed;
		protected VariableSingleHandle rotationSpeed;
        protected DamageController cpDamageController;
		protected EnemyMotionController cpMotionController;
		protected PressController cpPressController;
		protected SoundController cpSoundController;
		protected NavigationController cpNavigationController;

        protected RequestSetCollider cpRequestSetCollider;


        private float wanderingSpeedDefaultValue;
		private float chaseSpeedDefaultValue;

		private Character cpCharacter;

		public bool updatePosition = true;
		public bool updateGravity = true;

		private ConcurrentQueue<(string, EnemyPart)> partRegQueue = new();
		private ConcurrentQueue<(string, GameObject)> objNameRegQueue = new();
        protected Dictionary<string, EnemyPart> partDict = new Dictionary<string, EnemyPart>();
		protected Dictionary<string, GameObject> objDict = new Dictionary<string, GameObject>();

		[DataMember]
		private uint partCount = 6;

		private bool partRegisterDone = false;

		private ConcurrentQueue<string> hitManagerReadyQueue = new();
		private List<string> hitManagerReadyList = new();

		private bool isExtracted = false;
		protected GasId gasId = GasId.Undefined;

		protected bool isExtracting = false;

		[DataMember]
		public bool stopWayPoint = false;

        public override void awake()
		{
			behaviorTreeComponent = GameObject.getSameComponent<BehaviorTree>();
			cpNavigationController = GameObject.getSameComponent<NavigationController>();
			isSekika = new VariableBoolHandle(behaviorTreeComponent.findUserVariable(), via.str.makeHash("isSekika"));
			wanderingSpeed = new VariableSingleHandle(behaviorTreeComponent.findUserVariable(), via.str.makeHash("wanderingSpeed"));
			wanderingSpeedDefaultValue = enemyUserData.WanderingSpeed;
			if (stopWayPoint)
			{
				wanderingSpeedDefaultValue = 0;
				cpNavigationController.stopWayPoint = true;
			}
			wanderingSpeed.Value = wanderingSpeedDefaultValue;
			chaseSpeed = new VariableSingleHandle(behaviorTreeComponent.findUserVariable(), via.str.makeHash("chaseSpeed"));
			chaseSpeedDefaultValue = enemyUserData.ChaseSpeed;
			chaseSpeed.Value = chaseSpeedDefaultValue;
			rotationSpeed = new VariableSingleHandle(behaviorTreeComponent.findUserVariable(), str.makeHash("rotationSpeed"));
			rotationSpeed.Value = enemyUserData.RotationSpeed;
			cpCharacter = GameObject.getSameComponent<Character>();
            cpDamageController = GameObject.getSameComponent<DamageController>();
            cpMotionController = GameObject.getSameComponent<EnemyMotionController>();
			cpPressController = GameObject.getSameComponent<PressController>();
			cpSoundController = GameObject.getSameComponent<SoundController>();
            cpRequestSetCollider = GameObject.getSameComponent<RequestSetCollider>();
        }

		public override void start()
		{
            foreach (var soundAsset in cpSoundController._Sources)
            {
                soundAsset.Volume = SoundManager.Instance.SeVolumeRate;
            }
        }

        public override void update()
		{
			//if (HeadSekikaValue >= 0.9)
			//{
			//	debug.infoLine("Sekika! EnemyStop");
			//	isSekika.Value = true;
			//}

			while (!partRegQueue.IsEmpty)
			{
				(string, EnemyPart) res;
				if (partRegQueue.TryDequeue(out res))
				{
					partDict.Add(res.Item1, res.Item2);
				}
			}

			while (!objNameRegQueue.IsEmpty)
			{
                (string, GameObject) res;
				if (objNameRegQueue.TryDequeue(out res))
				{
					objDict.Add(res.Item1, res.Item2);
				}
            }

			while (!hitManagerReadyQueue.IsEmpty)
			{
				string readyPart;
				if (hitManagerReadyQueue.TryDequeue(out readyPart))
				{
					hitManagerReadyList.Add(readyPart);
				}
			}

			if (partDict.Count == partCount && objDict.Count == partCount)
			{
				partRegisterDone = true;
			}
			else
			{
				return;
			}

			if (!isExtracted)
			{
				cpRequestSetCollider.registerRequestSet(0, 48);
			}

			if (isSekika.Value == false)
			{
				wanderingSpeed.Value = wanderingSpeedDefaultValue * (1 - (rightLegSekikaValue / 2 + leftLegSekikaValue / 2));
				chaseSpeed.Value = chaseSpeedDefaultValue * (1 - (rightLegSekikaValue / 2 + leftLegSekikaValue / 2));
			}
			else
			{
				cpSoundController.stopAllSound(isFadeOutStop);
				wanderingSpeed.Value = 0;
				chaseSpeed.Value = 0;
			}

			if (updatePosition)
			{
				cpCharacter.updatePosition();
			}
			if (updateGravity)
			{
				cpCharacter.updateGravity();
			}

            var damageInfoList = cpDamageController.getDamageInfoListInThisFrame();
            var count = damageInfoList.Count;

			for (var i = 0; i < count; i++)
			{
				var damageInfo = damageInfoList[i];
				partDict[damageInfo.PartName].receiveDamage(damageInfo);
				if(cpSoundController is not null && !cpSoundController.isPlayingSound(StoningSoundID))
				{
                    cpSoundController.play(StoningSoundID);
                }
			}

			if (cpSoundController is not null && count == 0 && cpSoundController.isPlayingSound(StoningSoundID))
			{
				cpSoundController.stop(StoningSoundID);
			}

			updatePress();
 
        }

		public virtual void motionMethod(int id)
		{

		}

		public void setPart(string key, EnemyPart part)
		{
			partRegQueue.Enqueue((key, part));
		}

		public void registerObjectName(string name, GameObject obj)
		{
			objNameRegQueue.Enqueue((name, obj));
		}

		public void setManagerReady(string name)
		{
			hitManagerReadyQueue.Enqueue(name);
		}

		public GameObject getChildObject(string name)
		{
			if (objDict.ContainsKey(name))
			{
				return objDict[name];
			}
			else
			{
				return null;
			}
		}

		public GameObject[] getPartObjects()
		{
			return objDict.Values.ToArray();
		}

		public virtual void onExtractStarted(ExtractParam param)
		{
			isExtracting = true;
        }

        public virtual void onExtractPerformed(ExtractParam param)
		{
			if (!isExtracting)
			{
				isExtracting = true;
			}
		}

		public virtual void onExtractCompleted(ExtractParam param)
		{
            isExtracted = true;
			isExtracting = false;
        }

        #region プロパティ
        public float HeadSekikaValue
		{
			get { return headSekikaValue; }
			set { headSekikaValue = value; }
		}

        public float BodySekikaValue
        {
            get { return bodySekikaValue; }
            set { bodySekikaValue = value; }
        }

		public float RightArmSekikaValue
		{
			get { return rightArmSekikaValue; }
			set { rightArmSekikaValue = value; }
		}

		public float LeftArmSekikaValue
		{
			get { return leftArmSekikaValue; }
			set { leftArmSekikaValue = value; }
		}

		public float RightLegSekikaValue
		{
			get { return rightArmSekikaValue; }
			set { rightLegSekikaValue = value; }
		}

		public float LeftLegSekikaValue
		{
			get { return leftLegSekikaValue; }
			set { leftLegSekikaValue = value; }
		}

		public bool IsSekika
		{
			get
			{
				return isSekika.Value;
			}
		}

        public bool PartRegisterDone
		{
			get
			{
				return partRegisterDone;
			}
		}

		public bool hitManagerReadyDone
		{
			get
			{
				if (hitManagerReadyList == null)
				{
					return false;
				}
				if (hitManagerReadyList.Count == partCount)
				{
					return true;
				}
				return false;
			}
		}

		public EnemyMotionController MotionController
		{
			get
			{
				return cpMotionController;
			}
		}

		public SoundController SoundController
		{
			get
			{
				return cpSoundController;
			}
		}

		public RequestSetCollider RequestSetCollider
		{
			get
			{
				return cpRequestSetCollider;
			}
		}


        #endregion

        #region 当たり判定
        //-------------------------------------------------------------------------
        // 押し当たり
        //-------------------------------------------------------------------------
        private void updatePress()
        {
            if (cpPressController == null)
            {
                return;
            }

            var press = cpPressController.getTotalPressMove();
            if (press != null)
            {
                var pos = GameObject.Transform.Position;
                pos += press.moveDistance;
                GameObject.Transform.Position = pos;
            }
        }
        #endregion
    }
}
