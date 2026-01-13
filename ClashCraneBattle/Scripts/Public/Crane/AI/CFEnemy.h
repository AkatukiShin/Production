// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Crane/CFCraneBase.h"
#include "CFEnemy.generated.h"

/**
 * 
 */
UCLASS()
class NEWGAMEPROJECT_API ACFEnemy : public ACFCraneBase
{
	GENERATED_BODY()
// Variable

// Function
protected:
	UFUNCTION(BlueprintCallable)
	void XYMove(const FVector2D& Value);
};
