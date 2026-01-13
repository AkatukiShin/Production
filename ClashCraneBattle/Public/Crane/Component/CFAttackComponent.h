// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "CFAttackType.h"
#include "CFAttackComponent.generated.h"

class ACFCraneBase;
class UAnimMontage;
class USphereComponent;

UCLASS( ClassGroup=(Custom), meta=(BlueprintSpawnableComponent) )
class NEWGAMEPROJECT_API UCFAttackComponent : public UActorComponent
{
	GENERATED_BODY()

// Variable
protected:
	bool isAttackHit = false;
	bool isPlayingAnimation = false;

	float UpperAttackValue = 10.0f;
	float RotateAttackValue = 15.0f;

	UPROPERTY(EditAnywhere, Category = "Money")
	float AttackMoney = 50;

	UPROPERTY(EditAnywhere, Category = "AnimMontage")
	TObjectPtr<UAnimMontage> RightUpper = NULL;

	UPROPERTY(EditAnywhere, Category = "AnimMontage")
	TObjectPtr<UAnimMontage> LeftUpper = NULL;

	UPROPERTY(EditAnywhere, Category = "AnimMontage")
	TObjectPtr<UAnimMontage> RightRotate = NULL;

	UPROPERTY(EditAnywhere, Category = "AnimMontage")
	TObjectPtr<UAnimMontage> LeftRotate = NULL;

	UPROPERTY()
	TObjectPtr<ACFCraneBase> PlayerOwner;

	UPROPERTY(VisibleAnywhere, Category = "Animation")
	UAnimInstance* AnimInstance;

	// Component
	TObjectPtr<USphereComponent> AttackCollision;

private:
	float AttackValue = 0;
// Function
public:	
	// Sets default values for this component's properties
	UCFAttackComponent();

	void Initialize(float UpperAttack, float RotateAttack);

protected:
	// Called when the game starts
	virtual void BeginPlay() override;

	UFUNCTION()
	void OnAttackCollisionOverlap(
		UPrimitiveComponent* ThisComp,
		AActor* OtherActor,
		UPrimitiveComponent* OtherComp,
		int32 OtherBodyIndex,
		bool bFromSweep,
		const FHitResult& SweepResult
	);

public:	
	// Called every frame
	virtual void TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction) override;

	void EXecuteAttack(ECFAttackType AttackType);

	UFUNCTION(BlueprintCallable)
	void OnAttack();

	UFUNCTION(BlueprintCallable)
	void OffAttack();
};
