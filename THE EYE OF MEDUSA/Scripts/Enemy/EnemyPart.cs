//=============================================================================
// <summary>
// EnemyPart 
// </summary>
// <author>CGC_12_小宮 孝介</author>
//=============================================================================
using System.Collections.Generic;
using blackfilter;
using via;
using via.attribute;
using via.render;

namespace app
{
	public class EnemyPart : Behavior
	{
        [DataMember]
        private EnemyParts enemyPart;

        public SekikaHitPositionManager sekikaHitPositionManager;

		private EnemyUserDataBase enemyUserData;
		// 石化が広がる速度
		private float increaseSizePerSec = 0;
		// 石化してから解除が始まるまでの時間
		private float dissolveStartTime = 4;
		// 解除が終わる時間
		private float dissolveEndTime = 8;
        // 石化進行度
        private float sekikaProgressValue = 0;

        private Mesh mesh;

		private EnemyBase enemyBase;

        [DataMember]
        private string baseJointName;

        [DataMember]
        private vec3 aabbCenter;
        [DataMember]
        private vec3 aabbHalfExtents;

        public struct jointData
        {
            public GameObject targetObj;
            public string jointName;
        }

        [DataMember]
        private List<string> targetObjNames;
        [DataMember]
        private List<string> jointNames;

        private List<jointData> jointDatas = new();

        [DataMember]
        private List<int> bufferStartIndexParams = new List<int> { 0 };

        [DataMember]
        private List<int> bufferDataCountParams = new List<int> { 1 };

        [DataMember]
        private List<int> increasingIdxParams = new List<int> { 2 };

        public enum EnemyParts
        {
            None,
            Head,
            Body,
            RightArm,
            LeftArm,
            RightLeg,
            LeftLeg,
        }

        public override void awake()
        {
            // コンポーネント取得
			//damageController = GameObject.getSameComponent<DamageController>();
            enemyBase = GameObject.Transform.Parent.GameObject.getComponent<EnemyBase>();
            mesh = GameObject.getSameComponent<Mesh>();

            if (enemyBase == null)   // 胴体以外の部位の処理
            {
				enemyBase = GameObject.Transform.Parent.Parent.GameObject.getComponent<EnemyBase>();
            }

            enemyBase.setPart(enemyPart.ToString(), this);
            enemyBase.registerObjectName(GameObject.Name, GameObject);
            debug.infoLine(enemyPart.ToString());
        }

        public override void start()
        {
            base.start();
        }

        public override void update()
		{
            base.update();

            if (!isSekikaInitialized && SekikaHitPositionWriter.Instance.IsReady() && enemyBase.PartRegisterDone)
            {
                initilalizeJointData();
                initializeSekika();
            }

            if (!isConnectedPartInitialized && isSekikaInitialized && enemyBase.hitManagerReadyDone)
            {
                initializeConnectedPart();
            }

            if (sekikaHitPositionManager is not null)
            {
                sekikaHitPositionManager.updateState(DeltaTime);

                sekikaProgressValue = sekikaHitPositionManager.getSekikaProgress();
                switch (enemyPart)
                {
                    case EnemyParts.Head:
                        enemyBase.HeadSekikaValue = sekikaProgressValue;
                        //debug.infoLine($"HeadsekikaValue : {sekikaProgressValue}");
                        break;
                    case EnemyParts.Body:
                        enemyBase.BodySekikaValue = sekikaProgressValue;
                        //debug.infoLine($"BodysekikaValue : {sekikaProgressValue}");
                        break;
                    case EnemyParts.RightArm:
                        enemyBase.RightArmSekikaValue = sekikaProgressValue;
                        //debug.infoLine($"RightArmsekikaValue : {sekikaProgressValue}");
                        break;
                    case EnemyParts.LeftArm:
                        enemyBase.LeftArmSekikaValue = sekikaProgressValue;
                        //debug.infoLine($"LeftArmsekikaValue : {sekikaProgressValue}");
                        break;
                    case EnemyParts.RightLeg:
                        enemyBase.rightLegSekikaValue = sekikaProgressValue;
                        //debug.infoLine($"RightLegsekikaValue : {sekikaProgressValue}");
                        break;
                    case EnemyParts.LeftLeg:
                        enemyBase.leftLegSekikaValue = sekikaProgressValue;
                        //debug.infoLine($"LeftLegsekikaValue : {sekikaProgressValue}");
                        break;
                    default:
                        debug.errorLine("enemyPartを設定してください。");
                        break;
                }
            }
        }

        private void initilalizeJointData()
        {
            for (int i = 0; i < targetObjNames.Count; i++)
            {
                GameObject obj = enemyBase.getChildObject(targetObjNames[i]);
                if (obj != null)
                {
                    jointData data = new jointData();
                    data.targetObj = obj;
                    data.jointName = jointNames[i];
                    jointDatas.Add(data);
                }
            }
        }

        public void receiveDamage(DamageInfo damageInfo)
        {
            if (!isSekikaInitialized)
            {
                return;
            }

            sekikaHitPositionManager.registerPosition(damageInfo, DeltaTime);
        }

        public void sekikaAll()
        {
            sekikaHitPositionManager.registerPosition(GameObject.Transform.Position, GameObject.Transform.Joints[0].Name, 1000000);
        }

        private bool isSekikaInitialized = false;
        private bool isConnectedPartInitialized = false;

        private void initializeSekika()
        {
            if (isSekikaInitialized)
            {
                return;
            }

            isSekikaInitialized = true;

            // エネミーのUserDataの取得・変数の設定
            enemyUserData = enemyBase.enemyUserData;

            increaseSizePerSec = enemyUserData.IncreaseSizePerSec;
            dissolveStartTime = enemyUserData.DissolveStartTime;
            dissolveEndTime = enemyUserData.DissolveEndTime;

            MaterialParam[] bufferStartIdx = new MaterialParam[bufferStartIndexParams.Count];
            for (int i = 0; i < bufferStartIndexParams.Count; i++)
            {
                bufferStartIdx[i] = mesh.Materials[bufferStartIndexParams[i]];
            }

            MaterialParam[] dataCountParams = new MaterialParam[bufferDataCountParams.Count];
            for (int i = 0; i < bufferDataCountParams.Count; i++)
            {
                dataCountParams[i] = mesh.Materials[bufferDataCountParams[i]];
            }

            MaterialParam[] increasingParams = new MaterialParam[increasingIdxParams.Count];
            for (int i = 0; i < increasingIdxParams.Count; i++)
            {
                increasingParams[i] = mesh.Materials[increasingIdxParams[i]];
            }

            int connectedPartCount = jointDatas.Count;

            sekikaHitPositionManager = new SekikaHitPositionManager(enemyUserData, GameObject.Transform, aabbCenter, aabbHalfExtents, bufferStartIdx, dataCountParams, increasingParams, increaseSizePerSec: increaseSizePerSec, nearThreshold: 0.5f, connectedPartCount: connectedPartCount);

            enemyBase.setManagerReady(enemyPart.ToString());
        }

        private void initializeConnectedPart()
        {
            if (isConnectedPartInitialized) return;
            isConnectedPartInitialized = true;

            if (jointDatas.Count is 0 || jointDatas[0].targetObj is null)
            {
                debug.infoLine("sekikahitmanger NULL");
            }

            for (int i = 0; i < jointDatas.Count; i++)
            {
                if (jointDatas[i].targetObj == null) continue;
                vec3 jointPos = EnemyJointMapManager.Instance.getJointBaseWorldData(enemyUserData.EnemyName, jointDatas[i].jointName).Position;

                sekikaHitPositionManager.addConnectedPart($"{GameObject.Name} To {jointDatas[i].targetObj.Name}", jointDatas[i].targetObj.getSameComponent<EnemyPart>().sekikaHitPositionManager, jointPos);
            }
        }

        public EnemyParts getPart() { return enemyPart; }

        public string getBaseJointName() { return baseJointName; }
    }
}
