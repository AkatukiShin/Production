	// Fill out your copyright notice in the Description page of Project Settings.

#include "Crane/Component/CFCatchComponent.h"
#include "Crane/CFCraneBase.h"
#include "Components/BoxComponent.h"

// Sets default values for this component's properties
UCFCatchComponent::UCFCatchComponent()
{
}


// Called when the game starts
void UCFCatchComponent::BeginPlay()
{
	PlayerOwner = Cast<ACFCraneBase>(GetOwner());

	if (!PlayerOwner)
	{
		UE_LOG(LogTemp, Error, TEXT("CFMoveComponentがNULLです。Pawn以外のアクターにアタッチされていませんか？"));
		return;
	}

	CatchCollision = PlayerOwner->CatchJudgeCollision;
	CatchCollision->OnComponentBeginOverlap.AddDynamic(this, &UCFCatchComponent::OnCatchCollisionOverlap);
}

void UCFCatchComponent::Initialize(float StopTimeValue)
{
	StopTime = StopTimeValue;
}

void UCFCatchComponent::OnCatchCollisionOverlap(UPrimitiveComponent* ThisComp, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult)
{
	UE_LOG(LogTemp, Log, TEXT("OVERRAP"));
	if (!PlayerOwner || OtherActor == PlayerOwner)	return;
	ECFCraneActionType CurrentState = PlayerOwner->GetCurrentState();

	if (CurrentState == ECFCraneActionType::Descending)
	{
		PlayerOwner->NotifyObjectHit(OtherActor);
		UE_LOG(LogTemp, Log, TEXT("Notify"));
	}
	else if(PlayerOwner->isCatchPrize)
	{
		PlayerOwner->Release();
	}
}

