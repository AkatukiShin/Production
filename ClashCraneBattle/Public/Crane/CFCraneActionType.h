// Copyright (c) 2025, Komiya Kousuke All rights reserved.

#pragma once

#include "CoreMinimal.h"
#include "CFCraneActionType.generated.h"

/**
 *
 */
UENUM(BlueprintType)
enum class ECFCraneActionType : uint8
{
	Idle = 0,
	Catch = 1,
	Release = 2,
	Stan = 3,
	Front = 4,
	Right = 5,
	Back = 6,
	Left = 7,
	Attack = 8,
	GrabEachOther = 9,
	RoundEnd = 10,
	Descending = 11,
	Rising = 12,
};