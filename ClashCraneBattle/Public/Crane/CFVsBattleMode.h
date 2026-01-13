// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Crane/CFGameModeBase.h"
#include "CFVsBattleMode.generated.h"

/**
 * 
 */
UCLASS()
class NEWGAMEPROJECT_API ACFVsBattleMode : public ACFGameModeBase
{
	GENERATED_BODY()
	
public:
	ACFVsBattleMode();

	virtual void BeginPlay() override;

	UPROPERTY(EditAnywhere)
	int localPlayers = 2;
};
