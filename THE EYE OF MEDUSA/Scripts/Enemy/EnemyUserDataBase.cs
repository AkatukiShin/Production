//=============================================================================
// <summary>
// EnemyUserDataBase 
// </summary>
// <author>CGC_12_小宮 孝介</author>
//=============================================================================
using System.Collections.Generic;
using via;
using via.attribute;

namespace app
{
	public class EnemyUserDataBase : via.UserData
	{
        #region フィールド
        [DataMember]
        private float farRange = 9f;

        [DataMember]
        private float middleRange = 7f;

        [DataMember]
        private float nearRange = 2f;

        [DataMember]
        private float wanderingSpeed = 0.04f;

        [DataMember]
        private float chaseSpeed = 0.06f;

        [DataMember]
        private float rotationSpeed = 0.1f;

        [DataMember]
        private float attackPower = 0.6f;

        [DataMember]
        private float attackRange = 1f;

        [DataMember]
        private float attackGap = 1f;

        [DataMember]
        private float increaseSizePerSec = 0;

        [DataMember]
        private float dissolveStartTime = 4;

        [DataMember]
        private float dissolveEndTime = 8;

        [DataMember]
        private string enemyName;

        [DataMember]
        private List<string> jointNames = new List<string>();

        [DataMember]
        private List<vec3> positions = new List<vec3>();

        [DataMember]
        private List<Float4> quaternions = new List<Float4>();
        #endregion

        #region プロパティ
        public float FarRange
        {
            get { return farRange; }
        }

        public float MiddleRange
        {
            get { return middleRange; }
        }

        public float NearRange
        {
            get { return nearRange; }
        }

        public float WanderingSpeed
        {
            get { return wanderingSpeed; }
        }

        public float ChaseSpeed
        {
            get { return chaseSpeed; }
        }

        public float RotationSpeed
        {
            get { return rotationSpeed; }
        }

        public float AttackPower
        {
            get { return attackPower; }
        }

        public float AttackRange
        {
            get { return attackRange; }
        }

        public float AttackGap
        {
            get { return attackGap; }
        }

        public float IncreaseSizePerSec
        {
            get { return increaseSizePerSec; }
        }

        public float DissolveStartTime
        {
            get { return dissolveStartTime; }
        }

        public float DissolveEndTime
        {
            get { return dissolveEndTime; }
        }

        public string EnemyName
        {
            get { return enemyName; }
        }

        public List<string> JointNames
        {
            get { return jointNames; }
        }

        public List<vec3> Positions
        {
            get { return positions; }
        }

        public List<Float4> Quaternions
        {
            get { return quaternions; }
        }
        #endregion
    }
}
