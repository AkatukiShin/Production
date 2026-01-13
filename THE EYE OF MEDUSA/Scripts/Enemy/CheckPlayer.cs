//=============================================================================
// <summary>
// CheckPlayer 
// </summary>
// <author>CGC_12_小宮 孝介</author>
//=============================================================================
using blackfilter;
using via;
using via.attribute;
using via.behaviortree;
using via.effect.script;
using via.physics;
using via.userdata;

namespace app
{
	public class CheckPlayer : via.Behavior
	{
        [DataMember]
        private bool isIgnoreRange = false;

        //[DataMember]
        //private float searchRange = 8;
        private EnemyUserDataBase enemyUserData;

        private GameObject owner;
        private Transform ownerTransform;
        private BehaviorTree behaviorTreeComponent;

        //private Player target;
        private GameObject target;

        private VariableBoolHandle isPlayerDetected;
        private VariableBoolHandle isNear;
        private VariableBoolHandle isMiddle;
        private VariableBoolHandle isFar;
        private VariableBoolHandle isLongFar;
        private vec3 playerPos;
        private vec3 enemyPos;
        private vec3 locationVec;

        private NavigationController navigationController;
        private via.navigation.NavigationSurface navigationSurface;

        public override void awake()
		{
            owner = GameObject;
            ownerTransform = owner.Transform;

            enemyUserData = GameObject.getComponent<EnemyBase>().enemyUserData;

            behaviorTreeComponent = GameObject.getSameComponent<BehaviorTree>();

            isPlayerDetected = new via.userdata.VariableBoolHandle(behaviorTreeComponent.findUserVariable(), via.str.makeHash("isPlayerDetected"));
            isNear = new via.userdata.VariableBoolHandle(behaviorTreeComponent.findUserVariable(), via.str.makeHash("isNear"));
            isMiddle = new via.userdata.VariableBoolHandle(behaviorTreeComponent.findUserVariable(), via.str.makeHash("isMiddle"));
            isFar = new via.userdata.VariableBoolHandle(behaviorTreeComponent.findUserVariable(), via.str.makeHash("isFar"));
            isLongFar = new via.userdata.VariableBoolHandle(behaviorTreeComponent.findUserVariable(), via.str.makeHash("isLongFar"));

            navigationController = GameObject.getSameComponent<NavigationController>();
            navigationSurface = GameObject.getSameComponent<via.navigation.NavigationSurface>();
        }

		public override void start()
        {
            if (PlayerManager.isValid())
            {
                var player = PlayerManager.Instance.getPlayer();

                if (PlayerManager.Instance.getPlayer() != null)
                {
                    //debug.infoLine("target NULL");
                    target = player.GameObject;
                }
            }
        }

		public override void update()
		{
            if(target == null)
            {
                if (PlayerManager.Instance.getPlayer() == null) return;
                target = PlayerManager.Instance.getPlayer().GameObject;
            }
            else
            {
                playerPos = target.Transform.Position + new vec3(0, 1, 0);
            }

            getTargetDistance();

            switch(isPlayerDetected.Value)
            {
                case true:
                    checkPlayerDirection();
                    break;

                case false:
                    checkPlayerDuration();
                    break;
            }
		}

        private void checkPlayerDuration()
        {
            if (isIgnoreRange && isHitTerrain())
            {
                isPlayerDetected.Value = true;
                navigationSurface.TargetGameObject = target;
                var isNavigationVariable = new via.userdata.VariableBoolHandle(behaviorTreeComponent.findUserVariable(), via.str.makeHash("isNavigation"));
                isNavigationVariable.Value = false;
                navigationController.startNavigation(target);
                return;
            }
            if (getTargetDistance() <= enemyUserData.FarRange)
            {
                switch (isHitTerrain())
                {
                    case true:
                        isPlayerDetected.Value = true;
                        navigationSurface.TargetGameObject = target;
                        var isNavigationVariable = new via.userdata.VariableBoolHandle(behaviorTreeComponent.findUserVariable(), via.str.makeHash("isNavigation"));
                        isNavigationVariable.Value = false;
                        navigationController.startNavigation(target);
                        break;
                    case false:
                        break;
                }
            }
            else isPlayerDetected.Value = false;
        }

        private bool isHitTerrain()
        {
            CastRayQuery query = new CastRayQuery();
            FilterInfo filter = new()
            {
                Layer = via.physics.System.getLayerIndex("Character")
            };
            filter.MaskBits = 0;
            //filter.MaskBits |= (1U << (int)via.physics.System.getMaskIndexInLayer(filter.Layer, "TbDefault"));
            query.copyFilterInfo(filter);
            query.disableAllHits();
            vec3 start = enemyPos;
            vec3 end = playerPos;
            query.setRay(enemyPos, playerPos);
            CastRayResult result = via.physics.System.castRay(query);
            if (result.Finished)
            {
                if (result.NumContactPoints == 0)
                {
                    isPlayerDetected.Value = true;
                    return true;
                }
            }

            return false;
        }

        private void checkPlayerDirection()
        {
            if (getTargetDistance() < enemyUserData.NearRange)
            {
                isNear.Value = true;
                isMiddle.Value = false;
                isFar.Value = false;
                isLongFar.Value = false;
                //debug.infoLine("Near!");
            }
            else if (getTargetDistance() < enemyUserData.MiddleRange)
            {
                isNear.Value = false;
                isMiddle.Value = true;
                isFar.Value = false;
                isLongFar.Value = false;
                //debug.infoLine("Middle");
            }
            else if (getTargetDistance() < enemyUserData.FarRange)
            {
                isNear.Value = false;
                isMiddle.Value = false;
                isFar.Value = true;
                isLongFar.Value = false;
                //debug.infoLine("Far!");
            }
            if (getTargetDistance() >= enemyUserData.FarRange)
            {
                if (isIgnoreRange)
                {
                    isLongFar.Value = true;
                    isNear.Value = false;
                    isMiddle.Value = false;
                    isFar.Value = false;
                    return;
                }
                isPlayerDetected.Value = false;
                isNear.Value = false;
                isMiddle.Value = false;
                isFar.Value = false;
            }
        }

        private float getTargetDistance()
        {
            enemyPos = this.GameObject.Transform.Position + new vec3(0, 1f, 0);
            locationVec = playerPos - enemyPos;

            return vector.length(locationVec);
        }
    }
}
