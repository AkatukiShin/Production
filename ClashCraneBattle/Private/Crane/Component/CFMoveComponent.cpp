// Fill out your copyright notice in the Description page of Project Settings.

#include "Crane/Component/CFMoveComponent.h"
#include "Crane/CFCraneBase.h"

// Sets default values for this component's properties
UCFMoveComponent::UCFMoveComponent()
{
	PrimaryComponentTick.bCanEverTick = true;
}


// Called when the game starts
void UCFMoveComponent::BeginPlay()
{
	Super::BeginPlay();

	PlayerOwner = Cast<ACFCraneBase>(GetOwner());

	if (!PlayerOwner)
	{
		UE_LOG(LogTemp, Error, TEXT("CFMoveComponent is Null"));
	}
	check(PlayerOwner);
	
}

void UCFMoveComponent::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	if (!PlayerOwner)	return;

	ECFCraneActionType CurrentState = PlayerOwner->GetCurrentState();

	if (CurrentState == ECFCraneActionType::Idle)	return;

	//FString StateName = UEnum::GetValueAsString(CurrentState);

	//UE_LOG(LogTemp, Log, TEXT("CurrentState: %s"), *StateName);

	switch (CurrentState)
	{
	case ECFCraneActionType::Descending:
		Down(DescentSpeed);
		break;
	case ECFCraneActionType::Rising:
		Up(RiseSpeed);
		break;
	default:
		break;
	}
}

void UCFMoveComponent::Up(float Value)
{
	const float TargetZ = PlayerOwner->GetBaseZLocation();
	const float CurrentZ = PlayerOwner->GetActorLocation().Z;

	if (FMath::IsNearlyEqual(CurrentZ, TargetZ, 3))
	{
		PlayerOwner->SetActorLocation(FVector(PlayerOwner->GetActorLocation().X, PlayerOwner->GetActorLocation().Y, TargetZ));
		PlayerOwner->NotifyArriveBaseZLocation();
	}
	else
	{
		PlayerOwner->AddMovementInput(FVector::UpVector, Value);
	}
}

void UCFMoveComponent::Down(float Value)
{
	PlayerOwner->AddMovementInput(FVector::UpVector, -Value);
	/*const float ZLimit = 95.0f;
	const FVector CurrentLocation = PlayerOwner->GetActorLocation();

	if (Value > 0.0f && CurrentLocation.Z <= ZLimit)
	{
		FVector NewLocation = CurrentLocation;
		NewLocation.Z = ZLimit;
		PlayerOwner->SetActorLocation(NewLocation);

		return;
	}

	PlayerOwner->AddMovementInput(FVector::UpVector, -Value);*/
}

void UCFMoveComponent::XYMove(const FInputActionValue& Value)
{
	if (!PlayerOwner)
	{
		PlayerOwner = Cast<ACFCraneBase>(GetOwner());
	}

	FVector2D MovementVector = Value.Get<FVector2D>();

	AController* Controller = PlayerOwner->GetController();
	if (!Controller)
	{
		return;
	}

	const FRotator Rotation = Controller->GetControlRotation();
	const FRotator YawRotation(0, Rotation.Yaw, 0);

	const FVector ForwardDirection = FRotationMatrix(YawRotation).GetUnitAxis(EAxis::X);
	const FVector RightDirection = FRotationMatrix(YawRotation).GetUnitAxis(EAxis::Y);

	PlayerOwner->AddMovementInput(FVector::ForwardVector, MovementVector.Y);
	PlayerOwner->AddMovementInput(FVector::RightVector, MovementVector.X);
}
