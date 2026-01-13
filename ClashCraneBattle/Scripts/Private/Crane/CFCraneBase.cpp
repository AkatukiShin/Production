// Copyright (c) 2025, Komiya Kousuke All rights reserved.

#include "Crane/CFCraneBase.h"
#include "Crane/CFCraneData.h"
#include "Crane/CFPlayerState.h"
#include "Crane/CFCraneController.h"
#include "Crane/Component/CFCatchComponent.h"
#include "Crane/Component/CFAttackComponent.h"
#include "Crane/Component/CFCommandComponent.h"
#include "CFAttackType.h"
#include "Components/BoxComponent.h"
#include "Components/SphereComponent.h"
#include "Components/CapsuleComponent.h"
#include "Engine/TimerHandle.h"
#include "GameFramework/CharacterMovementComponent.h"
#include "Kismet/GameplayStatics.h"
#include "TimerManager.h"
#include "Animation/AnimInstance.h"
#include "Enum/CFSEType.h"

// Sets default values
ACFCraneBase::ACFCraneBase()
{
	float SphereRadius = 0.1f;

	CharacterMesh = GetMesh();

	CatchJudgeCollision = CreateDefaultSubobject<UBoxComponent>(TEXT("CatchJudgeCollision"));
	CatchJudgeCollision->SetupAttachment(CharacterMesh, FName("prize"));

	AttackCollision = CreateDefaultSubobject<USphereComponent>(TEXT("AttackCollision"));
	AttackCollision->SetupAttachment(CharacterMesh);

	MoveComponent = CreateDefaultSubobject<UCFMoveComponent>(TEXT("MoveComponent"));
	CatchComponent = CreateDefaultSubobject<UCFCatchComponent>(TEXT("CatchComponent"));
	AttackComponent = CreateDefaultSubobject<UCFAttackComponent>(TEXT("AttackComponent"));
	CommandComponent = CreateDefaultSubobject<UCFCommandComponent>(TEXT("CommandComponent"));

	CharacterMovementComponent = this->GetCharacterMovement();

	AnimInstance = CharacterMesh->GetAnimInstance();
}

// Called when the game starts or when spawned
void ACFCraneBase::BeginPlay()
{
	Super::BeginPlay();

	CatchJudgeCollision->AttachToComponent(GetMesh(), FAttachmentTransformRules::KeepRelativeTransform, FName("prize"));
	//CatchJudgeCollision->SetGenerateOverlapEvents(false);
	
	AttackCollision->AttachToComponent(GetMesh(), FAttachmentTransformRules::KeepRelativeTransform, FName("attack"));

	if (CatchComponent == nullptr)	CatchComponent = GetComponentByClass<UCFCatchComponent>();
	if (AttackComponent == nullptr)	AttackComponent = GetComponentByClass<UCFAttackComponent>();
	if (CommandComponent == nullptr)	CommandComponent = GetComponentByClass<UCFCommandComponent>();

	CatchComponent->Initialize(CraneDatas->StopTime);
	AttackComponent->Initialize(CraneDatas->UpperAttackValue, CraneDatas->RotationAttackValue);
	CommandComponent->Initialize(CraneDatas->CommandGraceTime, CraneDatas->DeadZone);

	CharacterMovementComponent->MaxFlySpeed = CraneDatas->MoveSpeed;


	AnimInstance = CharacterMesh->GetAnimInstance();
	if (AnimInstance == NULL)
	{
		UE_LOG(LogTemp, Log, TEXT("AnimInstance NULL"));
	}

	// GameInstance を取得
	UGameInstance* GI = GetGameInstance();

	if (GI)
	{
		SoundManager = GI->GetSubsystem<UCFSoundManagerSubsystem>();
	}
}

void ACFCraneBase::StartDescent()
{
	if (CurrentState == ECFCraneActionType::Idle)
	{
		SetCurrentState(ECFCraneActionType::Descending);
		UE_LOG(LogTemp, Log, TEXT("降下開始"));
	}
}

void ACFCraneBase::Attack(ECFAttackType AttackType)
{
	AttackComponent->EXecuteAttack(AttackType);
	SoundManager->CallSE(CFSEType::CommandOn);
	FVector Location = GetActorLocation();
	FRotator Rotation = GetActorRotation();
	UNiagaraFunctionLibrary::SpawnSystemAtLocation(GetWorld(), SuccesCommand, Location, Rotation);
}

void ACFCraneBase::RoundInitialize()
{
	SetCurrentState(ECFCraneActionType::Idle);
	isCatchPrize = false;
}

void ACFCraneBase::NotifyObjectHit(AActor* HitActor)
{
	if (CurrentState == ECFCraneActionType::Descending && !CatchedPrize)
	{
		if (HitActor->ActorHasTag("Prize"))
		{
			CatchedPrize = HitActor->GetComponentByClass<UCapsuleComponent>();
			FTransform OriginalWorldTransform = HitActor->GetActorTransform();
			USkeletalMeshComponent* PrizeMesh = HitActor->GetComponentByClass<USkeletalMeshComponent>();
			if (!CatchedPrize) return;

			SetCurrentState(ECFCraneActionType::Catch);

			FTransform ParentSocketTransform = CatchJudgeCollision->GetSocketTransform(FName("prize"));

			FName ChildSocketName = FName("item_teddybare_Neck");

			FTransform ChildBoneRelativeTransform = PrizeMesh->GetSocketTransform(ChildSocketName).GetRelativeTransform(HitActor->GetActorTransform());

			FTransform NewActorTransform = ChildBoneRelativeTransform.Inverse() * ParentSocketTransform;

			HitActor->SetActorTransform(NewActorTransform);

			ACharacter* HitCharacter = Cast<ACharacter>(HitActor);
			if (HitCharacter)
			{
				UCharacterMovementComponent* PrizeMoveComp = HitCharacter->GetCharacterMovement();
				if (PrizeMoveComp)
				{
					PrizeMoveComp->SetMovementMode(EMovementMode::MOVE_None);
				}
			}

			CatchedPrize->SetSimulatePhysics(false);
			CatchedPrize->SetEnableGravity(false);

			FAttachmentTransformRules AttachmentRules(
				EAttachmentRule::KeepWorld, 
				EAttachmentRule::KeepWorld,    
				EAttachmentRule::KeepWorld,    
				false                          
			);

			CatchedPrize->AttachToComponent(CatchJudgeCollision, AttachmentRules, FName("prize"));

			CatchedPrize->SetWorldScale3D(FVector(0.3f, 0.3f, 0.3f));
			HitActor->SetActorTransform(OriginalWorldTransform);

			isCatchPrize = true;

			FVector Location = GetActorLocation();
			FRotator Rotation = GetActorRotation();
			UNiagaraFunctionLibrary::SpawnSystemAtLocation(GetWorld(), PicItem, Location, Rotation);

			UE_LOG(LogTemp, Log, TEXT("Prize Hit"));
		}
		else
		{
			UE_LOG(LogTemp, Log, TEXT("Other Hit"));
		}

		SetCurrentState(ECFCraneActionType::Rising);
	}
	else if (CurrentState == ECFCraneActionType::Descending && isCatchPrize)
	{
		SetCurrentState(ECFCraneActionType::Rising);
	}
}

void ACFCraneBase::NotifyArriveBaseZLocation()
{
	if (CurrentState == ECFCraneActionType::Rising)
	{
		UE_LOG(LogTemp, Log, TEXT("BaseZLocation Arrived"));
		SetCurrentState(ECFCraneActionType::Idle);
	}
}

void ACFCraneBase::HandleCommand(ECFAttackType AttackType)
{
	if (CurrentState != ECFCraneActionType::Idle)	return;

	if (CurrentState == ECFCraneActionType::Idle && AttackComponent)
	{
		Attack(AttackType);
	}
}

void ACFCraneBase::RecoverHP(float RecoverValue)
{
	if (ACFPlayerState* PS = GetPlayerState<ACFPlayerState>())
	{
		PS->ModifyHP(-RecoverValue);
	}
}

void ACFCraneBase::EndPlay(const EEndPlayReason::Type EndPlayReason)
{
	Super::EndPlay(EndPlayReason);
}

void ACFCraneBase::CatchReset()
{
	isCatchPrize = false;
}

// Called to bind functionality to input
void ACFCraneBase::SetupPlayerInputComponent(UInputComponent* PlayerInputComponent)
{
	Super::SetupPlayerInputComponent(PlayerInputComponent);
}

void ACFCraneBase::Move(const FInputActionValue& Value)
{
	if (CurrentState == ECFCraneActionType::Idle && !isCatchPrize)
	{
		CharacterMovementComponent->MaxFlySpeed = CraneDatas->MoveSpeed;
		MoveComponent->XYMove(Value);
	}
	else if (CurrentState == ECFCraneActionType::Idle && isCatchPrize)
	{
		CharacterMovementComponent->MaxFlySpeed = CraneDatas->CatchedMoveSpeed;
		MoveComponent->XYMove(Value);
	}
}

void ACFCraneBase::Catch()
{
	CatchJudgeCollision->SetGenerateOverlapEvents(true);
	StartDescent();
	if (isCatched)	isCatchedReturn = true;
}

void ACFCraneBase::Release()
{
	if (CatchJudgeCollision)
	{
		CatchJudgeCollision->SetGenerateOverlapEvents(false);
	}
	isAttackHit = false;

	//UE_LOG(LogTemp, Log, TEXT("Release Start! Catch"));

	if (!IsValid(CatchedPrize)) return;

	SetCurrentState(ECFCraneActionType::Release);

	AActor* OwnerActor = CatchedPrize->GetOwner();
	if (ACharacter* PrizeCharacter = Cast<ACharacter>(OwnerActor))
	{
		UCharacterMovementComponent* PrizeMove = PrizeCharacter->GetCharacterMovement();
		if (PrizeMove)
		{
			PrizeMove->SetMovementMode(EMovementMode::MOVE_Falling);
		}
	}
	CatchedPrize->SetSimulatePhysics(true);
	CatchedPrize->SetEnableGravity(true);
	CatchedPrize->DetachFromComponent(FDetachmentTransformRules::KeepWorldTransform);

	CatchedPrize = NULL;
	isCatchPrize = false;

	SetCurrentState(ECFCraneActionType::Idle);
}

bool ACFCraneBase::GetisCatchPriza()
{
	return isCatchPrize;

}

float ACFCraneBase::TakeDamage(float DamageAmount, FDamageEvent const& DamageEvent, AController* EventInstigator, AActor* DamageCauser)
{
	float ActualApplied = Super::TakeDamage(DamageAmount, DamageEvent, EventInstigator, DamageCauser);

	if (ACFPlayerState* PS = GetPlayerState<ACFPlayerState>())
	{
		UE_LOG(LogTemp, Warning, TEXT("TakeDamage: Victim=[%s] is modifying HP of PlayerState=[%s]"), *GetName(), *PS->GetName());
		if (CurrentState != ECFCraneActionType::Stan)
		{
			SoundManager->CallSE(CFSEType::Damage);

			FVector Location = GetActorLocation();
			FRotator Rotation = GetActorRotation();
			UNiagaraFunctionLibrary::SpawnSystemAtLocation(GetWorld(), Damage, Location, Rotation);
		}
		PS->ModifyHP(ActualApplied);
	}

	//アニメーションを止める処理
	//AnimInstance->StopAllMontages(0.3f);

	return ActualApplied;
}
