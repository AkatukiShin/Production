// Copyright (c) 2025, Komiya Kousuke All rights reserved.

#pragma once

#include "CoreMinimal.h"
#include "CFInputDirection.h"
#include "CFInputHistory.h"
#include "Crane/CFCraneActionType.h"
#include "GameFramework/PlayerController.h"
#include "InputMappingContext.h"
#include "InputAction.h"
#include "InputActionValue.h"
#include "Interface/CFIOnPrizeAcquired.h"
#include "CFCraneController.generated.h"

DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FOnMoneyChangedDelegate, float, Money);

class ACFPlayer;
class UCFCraneData;
class UCFPlayerInputData;
class UMaterialParameterCollection;
/**
 * 
 */
UCLASS()
class NEWGAMEPROJECT_API ACFCraneController : public APlayerController
{
	GENERATED_BODY()

private:
	UPROPERTY(EditDefaultsOnly, Category = "PlayerData")
	UCFCraneData* CraneDatas;

	UPROPERTY(EditDefaultsOnly, Category = "Input|ActionDataAsset")
	UCFPlayerInputData* InputActions;

	UPROPERTY(EditDefaultsOnly, Category = "Input|StanAction")
	TObjectPtr<UInputAction> StanAction;

	UPROPERTY(EditDefaultsOnly, Category = "Input|GrabEachOtherAction")
	TObjectPtr<UInputAction> GrabEachOtherAction;

public:
	ACFCraneController();

	virtual void Tick(float DeltaSeconds) override;

	UFUNCTION(BlueprintCallable)
	virtual void ChangeIMC(ECFCraneActionType Type);

	UFUNCTION(BlueprintCallable)
	void GetPrize(float Money);

	UFUNCTION(BlueprintCallable)
	float GetMoney();

	UFUNCTION(BlueprintCallable)
	void SetCGripValue(float Value);

	UFUNCTION(BlueprintCallable)
	void AddMoney(const float AddMoney);

	UFUNCTION(BlueprintCallable)
	void ResetMoney();

	UFUNCTION()
	void CallRelease();

protected:

	virtual void BeginPlay() override;

	UFUNCTION()
	void SetupPlayerInput();

	UFUNCTION()
	virtual void OnPossess(APawn* InPawn) override;
	virtual void OnUnPossess() override;

	UFUNCTION()
	void CraneMove(const FInputActionValue& Value);

	UFUNCTION()
	void CraneCatch();

	UFUNCTION()
	void CraneRelease();

	UFUNCTION()
	void CraneRecovery(const FInputActionValue& Value);

	UFUNCTION()
	void CraneGrabEachOtherBattle(const FInputActionValue& Value);

	UFUNCTION()
	void CraneUpper();

	UFUNCTION()
	void CraneRotateAttack();

	//UFUNCTION()
	//void CranePause();

private:
	bool IsInputActionsBound = false;
	//bool IsOpenPauseWidget = false;

	UPROPERTY(VisibleAnywhere, Category = "Variables|Hp")
	float CHp = 0;

	UPROPERTY(VisibleAnywhere, Category = "Variables|Move")
	float CMoveSpeed = 0;
	UPROPERTY(VisibleAnywhere, Category = "Variables|Move")
	float CUpSpeed = 0;
	UPROPERTY(VisibleAnywhere, Category = "Variables|Move")
	float CDownSpeed = 0;

	UPROPERTY(VisibleAnywhere, Category = "Variables|Grip")
	float CGripValue = 3.0f;
	UPROPERTY(VisibleAnywhere, Category = "Variables|Grip")
	float CDecreaseGripValue = 0.5;
	UPROPERTY(VisibleAnywhere, Category = "Variables|Grip")
	float CIncreaseGripValue = 0.15f;
	UPROPERTY(VisibleAnywhere, Category = "Variables|Grip")
	float CMaxGripValue = 3.0f;
	UPROPERTY(VisibleAnywhere, Category = "Variables|Grip")
	float CMaxKakinGripValue = 6.0f;
	UPROPERTY(VisibleAnywhere, Category = "Variables|Grip")
	bool isKakin = false;
	UPROPERTY(VisibleAnywhere, Category = "Variables|Catch")
	bool isCatch = false;

	UPROPERTY(VisibleAnywhere, Category = "Variables|Command")
	float CCommandGraceTime = 0.18f;
	UPROPERTY(VisibleAnywhere, Category = "Variables|Attack")
	float CUpperAttackValue = 0;
	UPROPERTY(VisibleAnywhere, Category = "Variables|Attack")
	float CRotationAttackValue = 0;
	UPROPERTY(VisibleAnywhere, Category = "Variables|Attack")
	float CGrabAttackValue = 0;
	UPROPERTY(VisibleAnywhere, Category = "Variables|Kakin")
	float CChargingMoney = 0;
	UPROPERTY(VisibleAnywhere, Category = "Variables|Kakin")
	float CUpperSuccesMoney = 0;
	UPROPERTY(VisibleAnywhere, Category = "Variables|Kakin")
	float CRotationSuccesMoney = 0;

	UPROPERTY(VisibleAnywhere, Category = "Variables|Recovery")
	float CRecoveryValue = 0;

	UPROPERTY(VisibleAnywhere, Category = "Variables|GrabEachOther")
	float CGrabEachOtherIncreaseValue = 0;

	UPROPERTY(VisibleAnywhere, Category = "Variables|GrabEachOther")
	float CGrabEachOtherWinValue = 0;

	UPROPERTY(VisibleAnywhere, Category = "Variables|Input")
	float DeadZone = 0.4f;

	int8 LastStruggleDirection = 0;
	float DoubleTapDelay = 0.5f;
	float LastRapidUpInputTime = 0.0f;

	TArray<FInputHistory> InputLog;
	TArray<ECFInputDirection> CommandCheckLog;

	UPROPERTY(EditAnywhere, Category = "AnimMontage")
	UAnimMontage* RightUpper = NULL;

	UPROPERTY(EditAnywhere, Category = "AnimMontage")
	UAnimMontage* LeftUpper = NULL;

	UPROPERTY(EditAnywhere, Category = "AnimMontage")
	UAnimMontage* RightRotation = NULL;

	UPROPERTY(EditAnywhere, Category = "AnimMontage")
	UAnimMontage* LeftRotation = NULL;

	UPROPERTY(EditAnywhere, Category = "Material|Parameter")
	TObjectPtr<UMaterialParameterCollection> ClashingParameter;

	UPROPERTY(EditAnywhere, Category = "Material|Parameter")
	TObjectPtr<UMaterialParameterCollection> GripParameter;

	UPROPERTY()
	TObjectPtr<UUserWidget> PauseWidgetInstance;

	float ClashingParameterValue = 50;

	//FString PauseWidgetPath = "/Game/UMG_Test.UMG_Test_C";
	//TSubclassOf<class UUserWidget> PauseWidgetClass = TSoftClassPtr<UUserWidget>(FSoftObjectPath(*PauseWidgetPath)).LoadSynchronous();

public:
	UPROPERTY(EditAnywhere, Category = "Input|IMC")
	TObjectPtr<UInputMappingContext> CraneMappingContext;

	UPROPERTY(EditAnywhere, Category = "Input|IMC")
	TObjectPtr<UInputMappingContext> StanMappingContext;

	UPROPERTY(EditAnywhere, Category = "Input|IMC")
	TObjectPtr<UInputMappingContext> GrabEachOtherMappingContext;

	UPROPERTY(EditAnywhere, Category = "Input|IMC")
	TObjectPtr<UInputMappingContext> RoundEndMappingContext;

	UPROPERTY(BlueprintAssignable, Category = "Events")
	FOnMoneyChangedDelegate OnMoneyChanged;

	UPROPERTY(VisibleAnywhere, Category = "Variables|Kakin")
	float CMoney = 0;

	TObjectPtr<ACFPlayer> Player;

	FName GripName;
};
