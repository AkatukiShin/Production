// Fill out your copyright notice in the Description page of Project Settings.

#include "Crane/Component/CFAttackComponent.h"
#include "Animation/AnimInstance.h"
#include "Animation/AnimMontage.h"
#include "Crane/CFCraneBase.h"
#include "Crane/CFPlayerState.h"
#include "Components/SphereComponent.h"
#include "CFAttackType.h"
#include "Kismet/GameplayStatics.h"

// Sets default values for this component's properties
UCFAttackComponent::UCFAttackComponent()
{
	// Set this component to be initialized when the game starts, and to be ticked every frame.  You can turn these features
	// off to improve performance if you don't need them.
	PrimaryComponentTick.bCanEverTick = true;

	// ...
}


// Called when the game starts
void UCFAttackComponent::BeginPlay()
{	
	PlayerOwner = Cast<ACFCraneBase>(GetOwner());

	if (!PlayerOwner)
	{
		UE_LOG(LogTemp, Error, TEXT("CFMoveComponent is NULL"));
		return;
	}

	AttackCollision = PlayerOwner->AttackCollision;
	AttackCollision->OnComponentBeginOverlap.AddDynamic(this, &UCFAttackComponent::OnAttackCollisionOverlap);
	AttackCollision->SetCollisionEnabled(ECollisionEnabled::NoCollision);

	AnimInstance = PlayerOwner->AnimInstance;

	if (!AnimInstance)
	{
		UE_LOG(LogTemp, Error, TEXT("AnimationInstance is NULL"))
	}
}

void UCFAttackComponent::Initialize(float UpperAttack, float RotateAttack)
{
	UpperAttackValue = UpperAttack;
	RotateAttackValue = RotateAttack;
}

// Called every frame
void UCFAttackComponent::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);

	// ...
}

void UCFAttackComponent::EXecuteAttack(ECFAttackType AttackType)
{
	if (!AnimInstance)
	{
		AnimInstance = PlayerOwner->AnimInstance;
	}

	if (isPlayingAnimation)	return;

	switch (AttackType)
	{
	case ECFAttackType::RightUpper:
		AttackValue = UpperAttackValue;
		AnimInstance->Montage_Play(RightUpper);
		break;
	case ECFAttackType::LeftUpper:
		AttackValue = UpperAttackValue;
		AnimInstance->Montage_Play(LeftUpper);
		break;
	case ECFAttackType::RightRotate:
		AttackValue = RotateAttackValue;
		AnimInstance->Montage_Play(RightRotate);
		break;
	case ECFAttackType::LeftRotate:
		AttackValue = RotateAttackValue;
		AnimInstance->Montage_Play(LeftRotate);
		break;
	default:
		return;
		break;
	}

	isPlayingAnimation = true;
}

void UCFAttackComponent::OnAttack()
{
	AttackCollision->SetCollisionEnabled(ECollisionEnabled::QueryOnly);
	UE_LOG(LogTemp, Log, TEXT("OnAttack"));
}

void UCFAttackComponent::OffAttack()
{
	AttackCollision->SetCollisionEnabled(ECollisionEnabled::NoCollision);
	isAttackHit = false;
	isPlayingAnimation = false;
	UE_LOG(LogTemp, Log, TEXT("OffAttack"));
}

void UCFAttackComponent::OnAttackCollisionOverlap(UPrimitiveComponent* ThisComp, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult)
{
	//AActor* Owner = GetOwner();
	//FString OwnerName = Owner ? Owner->GetName() : TEXT("NULL");
	//FString OtherName = OtherActor ? OtherActor->GetName() : TEXT("NULL");

	//UE_LOG(LogTemp, Warning, TEXT("Attack Hit Check: Attacker=[%s] vs Victim=[%s]"), *OwnerName, *OtherName);

	if (OtherActor == PlayerOwner || OtherActor == GetOwner())
	{
		return;
	}

	if (OtherActor->ActorHasTag(FName("Player")) && !isAttackHit)
	{
		UE_LOG(LogTemp, Log, TEXT("Overlap with: %s"), *OtherActor->GetName());

		UGameplayStatics::ApplyDamage(
			OtherActor,
			AttackValue,
			PlayerOwner->GetController(),
			PlayerOwner,
			UDamageType::StaticClass()
		);
		isAttackHit = true;

		AController* OwnerController = PlayerOwner->GetController();

		ACFCraneController* HumanController = Cast<ACFCraneController>(OwnerController);

		if (HumanController)
		{
			HumanController->AddMoney(AttackMoney);
		}
	}
}

