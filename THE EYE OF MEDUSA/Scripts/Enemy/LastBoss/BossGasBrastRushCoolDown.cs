//=============================================================================
// <summary>
// BossGasBrastRushCoolDown 
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
    public class BossGassBrastRush : via.behaviortree.Action
    {
        [DataMember]
        private float cooldown = 20;
        private VariableBoolHandle isGasBrastRushCdHandle;
        private float defaultCooldown;

        public override void start(ActionArg arg)
        {
            isGasBrastRushCdHandle = new VariableBoolHandle(arg.UserVariables, str.makeHash("GasBlastRushCd"));
            defaultCooldown = cooldown;
        }

        public override void end(ActionArg arg)
        {
        }

        public override void update(ActionArg arg)
        {
            cooldown -= Application.ElapsedSecond;
            if (cooldown < 0)
            {
                isGasBrastRushCdHandle.Value = true;
                cooldown = defaultCooldown;
            }
        }
    }
}