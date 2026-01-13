//=============================================================================
// <summary>
// LastBoss 
// </summary>
// <author>CGC_12_小宮 孝介</author>
//=============================================================================
using System;
using System.Collections.Generic;
using blackfilter;
using via;
using via.attribute;
using via.behaviortree;
using via.physics;
using via.userdata;

namespace blackfilter
{
	public class LastBoss : HumanEnemy, IBossAgent
	{
        [DataMember]
        public GameObjectRef blastRushGasObj;
        private float sekikaValue = 0;
        private VariableBoolHandle isTwoPartSekika;

        public override void start()
        {
            base.start();
            if(blastRushGasObj.Target is not null)
            {
                blastRushGasObj.Target.DrawSelf = false;
            }
            isTwoPartSekika = new VariableBoolHandle(behaviorTreeComponent.findUserVariable(), via.str.makeHash("isTwoPartSekika"));
        }
        public override void update()
        {
            base.update();
            sekikaValue = (headSekikaValue + bodySekikaValue + rightArmSekikaValue + leftArmSekikaValue + rightLegSekikaValue + leftLegSekikaValue);
            if(sekikaValue > 2.0f)
            {
                isTwoPartSekika.Value = true;
            }
        }


        // IBossAgent Begin
        public BehaviorTree getBehaviorTree()
        {
            return behaviorTreeComponent;
        }
        // IBossAgent End
    }
}
