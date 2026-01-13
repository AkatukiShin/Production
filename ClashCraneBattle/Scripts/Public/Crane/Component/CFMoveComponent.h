// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "InputActionValue.h"
#include "CFMoveComponent.generated.h"

class ACFCraneBase;

UCLASS( ClassGroup=(Custom), meta=(BlueprintSpawnableComponent) )
class NEWGAMEPROJECT_API UCFMoveComponent : public UActorComponent
{
	GENERATED_BODY()

// Variable
protected:
	UPROPERTY()
	TObjectPtr<ACFCraneBase> PlayerOwner;

	float DescentSpeed = 1.0f;
	float RiseSpeed = 1.0f;

// Function
public:	
	// Sets default values for this component's properties
	UCFMoveComponent();

protected:
	// Called when the game starts
	virtual void BeginPlay() override;
	virtual void TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction) override;

public:	
	void XYMove(const FInputActionValue& Value);

	void Up(float Value);

	void Down(float Value);
};
