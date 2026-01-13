// Copyright (c) 2025, Komiya Kousuke All rights reserved.

#pragma once

#include "CoreMinimal.h"
#include "Crane/CFCraneBase.h"
#include "CFCraneActionType.h"
#include "CFPlayer.generated.h"

/**
 * 
 */
UCLASS()
class NEWGAMEPROJECT_API ACFPlayer : public ACFCraneBase
{
	GENERATED_BODY()

public:
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	ECFCraneActionType PlayerType = ECFCraneActionType::Idle;

public:
	ACFPlayer();

	UFUNCTION(BlueprintCallable)
	void SetPlayerType(ECFCraneActionType type);

	UFUNCTION(BlueprintCallable)
	ECFCraneActionType GetPlayerType();
private:
	TObjectPtr<USkeletalMeshComponent> SkeltalMeshComp;
};
