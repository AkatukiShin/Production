// Copyright (c) 2025, Komiya Kousuke All rights reserved.


#include "Crane/CFPlayer.h"
#include "Animation/AnimInstance.h"
#include "Animation/AnimMontage.h"

ACFPlayer::ACFPlayer()
{
	
}

void ACFPlayer::SetPlayerType(ECFCraneActionType type)
{
	PlayerType = type;
}

ECFCraneActionType ACFPlayer::GetPlayerType()
{
	return ECFCraneActionType();
}

