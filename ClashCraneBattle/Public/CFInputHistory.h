// Copyright (c) 2025, Komiya Kousuke All rights reserved.

#pragma once

#include "CoreMinimal.h"
#include "CFInputDirection.h"
#include "CFInputHistory.generated.h"

USTRUCT()
struct FInputHistory
{
    GENERATED_BODY()

    UPROPERTY()
    ECFInputDirection Direction = ECFInputDirection::Neutral;

    UPROPERTY()
    float Duration = NULL;
};