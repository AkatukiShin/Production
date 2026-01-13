// Copyright (c) 2025, Komiya Kousuke All rights reserved.

#pragma once

#include "CoreMinimal.h"
#include "CFSEType.generated.h"

/**
 * 
 */
UENUM(BlueprintType)
enum class CFSEType : uint8
{
	GongStart = 0,
	GongEnd = 1,
	Winner = 2,
	AddCoin = 3,
	Enhance = 4,
	CommandOn = 5,
	ApperCommand = 6,
	RoundCommand = 7,
	Broken = 8,
	Damage = 9,
	Cure = 10,
	Cured = 11,
	GetPrize = 12,
	GrabEnemy = 13,
	GrabPrize = 14,
	GrabEach = 15,
	GripOff = 16,
	MoveHorizon = 17,
	MoveDown = 18,
	MoveStop = 19,
	Select = 20,
	Selected = 21,
};
