// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "CFInputDirection.h"
#include "CFInputHistory.h"
#include "Crane/CFCraneData.h"
#include "Components/ActorComponent.h"
#include "InputActionValue.h"
#include "CFCommandComponent.generated.h"

class ACFCraneBase;

UCLASS( ClassGroup=(Custom), meta=(BlueprintSpawnableComponent) )
class NEWGAMEPROJECT_API UCFCommandComponent : public UActorComponent
{
	GENERATED_BODY()

// Variable
protected:
	UPROPERTY(VisibleAnywhere, Category = "Variables|Command")
	float CommandGraceTime = 0.3f;
	UPROPERTY(VisibleAnywhere, Category = "Variables|Input")
	float DeadZone = 0.4f;

	TArray<FInputHistory> InputLog;
	TArray<ECFInputDirection> CommandCheckLog;

	UPROPERTY()
	TObjectPtr<ACFCraneBase> PlayerOwner;

// Function
public:	
	// Sets default values for this component's properties
	UCFCommandComponent();

	void Initialize(float CommandGraceTimeValue, float DeadZoneValue);

	void InputCommand(const FInputActionValue& Value);
protected:
	// Called when the game starts
	virtual void BeginPlay() override;

	void CommandCheck();

	bool CommandSuccesCheck(
		const TArray<ECFInputDirection>& CommandLog,
		const TArray<ECFInputDirection>& CommandList
	) const;
public:	
	// Called every frame
	virtual void TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction) override;

};
