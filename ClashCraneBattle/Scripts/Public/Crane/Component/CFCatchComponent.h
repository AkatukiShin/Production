// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "CFCatchComponent.generated.h"

class ACFCraneBase;
class UBoxComponent;

UCLASS( ClassGroup=(Custom), meta=(BlueprintSpawnableComponent) )
class NEWGAMEPROJECT_API UCFCatchComponent : public UActorComponent
{
	GENERATED_BODY()
// Variable
public:
	bool isCatchPrize = false;
protected:
	UPROPERTY()
	TObjectPtr<ACFCraneBase> PlayerOwner;

	float StopTime = 1.0f;
	// Component
	TObjectPtr<UBoxComponent> CatchCollision;
// Function
public:	
	// Sets default values for this component's properties
	UCFCatchComponent();

	void Initialize(float StopTimeValue);

protected:
	// Called when the game starts
	virtual void BeginPlay() override;
	
	UFUNCTION()
	void OnCatchCollisionOverlap(
		UPrimitiveComponent* ThisComp,
		AActor* OtherActor,
		UPrimitiveComponent* OtherComp,
		int32 OtherBodyIndex,
		bool bFromSweep,
		const FHitResult& SweepResult
	);
};
