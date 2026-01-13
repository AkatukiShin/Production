//=============================================================================
// <summary>
// BossBattleManager 
// </summary>
// <author>CGC_12_小宮 孝介</author>
//=============================================================================
using System;
using System.Collections.Generic;
using app;
using via;
using via.attribute;
using via.behaviortree;
using blackfilter;
using via.userdata;

namespace blackfilter
{
    [UpdateOrder((int)UpdateOrder.Enemy)]
    public class BossBattleManager : via.Behavior
	{
        [DataMember]
        private Prefab LastBossPrefab;
        [DataMember]
        private vec3 LastBossSpawnAngle;
        [DataMember, DisplayName("ClearGUIオブジェクト")]
        public GameObjectRef ClearGUIObj;
        [DataMember, DisplayName("スポーン位置")]
        public GameObjectRef SpawnPosObj;
        [DataMember]
        public string spawnFolderPath = "DynamicInstances/Enemy";

        public BossPhaseDefinition.BossPhase CurrentPhase { get; set; } = BossPhaseDefinition.BossPhase.Phase1;

        private bool isClearGUI = false;
        private bool isPhase2 = false;
        private LastBoss cpLastBossSource;
        private BehaviorTree cpBehaviorTree;
        private HumanEnemySummon cpHumanEnemySummon;
        private GameObject lastBossObj;
        private GUIGameClear cpGUIGameClear;

        private VariableBoolHandle humanSummon;
        private VariableBoolHandle bossEnhance;


        // 繊維条件
        private VariableBoolHandle isTwoPartSekika;
        private bool isAllEnemyDead = false;
		private bool isEndMeteor = false;
        private VariableBoolHandle isSekika;

        private bool isInitialized = false;
        private bool forceReset = false;

        private object bossLock = new();
        public override void awake()
        {
        }

        public override void start()
		{
            if (ClearGUIObj.Target != null)
            {
                cpGUIGameClear = ClearGUIObj.Target.getSameComponent<GUIGameClear>();
            }
            //initialize();
            PlayerManager.Instance.OnPlayerSpawned += onSpawnPlayer;
            PlayerManager.Instance.OnPlayerDespawned += onPlayerDespawn;
        }

		public override void update()
		{
            lock (bossLock)
            {
                if (!isInitialized)
                {
                    if(PlayerManager.Instance.getPlayer()  != null)
                    {
                        forceReset = true;
                        isInitialized = false;
                        initialize(true);
                    }
                    return;
                }

                if (forceReset)
                {
                    forceReset = false;
                    return;
                }

                if (lastBossObj == null)
                {
                    return;
                }
            }

			switch (CurrentPhase)
			{
				case BossPhaseDefinition.BossPhase.Phase1:
                    if (isTwoPartSekika.Value) PhaseChanger(BossPhaseDefinition.BossPhase.Phase2);
                    break;

				case BossPhaseDefinition.BossPhase.Phase2:
                    if (isAllEnemyDead) PhaseChanger(BossPhaseDefinition.BossPhase.Phase3);
                    break;

				case BossPhaseDefinition.BossPhase.Phase3:
                    if (isEndMeteor) PhaseChanger(BossPhaseDefinition.BossPhase.Phase4);
					break;

				case BossPhaseDefinition.BossPhase.Phase4:
                    if (isSekika.Value) PhaseChanger(BossPhaseDefinition.BossPhase.End);
					break;

				case BossPhaseDefinition.BossPhase.End:
                    if(!isClearGUI)
                    {
                        isClearGUI = true;
                        // 関数たたく
                    }
					break;

				default:
					debug.errorLine("未定義のPhaseです");
					break;
			}

            if (PlayerManager.isValid())
            {
                var player = PlayerManager.Instance?.getPlayer();

                if (player is not null)
                {
                    var isPlayerDead = player.IsDead;
                    if (isPlayerDead)
                    {
                        debug.infoLine("dead!!!!!!!!!");
                        cpHumanEnemySummon.HumanSummonInitialize();
                        //if(lastBossObj is not null)
                        //{
                        //    GameObject.destroy(lastBossObj);
                        //}
                        //initialize();
                    }
                }
            }
        }

		public void PhaseChanger(BossPhaseDefinition.BossPhase nextPhase)
		{
            lock (bossLock)
            {
                switch (nextPhase)
                {
                    case BossPhaseDefinition.BossPhase.Phase2:
                        debug.infoLine("Phase 2");
                        humanSummon.Value = true;
                        cpHumanEnemySummon.Summon(lastBossObj, cpLastBossSource);
                        lastBossObj = null;
                        isPhase2 = true;
                        CurrentPhase = BossPhaseDefinition.BossPhase.Phase2;
                        break;

                    case BossPhaseDefinition.BossPhase.Phase3:
                        debug.infoLine("Phase 3");
                        CurrentPhase = BossPhaseDefinition.BossPhase.Phase3;
                        break;

                    case BossPhaseDefinition.BossPhase.Phase4:
                        debug.infoLine("Phase 4");
                        initialize();
                        bossEnhance.Value = true;
                        CurrentPhase = BossPhaseDefinition.BossPhase.Phase4;
                        break;

                    case BossPhaseDefinition.BossPhase.End:
                        debug.infoLine("Ending");
                        cpGUIGameClear.GameClaer();
                        cpGUIGameClear.ActiveWindow();
                        break;

                    default:
                        debug.errorLine("未定義のPhaseです");
                        break;
                }
            }
		}

        private void initialize(bool reset = false)
        {
            spawnLastBoss();

            if(lastBossObj == null)
            {
                lastBossObj = SceneManager.MainScene.findGameObject("LastBoss");
            }

            cpLastBossSource = lastBossObj.getSameComponent<LastBoss>();

            if (lastBossObj is IBossAgent bossHandler)
            {
                cpBehaviorTree = bossHandler.getBehaviorTree();
            }
            else
            {
                cpBehaviorTree = cpLastBossSource.getBehaviorTree();
            }

            if (cpHumanEnemySummon is null)
            {
                cpHumanEnemySummon = GameObject.getSameComponent<HumanEnemySummon>();
            }

            isTwoPartSekika = new VariableBoolHandle(cpBehaviorTree.findUserVariable(), via.str.makeHash("isTwoPartSekika"));
            humanSummon = new VariableBoolHandle(cpBehaviorTree.findUserVariable(), str.makeHash("humanSummon"));
            bossEnhance = new VariableBoolHandle(cpBehaviorTree.findUserVariable(), str.makeHash("bossEnhance"));
            isSekika = new VariableBoolHandle(cpBehaviorTree.findUserVariable(), str.makeHash("isSekika"));
            CurrentPhase = BossPhaseDefinition.BossPhase.Phase1;
            lastBossObj.Transform.EulerAngle = LastBossSpawnAngle;

            if (reset)
            {
                isPhase2 = false;
                isInitialized = true;
                cpHumanEnemySummon.HumanSummonInitialize();
                return;
            }

            if (isPhase2)
            {
                isPhase2 = false;
                cpHumanEnemySummon.HumanSummonInitialize();
            }
        }

        private void spawnLastBoss()
        {
            lastBossObj = LastBossPrefab.instantiate(SpawnPosObj.Target.Transform.Position, SceneManager.CurrentScene.findFolder(spawnFolderPath));
        }

        private void onSpawnPlayer(Player player)
        {
            lock (bossLock)
            {
                forceReset = true;
                isInitialized = false;
                initialize(true);
            }
        }

        private void onPlayerDespawn(Player player)
        {
            lock (bossLock)
            {
                if (lastBossObj != null)
                {
                    GameObject.destroy(lastBossObj);
                }
            }
        }

        public override void onDestroy()
        {
            base.onDestroy();
            PlayerManager.Instance.OnPlayerSpawned -= onSpawnPlayer;
            PlayerManager.Instance.OnPlayerDespawned -= onPlayerDespawn;
        }
    }
}
