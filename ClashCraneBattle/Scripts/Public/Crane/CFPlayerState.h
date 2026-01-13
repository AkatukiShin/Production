// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/PlayerState.h"
#include "CFPlayerState.generated.h"

DECLARE_DYNAMIC_MULTICAST_DELEGATE_TwoParams(FOnHPChangedDelegate, float, NewHp, float, MaxHp);

class ACFPlayer;
class ACFCraneBase;
class ACFCraneController;
class UCFCraneData;
class UMaterialParameterCollection;
UCLASS()
class NEWGAMEPROJECT_API ACFPlayerState : public APlayerState
{
	GENERATED_BODY()
private:
	UPROPERTY(EditDefaultsOnly, Category = "PlayerData")
	UCFCraneData* CraneDatas;

	TObjectPtr<ACFCraneBase> Player;
	TObjectPtr<ACFCraneController> CraneController;
protected:

public:
	virtual void BeginPlay() override;
	virtual void GetLifetimeReplicatedProps(TArray<FLifetimeProperty>& OutLifetimeProps) const override;

	void ModifyHP(float Delta);

	UFUNCTION(BlueprintPure, Category = "Status")
	float GetHP() const { return HP; }

	UFUNCTION(BlueprintPure, Category = "Status")
	float GetMaxHP() const;

	UFUNCTION(BlueprintCallable, BlueprintNativeEvent)
	void CraneBroken();

	virtual void CraneBroken_Implementation();

	UFUNCTION(BlueprintCallable, BlueprintNativeEvent)
	void CraneCured();

	virtual void CraneCured_Implementation();

	UFUNCTION(BlueprintCallable)
	void HpReset();

public:
	UPROPERTY()
	float HP = 100.f;

	UPROPERTY(BlueprintAssignable, Category = "Events")
	FOnHPChangedDelegate OnHPChanged;
	UPROPERTY(EditAnywhere, Category = "Material|Parameter")
	TObjectPtr<UMaterialParameterCollection> HpParameter;
	float HpParameterValue = 100;
	FName HpParameterName;
};
