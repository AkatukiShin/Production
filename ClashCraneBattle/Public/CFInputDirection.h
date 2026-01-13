// Copyright (c) 2025, Komiya Kousuke All rights reserved.

#pragma once

#include "CoreMinimal.h"
#include "CFInputDirection.generated.h"
/**
 * 
 */
UENUM(BlueprintType)
enum class ECFInputDirection : uint8
{
	LeftDown	= 0,
	Down		= 1,
	RightDown	= 2,
	Left		= 3,
	Neutral		= 4,
	Right		= 5,
	LeftUp		= 6,
	Up			= 7,
	RightUp		= 8,
};