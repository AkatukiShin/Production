// Copyright (c) 2025, Komiya Kousuke All rights reserved.
#pragma once

#include "CoreMinimal.h"
#include "Crane/CFCraneController.h"
#include "Crane/Component/CFMoveComponent.h"
#include "CFCraneActionType.h"
#include "CFAttackType.h"
#include "GameFramework/Character.h"
#include "InputActionValue.h"
#include "SubSystem/CFSoundManagerSubsystem.h"
#include "NiagaraFunctionLibrary.h"
#include "CFCraneBase.generated.h"

class UAnimMontage;
class UBoxComponent;
class UCFCraneData;
class UCFCatchComponent;
class UCFAttackComponent;
class UCFCommandComponent;
class USphereComponent;
class UCapsuleComponent;
class UStaticMeshComponent;

UCLASS()
class NEWGAMEPROJECT_API ACFCraneBase : public ACharacter
{
	GENERATED_BODY()

public:
	// Sets default values for this pawn's properties
	ACFCraneBase();

// Variable
public:
	bool isCatched = false;
	bool isCatchedReturn = false;
	TObjectPtr<UCFSoundManagerSubsystem> SoundManager;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Effects")
	TObjectPtr<UNiagaraSystem> Explosion;
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Effects")
	TObjectPtr<UNiagaraSystem> Damage;
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Effects")
	TObjectPtr<UNiagaraSystem> KnockOut;
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Effects")
	TObjectPtr<UNiagaraSystem> PicItem;
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Effects")
	TObjectPtr<UNiagaraSystem> SuccesCommand;

	UPROPERTY(EditAnywhere, Category = "Variables|GrabEachOther")
	float GrabEachOtherIncreaseValue = 3;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Variables|GrabPosition")
	FVector GrabPosition = FVector(0, 0, 0);

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Variables|GrabPosition")
	FRotator GrabRotation = FRotator(0, 0, 0);


	UPROPERTY(EditAnywhere, Category = "Variables|GrabEachOther")
	float GrabEachOtherWinValue = 100;
	UPROPERTY(EditAnywhere, Category = "Variables|GrabAttackValue")
	float GrabAttackValue = 0;
	UPROPERTY(EditAnywhere, Category = "Material|Parameter")
	FName CGripName;
	UPROPERTY(VisibleAnywhere, Category = "Animation")
	UAnimInstance* AnimInstance;

	// Component
	UPROPERTY(EditDefaultsOnly, Category = "CatchCollision")
	TObjectPtr<UBoxComponent> CatchJudgeCollision;
	UPROPERTY(EditDefaultsOnly, Category = "AttackCollision")
	TObjectPtr<USphereComponent> AttackCollision;

protected:
	ECFCraneActionType CurrentState = ECFCraneActionType::Idle;

	TObjectPtr<ACFCraneController> CraneController;

	bool isAttackHit = false;
	float CAttackValue = 20;
	UPROPERTY(EditAnywhere, Category = "Variables|Height")
	float BaseZLocation = 50.0f;
	FTimerHandle EnemyCatchTimerHandle;

	// Component
	UPROPERTY(VisibleAnywhere, Category = "Components")
	TObjectPtr<UCFMoveComponent> MoveComponent;
	UPROPERTY(VisibleAnywhere, Category = "Components")
	TObjectPtr<UCFCatchComponent> CatchComponent;
	UPROPERTY(VisibleAnywhere, Category = "Components")
	TObjectPtr<UCFAttackComponent> AttackComponent;
	UPROPERTY(VisibleAnywhere, Category = "Components")
	TObjectPtr<UCFCommandComponent> CommandComponent;
	UPROPERTY(VisibleAnywhere, Category = "Components")
	TObjectPtr<UCharacterMovementComponent> CharacterMovementComponent;

	TObjectPtr<USkeletalMeshComponent> CharacterMesh;
	TObjectPtr<UCapsuleComponent> CatchedPrize;

// Function
public:
	FORCEINLINE float GetBaseZLocation() const { return BaseZLocation; }
	FORCEINLINE UCFCommandComponent* GetCommandComponent() const { return CommandComponent; }
	void NotifyObjectHit(AActor* HitActor);
	void NotifyArriveBaseZLocation();
	void HandleCommand(ECFAttackType AttackType);
	void AddMoney(float AddMoneyValue);

	// BlueprintCallable
	UFUNCTION(BlueprintCallable)
	FORCEINLINE ECFCraneActionType GetCurrentState() const { return CurrentState; }

	UFUNCTION(BlueprintCallable)
	FORCEINLINE void SetCurrentState(ECFCraneActionType ChangeType) { CurrentState = ChangeType; }

	UFUNCTION(BlueprintCallable)
	void Attack(ECFAttackType AttackType);

	UFUNCTION(BlueprintCallable)
	void RecoverHP(float RecoverValue);

	UFUNCTION(BlueprintCallable)
	void RoundInitialize();

	virtual void Move(const FInputActionValue& Value);
protected:
	virtual void BeginPlay() override;

	void StartDescent();
	
protected:
	// Called when the game starts or when spawned
	virtual void EndPlay(const EEndPlayReason::Type EndPlayReason) override;

	UPROPERTY(EditAnywhere, Category = "PlayerData")
	TObjectPtr<UCFCraneData> CraneDatas;

	UFUNCTION()
	float TakeDamage(
		float DamageAmount,
		FDamageEvent const& DamageEvent,
		AController* EventInstigator,
		AActor* DamageCauser
	);

	UPROPERTY(EditAnywhere, Category = "CatchCollision")
	float ArmSphereRadius = 0;
public:	
	UPROPERTY(BlueprintReadOnly)
	bool isCatchPrize = false;
	UFUNCTION(BlueprintCallable)
	void CatchReset();

	// Called to bind functionality to input
	virtual void SetupPlayerInputComponent(class UInputComponent* PlayerInputComponent);
	


	UFUNCTION(BlueprintCallable)
	void Catch();

	UFUNCTION()
	void Release();


	UFUNCTION()
	bool GetisCatchPriza();
};
