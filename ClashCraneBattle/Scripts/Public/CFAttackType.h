// Copyright (c) 2025, Komiya Kousuke All rights reserved.

#pragma once

#include "CoreMinimal.h"
#include "CFAttackType.generated.h"

/**
 * 
 */
UENUM(BlueprintType)
enum class ECFAttackType : uint8
{
	RightUpper = 0,
	LeftUpper = 1,
	RightRotate = 2,
	LeftRotate = 3,
};
