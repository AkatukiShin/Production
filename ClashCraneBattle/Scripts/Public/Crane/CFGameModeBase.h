// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/GameModeBase.h"
#include "CFGameModeBase.generated.h"

/**
 * 
 */
UCLASS()
class NEWGAMEPROJECT_API ACFGameModeBase : public AGameModeBase
{
	GENERATED_BODY()
	
public:
	virtual void BeginPlay();
};
